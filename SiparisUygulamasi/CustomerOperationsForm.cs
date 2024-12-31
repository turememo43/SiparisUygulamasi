using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiparisUygulamasi
{
    public partial class CustomerOperationsForm : Form
    {
        private string connectionString;
        private LogManager logManager;
        private AdminForm adminForm;

        public CustomerOperationsForm(string connectionString, LogManager logManager, AdminForm adminForm)
        {
            InitializeComponent();
            this.connectionString = connectionString;
            this.logManager = logManager;
            this.adminForm = adminForm;

            LoadCustomers();
        }

        private void LoadCustomers()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CustomerID, CustomerName, CustomerType FROM Customers";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable customersTable = new DataTable();
                    adapter.Fill(customersTable);

                    cmbCustomers.DataSource = customersTable;
                    cmbCustomers.DisplayMember = "CustomerName"; 
                    cmbCustomers.ValueMember = "CustomerID"; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Müşteriler yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void btnUpgradeToPremium_Click_1(object sender, EventArgs e)
        {
            if (cmbCustomers.SelectedItem == null)
            {
                MessageBox.Show("Lütfen bir müşteri seçin!");
                return;
            }

            int customerId = (int)cmbCustomers.SelectedValue;
            string customerName = ((DataRowView)cmbCustomers.SelectedItem)["CustomerName"].ToString();

            // siparişler onaylanıyorsa işlemi sıraya al
            if (adminForm.isOrderApprovalInProgress)
            {
                adminForm.pendingCustomerUpdates.Add(customerId); // AdminForm'daki kuyruğa ekle
                MessageBox.Show($"{customerName} başarıyla Premium statüsüne alınmak üzere sıraya alındı!");
                return;
            }

            
            UpgradeCustomerToPremium(customerId);
        }

        public void ProcessPendingCustomerUpdates()
        {
            foreach (int customerId in adminForm.pendingCustomerUpdates)
            {
                UpgradeCustomerToPremium(customerId); 
            }

            adminForm.pendingCustomerUpdates.Clear();
        }

        private void UpgradeCustomerToPremium(int customerId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE Customers SET CustomerType = 'Premium' WHERE CustomerID = @customerId";
                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@customerId", customerId);
                    cmd.ExecuteNonQuery();

                    string customerName = GetCustomerName(customerId);

                    // log kaydı oluştur
                    logManager.AddLog(customerId, null, "Bilgilendirme", "Standard", null, null,
                        $"{customerName} adlı müşteri Premium seviyeye yükseltildi.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Müşteri ID {customerId} güncellenirken bir hata oluştu: {ex.Message}");
                }
            }
        }

        private string GetCustomerName(int customerId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CustomerName FROM Customers WHERE CustomerID = @customerId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@customerId", customerId);
                    object result = cmd.ExecuteScalar();
                    return result?.ToString() ?? "Bilinmiyor";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Müşteri adı alınırken bir hata oluştu: " + ex.Message);
                    return "Bilinmiyor";
                }
            }
        }
    }
}
