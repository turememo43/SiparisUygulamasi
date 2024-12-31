using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiparisUygulamasi
{
    public class LogManager
    {
        private readonly string connectionString;

        public LogManager(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddLog(int? customerId, int? orderId, string logType, string customerType,
                           string productName, int? quantity, string logDetails)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Logs (CustomerID, OrderID, LogDate, LogType, CustomerType, ProductName, Quantity, LogDetails) " +
                               "VALUES (@CustomerID, @OrderID, GETDATE(), @LogType, @CustomerType, @ProductName, @Quantity, @LogDetails)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CustomerID", customerId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OrderID", orderId ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@LogType", logType);
                command.Parameters.AddWithValue("@CustomerType", customerType);
                command.Parameters.AddWithValue("@ProductName", productName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Quantity", quantity ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@LogDetails", logDetails);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void AddAdminLog(
        int? customerId,
        int? orderId,
        string logType,
        string customerType,
        string productName,
        int? quantity,
        string logDetails)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                INSERT INTO Logs (CustomerID, OrderID, LogDate, LogType, CustomerType, ProductName, Quantity, LogDetails) 
                VALUES (@customerId, @orderId, GETDATE(), @logType, @customerType, @productName, @quantity, @logDetails)";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@customerId", (object)customerId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@orderId", (object)orderId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@logType", logType);
                    cmd.Parameters.AddWithValue("@customerType", customerType);
                    cmd.Parameters.AddWithValue("@productName", (object)productName ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@quantity", (object)quantity ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@logDetails", logDetails);

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Log kaydı sırasında bir hata oluştu: " + ex.Message);
                }
            }
        }


    }


}
