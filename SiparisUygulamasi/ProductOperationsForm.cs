using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SiparisUygulamasi
{
    public partial class ProductOperationsForm : Form
    {
        private string connectionString = @"Server=PARZIVAL;Database=OrderAndStockDB;Trusted_Connection=True;";

        public ProductOperationsForm()
        {
            InitializeComponent();
            LoadProducts();
            dgvProducts.CellFormatting += dgvProducts_CellFormatting;
            ConfigureDataGridView();
            LoadStockChart();
        }

        private void btnAddProduct_Click_1(object sender, EventArgs e)
        {
            AddProductForm addProductForm = new AddProductForm();
            addProductForm.ShowDialog();
            LoadProducts(); 
        }

        private void btnDeleteProduct_Click_1(object sender, EventArgs e)
        {
            DeleteProductForm deleteProductForm = new DeleteProductForm();
            deleteProductForm.ShowDialog(); 
            LoadProducts(); 
        }
        private void btnUpdateProduct_Click_1(object sender, EventArgs e)
        {
            UpdateProductForm updateProductForm = new UpdateProductForm();
            updateProductForm.ShowDialog(); 
            LoadProducts(); 

        }


        private void LoadProducts()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Products"; 
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    
                    dgvProducts.DataSource = null; 
                    dgvProducts.DataSource = dataTable; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ürünler yüklenirken bir hata oluştu: " + ex.Message);
                }
            }
        }

        private void dgvProducts_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvProducts.Columns[e.ColumnIndex].Name == "Stock")  
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

        private void ConfigureDataGridView()
        {
            dgvProducts.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvProducts.RowHeadersVisible = false;
            dgvProducts.AutoGenerateColumns = false;
        }


        private void LoadStockChart()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT ProductName, Stock FROM Products";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable stockData = new DataTable();
                    adapter.Fill(stockData);

                    // chart ayarları
                    chartStock.Series.Clear();
                    chartStock.Titles.Clear();
                    chartStock.Titles.Add("Ürün Stok Durumu");
                    chartStock.ChartAreas[0].AxisX.Title = "Ürünler";
                    chartStock.ChartAreas[0].AxisY.Title = "Stok Miktarı";
                    chartStock.ChartAreas[0].AxisX.Interval = 1;
                    chartStock.ChartAreas[0].AxisX.MajorGrid.LineWidth = 0;
                    chartStock.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;

                    Series series = new Series
                    {
                        ChartType = SeriesChartType.Column,
                        IsValueShownAsLabel = true
                    };

                    foreach (DataRow row in stockData.Rows)
                    {
                        string productName = row["ProductName"].ToString();
                        int stock = Convert.ToInt32(row["Stock"]);

                        int criticalThreshold = 10;
                        int lowThreshold = 50;

                        DataPoint point = new DataPoint
                        {
                            AxisLabel = productName,
                            YValues = new double[] { stock },
                        };

                       
                        if (stock <= criticalThreshold)
                        {
                            point.Color = Color.Red;
                        }
                        else if (stock <= lowThreshold)
                        {
                            point.Color = Color.Yellow;
                        }
                        else
                        {
                            point.Color = Color.Green;
                        }

                        series.Points.Add(point);
                    }

                    chartStock.Series.Add(series);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Stok grafiği yüklenirken bir hata oluştu: " + ex.Message);
            }
        }

        private void btnRefreshChart_Click(object sender, EventArgs e)
        {
            LoadStockChart(); // grafiği yeniden yükle
        }
    }
}
