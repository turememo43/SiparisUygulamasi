namespace SiparisUygulamasi
{
    partial class CustomerOperationsForm
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
            this.cmbCustomers = new System.Windows.Forms.ComboBox();
            this.btnUpgradeToPremium = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbCustomers
            // 
            this.cmbCustomers.FormattingEnabled = true;
            this.cmbCustomers.Location = new System.Drawing.Point(57, 54);
            this.cmbCustomers.Name = "cmbCustomers";
            this.cmbCustomers.Size = new System.Drawing.Size(270, 28);
            this.cmbCustomers.TabIndex = 0;
            this.cmbCustomers.Text = "Müşteri Seçiniz.";
            // 
            // btnUpgradeToPremium
            // 
            this.btnUpgradeToPremium.Location = new System.Drawing.Point(102, 114);
            this.btnUpgradeToPremium.Name = "btnUpgradeToPremium";
            this.btnUpgradeToPremium.Size = new System.Drawing.Size(174, 34);
            this.btnUpgradeToPremium.TabIndex = 1;
            this.btnUpgradeToPremium.Text = "Premium yap";
            this.btnUpgradeToPremium.UseVisualStyleBackColor = true;
            this.btnUpgradeToPremium.Click += new System.EventHandler(this.btnUpgradeToPremium_Click_1);
            // 
            // CustomerOperationsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(394, 218);
            this.Controls.Add(this.btnUpgradeToPremium);
            this.Controls.Add(this.cmbCustomers);
            this.Name = "CustomerOperationsForm";
            this.Text = "CustomerOperationsForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbCustomers;
        private System.Windows.Forms.Button btnUpgradeToPremium;
    }
}