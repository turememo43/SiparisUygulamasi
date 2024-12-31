using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows.Forms;

namespace SiparisUygulamasi
{
    public partial class Form1 : Form
    {
        

        string connectionString = @"Server=PARZIVAL;Database=OrderAndStockDB;Trusted_Connection=True;";
        private OrderForm activeOrderForm = null;
        private AdminForm adminForm;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadCustomers2(); // müşteri verilerini yükle
            ConfigureDataGridView3();
        }

        public void LoadCustomers2()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Customers";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);

                    if (dataTable.Rows.Count > 0)
                    {
                        dgvCustomers.DataSource = dataTable;
                    }
                    else
                    {
                        MessageBox.Show("Customers tablosunda veri bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("LoadCustomers hatası: " + ex.Message);
                }
            }
        }

        private void btnAdmin_Click(object sender, EventArgs e)
        {
            string adminPassword = "0000"; // admin şifresi
            string inputPassword = Microsoft.VisualBasic.Interaction.InputBox("Admin şifresini giriniz:", "Admin Giriş", "");

            if (inputPassword == adminPassword)
            {
                AdminForm adminForm = new AdminForm(this); 
                adminForm.Show(); 
            }
            else
            {
                MessageBox.Show("Yanlış şifre. Admin girişi başarısız.");
            }
        }

        

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            string customerIdInput = Microsoft.VisualBasic.Interaction.InputBox("Müşteri ID'yi giriniz:", "Müşteri Giriş", "");
            string passwordInput = Microsoft.VisualBasic.Interaction.InputBox("Şifreyi giriniz:", "Müşteri Giriş", "");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Customers WHERE CustomerID = @customerId AND Password = @password";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@customerId", customerIdInput);
                    cmd.Parameters.AddWithValue("@password", passwordInput);

                    int result = (int)cmd.ExecuteScalar();
                    if (result > 0)
                    {
                        int customerId = int.Parse(customerIdInput);

                        if (activeOrderForm == null || activeOrderForm.IsDisposed)
                        {
                            activeOrderForm = new OrderForm(customerId);
                            activeOrderForm.Show();
                        }
                        else
                        {
                            MessageBox.Show("Bu müşteri için zaten bir sipariş formu açık.");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Geçersiz müşteri bilgileri.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void ConfigureDataGridView3()
        {
            dgvCustomers.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvCustomers.RowHeadersVisible = false;
            dgvCustomers.AutoGenerateColumns = false;
        }      
    }
}
