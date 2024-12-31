namespace SiparisUygulamasi
{
    partial class AdminForm
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
            this.components = new System.ComponentModel.Container();
            this.dgvPendingOrders = new System.Windows.Forms.DataGridView();
            this.btnApproveOrders = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnProductOperations = new System.Windows.Forms.Button();
            this.dgvInProgressOrders = new System.Windows.Forms.DataGridView();
            this.dgvApprovedOrders = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvLogs = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCustomerOperations = new System.Windows.Forms.Button();
            this.pictureBoxLoading = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendingOrders)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInProgressOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvApprovedOrders)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvPendingOrders
            // 
            this.dgvPendingOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPendingOrders.Location = new System.Drawing.Point(318, 307);
            this.dgvPendingOrders.Name = "dgvPendingOrders";
            this.dgvPendingOrders.RowHeadersWidth = 62;
            this.dgvPendingOrders.RowTemplate.Height = 28;
            this.dgvPendingOrders.Size = new System.Drawing.Size(1426, 508);
            this.dgvPendingOrders.TabIndex = 5;
            // 
            // btnApproveOrders
            // 
            this.btnApproveOrders.Location = new System.Drawing.Point(61, 48);
            this.btnApproveOrders.Name = "btnApproveOrders";
            this.btnApproveOrders.Size = new System.Drawing.Size(179, 41);
            this.btnApproveOrders.TabIndex = 6;
            this.btnApproveOrders.Text = "Siparişleri Onayla";
            this.btnApproveOrders.UseVisualStyleBackColor = true;
            this.btnApproveOrders.Click += new System.EventHandler(this.btnApproveOrders_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnApproveOrders);
            this.groupBox1.Location = new System.Drawing.Point(12, 84);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 136);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Onaylama İşlemleri";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            // 
            // btnProductOperations
            // 
            this.btnProductOperations.Location = new System.Drawing.Point(12, 12);
            this.btnProductOperations.Name = "btnProductOperations";
            this.btnProductOperations.Size = new System.Drawing.Size(155, 41);
            this.btnProductOperations.TabIndex = 8;
            this.btnProductOperations.Text = "Ürün işlemleri";
            this.btnProductOperations.UseVisualStyleBackColor = true;
            this.btnProductOperations.Click += new System.EventHandler(this.btnProductOperations_Click_1);
            // 
            // dgvInProgressOrders
            // 
            this.dgvInProgressOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvInProgressOrders.Location = new System.Drawing.Point(318, 257);
            this.dgvInProgressOrders.Name = "dgvInProgressOrders";
            this.dgvInProgressOrders.RowHeadersWidth = 62;
            this.dgvInProgressOrders.RowTemplate.Height = 28;
            this.dgvInProgressOrders.Size = new System.Drawing.Size(1426, 44);
            this.dgvInProgressOrders.TabIndex = 9;
            // 
            // dgvApprovedOrders
            // 
            this.dgvApprovedOrders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvApprovedOrders.Location = new System.Drawing.Point(318, 93);
            this.dgvApprovedOrders.Name = "dgvApprovedOrders";
            this.dgvApprovedOrders.RowHeadersWidth = 62;
            this.dgvApprovedOrders.RowTemplate.Height = 28;
            this.dgvApprovedOrders.Size = new System.Drawing.Size(1426, 158);
            this.dgvApprovedOrders.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.Location = new System.Drawing.Point(901, 853);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 37);
            this.label1.TabIndex = 11;
            this.label1.Text = "LOG PANELİ";
            // 
            // dgvLogs
            // 
            this.dgvLogs.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLogs.Location = new System.Drawing.Point(318, 893);
            this.dgvLogs.Name = "dgvLogs";
            this.dgvLogs.RowHeadersWidth = 62;
            this.dgvLogs.RowTemplate.Height = 28;
            this.dgvLogs.Size = new System.Drawing.Size(1426, 470);
            this.dgvLogs.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.Location = new System.Drawing.Point(862, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(277, 37);
            this.label2.TabIndex = 13;
            this.label2.Text = "SİPARİŞ PANELİ";
            // 
            // btnCustomerOperations
            // 
            this.btnCustomerOperations.Location = new System.Drawing.Point(213, 12);
            this.btnCustomerOperations.Name = "btnCustomerOperations";
            this.btnCustomerOperations.Size = new System.Drawing.Size(155, 40);
            this.btnCustomerOperations.TabIndex = 14;
            this.btnCustomerOperations.Text = "Müşteri işlemleri";
            this.btnCustomerOperations.UseVisualStyleBackColor = true;
            this.btnCustomerOperations.Click += new System.EventHandler(this.btnCustomerOperations_Click);
            // 
            // pictureBoxLoading
            // 
            this.pictureBoxLoading.Image = global::SiparisUygulamasi.Properties.Resources.loading;
            this.pictureBoxLoading.Location = new System.Drawing.Point(1703, 261);
            this.pictureBoxLoading.Name = "pictureBoxLoading";
            this.pictureBoxLoading.Size = new System.Drawing.Size(36, 36);
            this.pictureBoxLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxLoading.TabIndex = 15;
            this.pictureBoxLoading.TabStop = false;
            this.pictureBoxLoading.Visible = false;
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(2350, 1479);
            this.Controls.Add(this.pictureBoxLoading);
            this.Controls.Add(this.btnCustomerOperations);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dgvLogs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dgvApprovedOrders);
            this.Controls.Add(this.dgvInProgressOrders);
            this.Controls.Add(this.btnProductOperations);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgvPendingOrders);
            this.Name = "AdminForm";
            this.Text = "AdminForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgvPendingOrders)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInProgressOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvApprovedOrders)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvPendingOrders;
        private System.Windows.Forms.Button btnApproveOrders;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnProductOperations;
        private System.Windows.Forms.DataGridView dgvInProgressOrders;
        private System.Windows.Forms.DataGridView dgvApprovedOrders;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dgvLogs;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCustomerOperations;
        private System.Windows.Forms.PictureBox pictureBoxLoading;
    }
}