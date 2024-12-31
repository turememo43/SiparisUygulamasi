using System;
using System.Data;
using System.Data.SqlClient;

namespace SiparisUygulamasi
{
    public class OrderManager
    {
        private string connectionString = @"Server=PARZIVAL;Database=OrderAndStockDB;Trusted_Connection=True;";

        // öncelik skorlarını güncelleme
        public void UpdatePriorityScores()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // pending durumundaki tüm siparişleri al
                    string query = "SELECT OrderID, CustomerID, OrderDate FROM Orders WHERE OrderStatus = 'Pending'";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    DataTable ordersTable = new DataTable();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(ordersTable); // tüm siparişler datatable'a
                    }

                    foreach (DataRow row in ordersTable.Rows)
                    {
                        int orderId = (int)row["OrderID"];
                        int customerId = (int)row["CustomerID"];
                        DateTime orderDate = (DateTime)row["OrderDate"];

                        // bekleme süresini hesapla
                        double waitingTimeInSeconds = (DateTime.Now - orderDate).TotalSeconds;

                        // müşteri türüne göre temel öncelik skoru belirle
                        string customerType = GetCustomerType(customerId);
                        double basePriorityScore = customerType == "Premium" ? 35 : 10;

                        // dinamik öncelik skoru hesapla
                        double priorityScore = basePriorityScore + (waitingTimeInSeconds * 0.5);

                        // siparişin PriorityScore ve WaitingTime değerlerini güncelle
                        string updateQuery = "UPDATE Orders SET PriorityScore = @priorityScore, WaitingTime = @waitingTime WHERE OrderID = @orderId";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@priorityScore", priorityScore);
                            updateCmd.Parameters.AddWithValue("@waitingTime", waitingTimeInSeconds);
                            updateCmd.Parameters.AddWithValue("@orderId", orderId);
                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Öncelik skorları güncellenirken hata oluştu: " + ex.Message);
                }
            }
        }

        
        private string GetCustomerType(int customerId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT CustomerType FROM Customers WHERE CustomerID = @customerId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@customerId", customerId);

                    object result = cmd.ExecuteScalar();
                    return result != null ? result.ToString() : "Standard";
                }
            }
        }
    }
}
