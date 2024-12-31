using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SiparisUygulamasi
{
    public partial class DeleteProductForm : Form
    {
        private string connectionString = @"Server=PARZIVAL;Database=OrderAndStockDB;Trusted_Connection=True;";
        private LogManager logManager;
        public DeleteProductForm()
        {
            InitializeComponent();
        }

        private void DeleteProductForm_Load(object sender, EventArgs e)
        {
            LoadProductNames(); // ürün adlarını yükle
            logManager = new LogManager(connectionString);
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

                    cmbDeleteProduct.Items.Clear();
                    while (reader.Read())
                    {
                        cmbDeleteProduct.Items.Add(reader["ProductName"].ToString());
                    }
                    reader.Close(); 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürün adları yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void btnDeleteProduct_Click2(object sender, EventArgs e)
        {
            if (cmbDeleteProduct.SelectedIndex != -1)
            {
                string selectedProduct = cmbDeleteProduct.SelectedItem.ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM Products WHERE ProductName = @name";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@name", selectedProduct);
                        cmd.ExecuteNonQuery();

                        // log kaydı ekle
                        string adminName = "Admin";
                        logManager.AddLog(
                            null,
                            null,
                            "Ürün Silme",
                            adminName,
                            selectedProduct,
                            null,
                            $"Admin {selectedProduct} adlı ürünü sildi."
                        );

                        MessageBox.Show("Ürün başarıyla silindi!");
                        this.Close(); // işlem tamamlandıktan sonra formu kapat
                        LoadProductNames(); // ComboBox'ı güncelle
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ürün silerken bir hata oluştu: " + ex.Message);

                        // hata durumunda log kaydı ekle
                        logManager.AddLog(
                            null,
                            null,
                            "Hata",
                            "Admin",
                            selectedProduct,
                            null,
                            $"Ürün silme işlemi sırasında bir hata oluştu: {ex.Message}"
                        );
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
