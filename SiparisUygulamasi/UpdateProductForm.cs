using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SiparisUygulamasi
{
    public partial class UpdateProductForm : Form
    {
        private string connectionString = @"Server=PARZIVAL;Database=OrderAndStockDB;Trusted_Connection=True;";
        private LogManager logManager;


        public UpdateProductForm()
        {
            InitializeComponent();
            this.Load += UpdateProductForm_Load; // load olayını bağla
            logManager = new LogManager(connectionString);
        }

        private void UpdateProductForm_Load(object sender, EventArgs e)
        {
            LoadProductNames();
           
        }

        private void LoadProductNames()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ProductName FROM Products";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    cmbUpdateProduct.Items.Clear(); 

                    while (reader.Read())
                    {
                        string productName = reader["ProductName"].ToString();
                        cmbUpdateProduct.Items.Add(productName);
                    }
                    reader.Close(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürün adları yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            if (cmbUpdateProduct.SelectedIndex != -1)
            {
                string selectedProduct = cmbUpdateProduct.SelectedItem.ToString();
                decimal newPrice;
                int newStock;

                if (!decimal.TryParse(txtUpdatePrice.Text, out newPrice) || !int.TryParse(txtUpdateStock.Text, out newStock))
                {
                    MessageBox.Show("Lütfen geçerli bir fiyat ve stok miktarı girin.");
                    return;
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "UPDATE Products SET Price = @price, Stock = @stock WHERE ProductName = @name";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@price", newPrice);
                        cmd.Parameters.AddWithValue("@stock", newStock);
                        cmd.Parameters.AddWithValue("@name", selectedProduct);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Ürün başarıyla güncellendi!");

                        // log kaydı ekle
                        logManager.AddAdminLog(
                            null, // CustomerID
                            null, // OrderID
                            "Ürün Güncelleme", // LogType
                            "Admin", // CustomerType
                            selectedProduct, 
                            newStock, 
                            $"Admin '{selectedProduct}' adlı ürünü güncelledi. Yeni fiyat: {newPrice}, Yeni stok: {newStock}");

                        this.Close(); 
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ürün güncellenirken bir hata oluştu: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen bir ürün seçin.");
            }
        }


    }
}
