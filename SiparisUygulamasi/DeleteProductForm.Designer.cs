namespace SiparisUygulamasi
{
    partial class DeleteProductForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbDeleteProduct = new System.Windows.Forms.ComboBox();
            this.btnDeleteProduct2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbDeleteProduct
            // 
            this.cmbDeleteProduct.FormattingEnabled = true;
            this.cmbDeleteProduct.Location = new System.Drawing.Point(104, 105);
            this.cmbDeleteProduct.Name = "cmbDeleteProduct";
            this.cmbDeleteProduct.Size = new System.Drawing.Size(186, 28);
            this.cmbDeleteProduct.TabIndex = 0;
            // 
            // btnDeleteProduct2
            // 
            this.btnDeleteProduct2.Location = new System.Drawing.Point(104, 167);
            this.btnDeleteProduct2.Name = "btnDeleteProduct2";
            this.btnDeleteProduct2.Size = new System.Drawing.Size(186, 47);
            this.btnDeleteProduct2.TabIndex = 1;
            this.btnDeleteProduct2.Text = "Ürünü Sil";
            this.btnDeleteProduct2.UseVisualStyleBackColor = true;
            this.btnDeleteProduct2.Click += new System.EventHandler(this.btnDeleteProduct_Click2);
            // 
            // DeleteProductForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 335);
            this.Controls.Add(this.cmbDeleteProduct);
            this.Controls.Add(this.btnDeleteProduct2);
            this.Name = "DeleteProductForm";
            this.Text = "DeleteProductForm";
            this.Load += new System.EventHandler(this.DeleteProductForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbDeleteProduct;
        private System.Windows.Forms.Button btnDeleteProduct2;
    }
}