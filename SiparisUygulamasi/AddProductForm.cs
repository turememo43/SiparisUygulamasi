using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SiparisUygulamasi
{
    public partial class AddProductForm : Form
    {
        private string connectionString = @"Server=PARZIVAL;Database=OrderAndStockDB;Trusted_Connection=True;";

        private LogManager logManager;

        public AddProductForm()
        {
            InitializeComponent();
            logManager = new LogManager(connectionString);
        }

        private void btnSaveProduct_Click_2(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO Products (ProductName, Stock, Price) VALUES (@name, @stock, @price)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtProductName.Text);
                    cmd.Parameters.AddWithValue("@stock", decimal.Parse(txtStockQuantity.Text));
                    cmd.Parameters.AddWithValue("@price", int.Parse(txtPrice.Text));
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Ürün başarıyla eklendi!");

                    // log kaydı ekle
                    logManager.AddAdminLog(
                        null,
                        null,
                        "Ürün Ekleme",
                        "Admin",
                        txtProductName.Text,
                        null,
                        $"Admin '{txtProductName.Text}' adlı ürün ekledi. Fiyatı: {txtPrice.Text}, Stok miktarı: {txtStockQuantity.Text}");

                    this.Close(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürün eklerken bir hata oluştu: " + ex.Message);
                }
            }
        }
    }

}

