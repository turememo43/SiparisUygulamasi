using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SiparisUygulamasi
{
    public class OrderLoader
    {
        private string connectionString;

        public OrderLoader(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void LoadApprovedOrders(DataGridView dgvApprovedOrders)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT OrderID, CustomerID, ProductID, Quantity, TotalPrice, OrderDate, PriorityScore, WaitingTime, OrderStatus 
                        FROM (
                            SELECT TOP 3 OrderID, CustomerID, ProductID, Quantity, TotalPrice, OrderDate, PriorityScore, WaitingTime, OrderStatus 
                            FROM Orders 
                            WHERE OrderStatus = 'Approved' OR OrderStatus = 'Cancelled'
                            ORDER BY OrderID DESC
                        ) AS TopOrders
                        ORDER BY OrderID ASC";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable approvedOrders = new DataTable();
                    adapter.Fill(approvedOrders);

                    dgvApprovedOrders.DataSource = null;
                    dgvApprovedOrders.DataSource = approvedOrders;
                    ConfigureDataGridView(dgvApprovedOrders, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Approved siparişler yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        public void LoadInProgressOrders(DataGridView dgvInProgressOrders)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT OrderID, CustomerID, ProductID, Quantity, TotalPrice, OrderDate, PriorityScore, WaitingTime, OrderStatus
                        FROM Orders 
                        WHERE OrderStatus = 'In Progress'";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable inProgressOrders = new DataTable();
                    adapter.Fill(inProgressOrders);

                    dgvInProgressOrders.DataSource = null;
                    dgvInProgressOrders.DataSource = inProgressOrders;
                    ConfigureDataGridView(dgvInProgressOrders, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("In Progress siparişler yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        public void LoadPendingOrders(DataGridView dgvPendingOrders)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT OrderID, CustomerID, ProductID, Quantity, TotalPrice, OrderDate, PriorityScore, WaitingTime, OrderStatus 
                        FROM Orders 
                        WHERE OrderStatus = 'Pending'
                        ORDER BY PriorityScore DESC";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable pendingOrders = new DataTable();
                    adapter.Fill(pendingOrders);

                    dgvPendingOrders.DataSource = null;
                    dgvPendingOrders.DataSource = pendingOrders;
                    ConfigureDataGridView(dgvPendingOrders, false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Pending siparişler yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        private void ConfigureDataGridView(DataGridView dgv, bool showColumnHeaders)
        {
            dgv.ColumnHeadersVisible = showColumnHeaders;
            dgv.RowHeadersVisible = false;
            dgv.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgv.AllowUserToAddRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ScrollBars = ScrollBars.None;

            // renklendirme işlemi için CellFormatting olayını bağla
            dgv.CellFormatting += (sender, e) =>
            {
                if (dgv.Columns[e.ColumnIndex].Name == "OrderStatus" && e.Value != null)
                {
                    string status = e.Value.ToString();
                    switch (status)
                    {
                        case "Pending":
                            e.CellStyle.BackColor = Color.Blue;
                            e.CellStyle.ForeColor = Color.White;
                            break;

                        case "In Progress":
                            e.CellStyle.BackColor = Color.Yellow;
                            e.CellStyle.ForeColor = Color.Black;
                            break;

                        case "Approved":
                            e.CellStyle.BackColor = Color.Green;
                            e.CellStyle.ForeColor = Color.White;
                            break;

                        case "Cancelled":
                            e.CellStyle.BackColor = Color.Red;
                            e.CellStyle.ForeColor = Color.White;
                            break;

                        default:
                            e.CellStyle.BackColor = Color.White;
                            e.CellStyle.ForeColor = Color.Black;
                            break;
                    }
                }
            };
        }
    }
}
