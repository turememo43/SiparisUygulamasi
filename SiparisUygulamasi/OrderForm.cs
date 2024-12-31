using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace SiparisUygulamasi
{
    public partial class OrderForm : Form
    {
        string connectionString = @"Server=PARZIVAL;Database=OrderAndStockDB;Trusted_Connection=True;";

        

        public int CustomerId { get; set; } 
        
        private LogManager logManager;
        private BackgroundWorker backgroundWorker;



        public OrderForm(int customerId)
        {
            InitializeComponent();
            this.CustomerId = customerId;

            // load olayını tanımlayın
            this.Load += OrderForm_Load;

            // backgroundWorker
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            backgroundWorker.WorkerSupportsCancellation = true;

            logManager = new LogManager(connectionString);

            LoadCustomerDetails();
            LoadProducts();
            LoadDataGridView2();
            ConfigureDataGridView4();
            dgvProducts2.CellFormatting += dgvProducts2_CellFormatting;

            
            LoadProductsToComboBox();
            cmbProducts.SelectedIndexChanged += cmbProducts_SelectedIndexChanged_1;

            this.FormClosing += OrderForm_FormClosing;

        }

        // form yüklendiğinde çağrılır
        private void OrderForm_Load(object sender, EventArgs e)
        {
            // backgroundWorker başlatılır
            backgroundWorker.RunWorkerAsync();
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!backgroundWorker.CancellationPending)
            {
                System.Threading.Thread.Sleep(1000); // 1 saniye bekleme

                if (this.InvokeRequired && !this.IsDisposed && !this.Disposing)
                {
                    try
                    {
                        this.Invoke(new Action(() =>
                        {
                            if (!this.IsDisposed && !this.Disposing)
                            {
                                LoadDataGridView2(); // dgv güncelle
                            }
                        }));
                    }
                    catch (ObjectDisposedException)
                    {
                        // form kapatıldıysa veya Dispose edildiğinde işlemi durdur
                        break;
                    }
                }
            }
        }




        // işlem tamamlandığında
        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Güncelleme işlemi durduruldu.");
            }
        }



        // form kapanırken işlemi durdur
        private void OrderForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker != null && backgroundWorker.IsBusy)
            {
                backgroundWorker.CancelAsync(); // arka plan işlemini iptal et
                while (backgroundWorker.IsBusy)
                {
                    Application.DoEvents(); // işlemi tamamlaması için uygulamayı beklet
                }
            }
        }







        private void LoadCustomerDetails()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CustomerName, Budget FROM Customers WHERE CustomerID = @id";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", CustomerId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        lblCustomerName.Text = "Müşteri: " + reader["CustomerName"].ToString();
                        lblCustomerBudget.Text = "Bakiye: " + reader["Budget"].ToString() + " TL";
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Müşteri bilgisi yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        public void LoadProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ProductID, ProductName, Stock, Price FROM Products";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cmbProducts.Items.Add(new { Text = reader["ProductName"].ToString(), Value = reader["ProductID"], Stock = reader["Stock"], Price = reader["Price"] });
                    }
                    reader.Close();

                    cmbProducts.DisplayMember = "Text";
                    cmbProducts.ValueMember = "Value";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürünler yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        private void LoadProductsToComboBox()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ProductID, ProductName, Stock, Price FROM Products";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable productTable = new DataTable();
                    adapter.Fill(productTable);

                    cmbProducts.DataSource = productTable;
                    cmbProducts.DisplayMember = "ProductName"; 
                    cmbProducts.ValueMember = "ProductID";

                    // ilk ürünü seçili yap
                    if (cmbProducts.Items.Count > 0)
                    {
                        cmbProducts.SelectedIndex = 0;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürünler yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        private void cmbProducts_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem == null)
                return;

            // seçili ürünün ProductID'sini al
            int selectedProductId = Convert.ToInt32((cmbProducts.SelectedItem as DataRowView)["ProductID"]);

            // güncel stok ve fiyat bilgisini veritabanından al
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT Stock, Price FROM Products WHERE ProductID = @productId";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@productId", selectedProductId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int stock = reader.GetInt32(0); // stok bilgisi
                            decimal price = reader.GetDecimal(1); // fiyat bilgisi

                            // stok ve fiyat bilgilerini güncelle
                            lblProductStock.Text = $"Stok: {stock}";
                            lblProductPrice.Text = $"Fiyat: {price} TL";
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Stok bilgisi alınırken hata oluştu: " + ex.Message);
                }
            }
        }


        private void LoadDataGridView2()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT ProductID, ProductName, Stock, Price FROM Products";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    System.Data.DataTable dataTable = new System.Data.DataTable();
                    adapter.Fill(dataTable);
                    dgvProducts2.DataSource = dataTable;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürünler yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }


        private void ConfigureDataGridView4()
        {
            dgvProducts2.RowHeadersVisible = false;
            dgvProducts2.AutoGenerateColumns = false;
            dgvProducts2.EditMode = DataGridViewEditMode.EditProgrammatically;
        }

        private void dgvProducts2_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProducts2.Columns[e.ColumnIndex].Name == "Stock") // "Stock" sütununu kontrol et
            {
                if (e.Value != null && int.TryParse(e.Value.ToString(), out int stockValue))
                {
                    if (stockValue < 10)
                    {
                        e.CellStyle.BackColor = Color.Red; 
                        e.CellStyle.ForeColor = Color.White;
                    }
                    else if (stockValue >= 10 && stockValue <= 50)
                    {
                        e.CellStyle.BackColor = Color.Yellow;
                        e.CellStyle.ForeColor = Color.Black; 
                    }
                    else if (stockValue > 50)
                    {
                        e.CellStyle.BackColor = Color.Green; 
                        e.CellStyle.ForeColor = Color.White; 
                    }
                }
            }
        }

        private string GetCustomerName(int customerId)
        {
            string customerName = string.Empty;
            string connectionString = @"Server=PARZIVAL;Database=OrderAndStockDB;Trusted_Connection=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CustomerName FROM Customers WHERE CustomerID = @customerID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@customerID", customerId);

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        customerName = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Müşteri adı alınırken bir hata oluştu: " + ex.Message);
                }
            }

            return customerName;
        }

        private string GetCustomerType(int customerId)
        {
            string customerType = "Bilinmiyor";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT CustomerType FROM Customers WHERE CustomerID = @customerID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@customerID", customerId);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        customerType = result.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Müşteri tipi alınırken bir hata oluştu: " + ex.Message);
                }
            }
            return customerType;
        }





        private void btnPlaceOrder_Click_1(object sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem == null || nudQuantity.Value <= 0)
            {
                MessageBox.Show("Lütfen bir ürün seçin ve adet girin.");
                return;
            }

            // seçilen ürün bilgilerini al
            DataRowView selectedRow = cmbProducts.SelectedItem as DataRowView;

            if (selectedRow != null)
            {
                int productId = Convert.ToInt32(selectedRow["ProductID"]);
                string productName = selectedRow["ProductName"].ToString();
                int quantity = (int)nudQuantity.Value;

                int stock = 0;
                decimal price = 0; 

                // güncel stok ve fiyatı veritabanından al
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "SELECT Stock, Price FROM Products WHERE ProductID = @productId";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@productId", productId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                stock = reader.GetInt32(0); 
                                price = reader.GetDecimal(1); 
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Güncel stok ve fiyat alınırken hata oluştu: " + ex.Message);
                        return;
                    }
                }

                // geçersiz miktar kontrolü
                if (quantity <= 0 || quantity > stock)
                {
                    MessageBox.Show("Geçersiz miktar. Lütfen stoğa uygun bir değer girin.");

                    if (quantity > stock)
                    {
                        string customerName = GetCustomerName(CustomerId);
                        string customerType = GetCustomerType(CustomerId);

                        logManager.AddLog(CustomerId, null, "Hata", customerType, productName, quantity,
                            $"{customerName} ({customerType}) adlı müşterinin siparişi stok yetersizliği nedeniyle iptal oldu.");
                    }
                    return;
                }

                // en fazla 5 adet kontrolü
                if (quantity > 5)
                {
                    MessageBox.Show("Bir üründen en fazla 5 adet alabilirsiniz.");

                    string customerName = GetCustomerName(CustomerId);
                    string customerType = GetCustomerType(CustomerId);

                    logManager.AddLog(CustomerId, null, "Hata", customerType, productName, quantity,
                        $"{customerName} ({customerType}) adlı müşterinin siparişi aşırı talep nedeniyle iptal oldu.");
                    return;
                }

                decimal totalPrice = price * quantity;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        // müşteri bakiyesi kontrolü
                        string budgetQuery = "SELECT Budget, CustomerType FROM Customers WHERE CustomerID = @id";
                        SqlCommand budgetCmd = new SqlCommand(budgetQuery, conn);
                        budgetCmd.Parameters.AddWithValue("@id", CustomerId);
                        SqlDataReader reader = budgetCmd.ExecuteReader();

                        if (reader.Read())
                        {
                            decimal budget = (decimal)reader["Budget"];
                            string customerType = reader["CustomerType"].ToString();
                            string customerName = GetCustomerName(CustomerId);

                            if (budget < totalPrice)
                            {
                                MessageBox.Show("Yetersiz bakiye.");

                                logManager.AddLog(CustomerId, null, "Hata", customerType, productName, quantity,
                                    $"{customerName} ({customerType}) adlı müşterinin siparişi yetersiz bakiye nedeniyle iptal oldu.");
                                return;
                            }

                            reader.Close();

                            // sipariş ekle
                            string orderQuery = "INSERT INTO Orders (CustomerID, ProductID, Quantity, TotalPrice, OrderDate, OrderStatus) " +
                                                "OUTPUT INSERTED.OrderID VALUES (@customerId, @productId, @quantity, @totalPrice, GETDATE(), 'Pending')";
                            SqlCommand orderCmd = new SqlCommand(orderQuery, conn);
                            orderCmd.Parameters.AddWithValue("@customerId", CustomerId);
                            orderCmd.Parameters.AddWithValue("@productId", productId);
                            orderCmd.Parameters.AddWithValue("@quantity", quantity);
                            orderCmd.Parameters.AddWithValue("@totalPrice", totalPrice);
                            int orderId = (int)orderCmd.ExecuteScalar();

                            MessageBox.Show("Sipariş başarıyla oluşturuldu, Admin onayına gönderildi.");

                            logManager.AddLog(CustomerId, orderId, "Bilgilendirme", customerType, productName, quantity,
                                $"{customerName} ({customerType}) {productName}'den {quantity} adet sipariş verdi.");

                            // stok sıfırlandıysa log kaydı
                            string checkStockQuery = "SELECT Stock FROM Products WHERE ProductID = @productId";
                            SqlCommand checkStockCmd = new SqlCommand(checkStockQuery, conn);
                            checkStockCmd.Parameters.AddWithValue("@productId", productId);
                            int remainingStock = (int)checkStockCmd.ExecuteScalar();

                            if (remainingStock == 0)
                            {
                                logManager.AddLog(null, null, "Bilgilendirme", "Bilinmiyor", productName, 0,
                                    $"Ürün stoğu tükendi: {productName}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Sipariş sırasında hata oluştu: " + ex.Message);

                        string customerName = GetCustomerName(CustomerId);
                        string customerType = GetCustomerType(CustomerId);

                        logManager.AddLog(CustomerId, null, "Hata", customerType, productName, quantity,
                            $"{customerName} ({customerType}) adlı müşterinin siparişi sırasında bir hata oluştu: {ex.Message}");
                    }
                }
            }
        }

    }
}
