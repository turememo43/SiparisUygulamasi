using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiparisUygulamasi
{

    public partial class AdminForm : Form
    {
        private bool isUpdating = false; // işlem kontrolü için flag
        public bool isOrderApprovalInProgress = false; // sipariş onaylama işlemi kontrolü
      

        private ManualResetEvent pauseEvent = new ManualResetEvent(true); // başlangıçta çalışır
        private ManualResetEvent orderApprovalComplete = new ManualResetEvent(false); // başlangıçta çalışmaz

        private string connectionString = @"Server=PARZIVAL;Database=OrderAndStockDB;Trusted_Connection=True;";
        public List<int> pendingCustomerUpdates = new List<int>();


        private OrderManager orderManager;
        private LogManager logManager; 
        private OrderLoader OrderLoader;
        private Form1 mainForm;

        public AdminForm(Form1 form)
        {
            InitializeComponent();

            orderManager = new OrderManager();
            logManager = new LogManager(connectionString); 
            OrderLoader = new OrderLoader(connectionString);
            mainForm = form;

            LoadAllOrders(); 
            LoadLogs();

            pictureBoxLoading.Image = Properties.Resources.loading;

            timer1.Interval = 3000; // 3 saniye
            timer1.Tick += Timer1_Tick; // tick olayını bağlama
            timer1.Start(); // timer'ı başlat
        }

       
        private void LoadAllOrders()
        {
            OrderLoader.LoadApprovedOrders(dgvApprovedOrders);
            OrderLoader.LoadInProgressOrders(dgvInProgressOrders);
            OrderLoader.LoadPendingOrders(dgvPendingOrders);
        }

        private void LoadLogs()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT LogDetails FROM Logs ORDER BY LogID DESC"; 
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable logTable = new DataTable();
                    adapter.Fill(logTable);

                    dgvLogs.DataSource = null; 
                    dgvLogs.DataSource = logTable; 

                    
                    dgvLogs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    
                    foreach (DataGridViewColumn column in dgvLogs.Columns)
                    {
                        if (column.Name != "LogDetails")
                        {
                            column.Visible = false; // LogDetails dışında tüm kolonları gizle
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Loglar yüklenirken hata oluştu: " + ex.Message);
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (isUpdating) return; // başka bir işlem devam ediyorsa Timer çıkış yapar
            isUpdating = true;

            try
            {
                orderManager.UpdatePriorityScores(); 
                LoadAllOrders(); 
                LoadLogs();
                mainForm.LoadCustomers2();

                CheckOrderTimeout(); // süre aşımı kontrolü

                // in progress siparişlerinin kontrolü
                bool inProgressExists = CheckInProgressOrders();

                // işlemde sipariş varsa GIF'i göster
                pictureBoxLoading.Visible = inProgressExists;

            }
            catch (Exception ex)
            {
                timer1.Stop(); 
                MessageBox.Show("Dinamik öncelik güncellemesi sırasında hata oluştu: " + ex.Message);
            }
            finally
            {
                isUpdating = false; 
            }
        }

        private bool CheckInProgressOrders()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM Orders WHERE OrderStatus = 'In Progress'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0; // işlemde sipariş var mı?
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Sipariş durumu kontrol edilirken hata oluştu: " + ex.Message);
                    return false;
                }
            }
        }

        private void CheckOrderTimeout()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // süre aşımı kontrolü ve iptal edilecek siparişlerin bilgilerini al
                    string timeoutQuery = @"
                SELECT o.OrderID, o.CustomerID, o.ProductID, o.Quantity, c.CustomerName, p.ProductName
                FROM Orders o
                INNER JOIN Customers c ON o.CustomerID = c.CustomerID
                INNER JOIN Products p ON o.ProductID = p.ProductID
                WHERE o.OrderStatus = 'Pending'
                AND DATEDIFF(SECOND, o.OrderDate, GETDATE()) > 300;
            ";

                    SqlCommand timeoutCmd = new SqlCommand(timeoutQuery, conn);
                    SqlDataReader reader = timeoutCmd.ExecuteReader();

                    List<(int OrderID, string CustomerName, int Quantity, string ProductName)> timedOutOrders = new List<(int, string, int, string)>();

                    while (reader.Read())
                    {
                        int orderId = reader.GetInt32(reader.GetOrdinal("OrderID"));
                        string customerName = reader.GetString(reader.GetOrdinal("CustomerName"));
                        int quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                        string productName = reader.GetString(reader.GetOrdinal("ProductName"));

                        timedOutOrders.Add((orderId, customerName, quantity, productName));
                    }


                    reader.Close();

                    // süre aşımı olan siparişleri iptal et
                    foreach (var order in timedOutOrders)
                    {
                        string cancelQuery = "UPDATE Orders SET OrderStatus = 'Cancelled' WHERE OrderID = @orderId";
                        SqlCommand cancelCmd = new SqlCommand(cancelQuery, conn);
                        cancelCmd.Parameters.AddWithValue("@orderId", order.OrderID);
                        cancelCmd.ExecuteNonQuery();

                        // loglama
                        logManager.AddLog(
                            null, order.OrderID, "Uyarı", "Sistem", order.ProductName, order.Quantity,
                            $"{order.CustomerName} adlı müşterinin {order.Quantity} adet {order.ProductName} siparişi süre aşımı nedeniyle iptal edildi."

                        );
                    }

                    // güncellemeler sonrası dgvleri ve logları yeniden yükle
                    if (timedOutOrders.Count > 0)
                    {
                        Invoke(new Action(() =>
                        {
                            LoadAllOrders(); 
                            LoadLogs(); 
                        }));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Süre aşımı kontrolü sırasında hata oluştu: " + ex.Message);
            }
        }


        private void btnApproveOrders_Click(object sender, EventArgs e)
        {
            Thread orderThread = new Thread(() =>
            {
                ApproveOrders(); // sipariş onaylama işlemini başlat

                // sipariş onaylama tamamlanana kadar bekle
                orderApprovalComplete.WaitOne();
            });

            orderThread.IsBackground = true; // uygulama kapandığında thread otomatik kapanır
            orderThread.Start();
        }


        private void btnCustomerOperations_Click(object sender, EventArgs e)
        {
            CustomerOperationsForm customerOperationsForm = new CustomerOperationsForm(connectionString, logManager, this);
            customerOperationsForm.ShowDialog();
            LoadAllOrders();
        }


        private void btnProductOperations_Click_1(object sender, EventArgs e)
        {
            // sipariş onaylama işlemini durdur
            pauseEvent.Reset();

            // ürün İşlemleri Formunu aç
            ProductOperationsForm productOperationsForm = new ProductOperationsForm();
            productOperationsForm.FormClosed += (s, args) =>
            {
                // ürün işlemleri formu kapatıldığında sipariş onaylama işlemini devam ettir
                pauseEvent.Set();
            };
            productOperationsForm.Show();
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

                    // ExecuteScalar: tek bir değer döndürür (CustomerName)
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

        private void ApproveOrders()
        {
            isOrderApprovalInProgress = true; // sipariş onaylama işlemi başladı
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT OrderID, ProductID, Quantity, CustomerID, TotalPrice FROM Orders WHERE OrderStatus = 'Pending' ORDER BY PriorityScore DESC";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    DataTable ordersTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ordersTable);
                    }

                    if (ordersTable.Rows.Count > 0)
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show("Sipariş onaylama işlemi başlatıldı!");

                            // log kaydı ekle
                            logManager.AddLog(null, null, "Bilgilendirme", "Admin", null, 0,
                                "Admin sipariş onaylama işlemini başlattı.");
                        }));

                        foreach (DataRow row in ordersTable.Rows)
                        {
                            // pause durumunu kontrol et
                            pauseEvent.WaitOne();

                            int orderId = (int)row["OrderID"];
                            int productId = (int)row["ProductID"];
                            int quantity = (int)row["Quantity"];
                            int customerId = (int)row["CustomerID"];
                            decimal totalPrice = (decimal)row["TotalPrice"];

                            string customerName = GetCustomerName(customerId);
                            string customerType = GetCustomerType(customerId);

                            // ürün adı al
                            string productNameQuery = "SELECT ProductName FROM Products WHERE ProductID = @productId";
                            SqlCommand productNameCmd = new SqlCommand(productNameQuery, conn);
                            productNameCmd.Parameters.AddWithValue("@productId", productId);
                            string productName = (string)productNameCmd.ExecuteScalar();


                            // in progress güncellemesi
                            string updateOrderToInProgressQuery = "UPDATE Orders SET OrderStatus = 'In Progress' WHERE OrderID = @orderId";
                            SqlCommand updateOrderToInProgressCmd = new SqlCommand(updateOrderToInProgressQuery, conn);
                            updateOrderToInProgressCmd.Parameters.AddWithValue("@orderId", orderId);
                            updateOrderToInProgressCmd.ExecuteNonQuery();


                            // sipariş işleniyor için log kaydı
                            logManager.AddLog(customerId, orderId, "Bilgilendirme", customerType, productName, quantity,
                                $"{customerName} ({customerType}) adlı müşterinin {productName} siparişi işleniyor.");

                            // Her sipariş için işlem gecikmesi
                            Thread.Sleep(3000); 

                            Invoke(new Action(() =>
                            {
                                LoadAllOrders(); // dgvyi yükle
                                LoadLogs(); // logları yükle
                            }));

                            // stok kontrolü
                            string stockQuery = "SELECT Stock FROM Products WHERE ProductID = @productId";
                            SqlCommand stockCmd = new SqlCommand(stockQuery, conn);
                            stockCmd.Parameters.AddWithValue("@productId", productId);

                            int currentStock = (int)stockCmd.ExecuteScalar();

                            if (currentStock < quantity)
                            {
                                // stok yetersizse siparişi iptal et
                                string cancelOrderQuery = "UPDATE Orders SET OrderStatus = 'Cancelled' WHERE OrderID = @orderId";
                                SqlCommand cancelOrderCmd = new SqlCommand(cancelOrderQuery, conn);
                                cancelOrderCmd.Parameters.AddWithValue("@orderId", orderId);
                                cancelOrderCmd.ExecuteNonQuery();

                                // ürün adı al
                                string getProductNameQuery = "SELECT ProductName FROM Products WHERE ProductID = @productId";
                                SqlCommand getProductNameCmd = new SqlCommand(getProductNameQuery, conn);
                                getProductNameCmd.Parameters.AddWithValue("@productId", productId);
                                string productNameResult = (string)getProductNameCmd.ExecuteScalar();

                                // müşteri adını ve tipini al
                                string customerNameResult = GetCustomerName(customerId);
                                string customerTypeResult = GetCustomerType(customerId);

                                // Log kaydı ekle
                                logManager.AddLog(
                                    customerId, orderId, "Uyarı", customerTypeResult, productNameResult, quantity,
                                    $"{customerNameResult} ({customerTypeResult}) adlı müşterinin siparişi stok yetersizliği nedeniyle iptal edildi."
                                );

                                continue; 
                            }


                            // stok güncelle
                            string updateStockQuery = "UPDATE Products SET Stock = Stock - @quantity WHERE ProductID = @productId";
                            SqlCommand updateStockCmd = new SqlCommand(updateStockQuery, conn);
                            updateStockCmd.Parameters.AddWithValue("@quantity", quantity);
                            updateStockCmd.Parameters.AddWithValue("@productId", productId);
                            updateStockCmd.ExecuteNonQuery();

                            // müşteri bakiyesi güncelle
                            string updateBudgetQuery = "UPDATE Customers SET Budget = Budget - @totalPrice, TotalSpent = TotalSpent + @totalPrice WHERE CustomerID = @customerId";
                            SqlCommand updateBudgetCmd = new SqlCommand(updateBudgetQuery, conn);
                            updateBudgetCmd.Parameters.AddWithValue("@totalPrice", totalPrice);
                            updateBudgetCmd.Parameters.AddWithValue("@customerId", customerId);
                            updateBudgetCmd.ExecuteNonQuery();


                            // sipariş durumunu approved olarak güncelle
                            string updateOrderStatusQuery = "UPDATE Orders SET OrderStatus = 'Approved' WHERE OrderID = @orderId";
                            SqlCommand updateOrderStatusCmd = new SqlCommand(updateOrderStatusQuery, conn);
                            updateOrderStatusCmd.Parameters.AddWithValue("@orderId", orderId);
                            updateOrderStatusCmd.ExecuteNonQuery();

                            // log kaydı sipariş onaylandı
                            logManager.AddLog(customerId, orderId, "Bilgilendirme", customerType, productName, quantity,
                                $"{customerName} ({customerType}) adlı müşterinin {productName} siparişi onaylandı.");
                        }

                        Invoke(new Action(() =>
                        {
                            MessageBox.Show("Tüm siparişler başarıyla onaylandı!");

                            CustomerOperationsForm customerForm = new CustomerOperationsForm(connectionString, logManager, this);
                            customerForm.ProcessPendingCustomerUpdates(); // müşteri premium güncellemesi
                        }));
                    }
                    else
                    {
                        Invoke(new Action(() =>
                        {
                            MessageBox.Show("Onaylanacak sipariş bulunamadı.");
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        MessageBox.Show("Sipariş onaylama sırasında hata oluştu: " + ex.Message);
                    }));
                }
            }
        }
    }
}
