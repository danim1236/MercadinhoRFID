namespace MercadinhoRFID
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dualTagObjectBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ForaHa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IncoerenciaStatus = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.hasLostDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.PerdidoHa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.HasRemocao = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.RemocaoHa = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dualTagObjectBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(3, 32);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(3, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(539, 186);
            this.listBox1.TabIndex = 2;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.statusDataGridViewTextBoxColumn,
            this.ForaHa,
            this.IncoerenciaStatus,
            this.hasLostDataGridViewCheckBoxColumn,
            this.PerdidoHa,
            this.HasRemocao,
            this.RemocaoHa});
            this.dataGridView1.DataSource = this.dualTagObjectBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(4, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(750, 155);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridView1_RowPrePaint);
            // 
            // dualTagObjectBindingSource
            // 
            this.dualTagObjectBindingSource.DataSource = typeof(MercadinhoRFID.Driver.DualTagObject);
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Location = new System.Drawing.Point(3, 3);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.Size = new System.Drawing.Size(191, 189);
            this.dataGridView2.TabIndex = 4;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.dataGridView1);
            this.panel1.Location = new System.Drawing.Point(95, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(761, 165);
            this.panel1.TabIndex = 5;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.listBox1);
            this.panel2.Location = new System.Drawing.Point(95, 184);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(549, 199);
            this.panel2.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.dataGridView2);
            this.panel3.Location = new System.Drawing.Point(650, 184);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(201, 199);
            this.panel3.TabIndex = 7;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.button1);
            this.panel4.Controls.Add(this.button2);
            this.panel4.Location = new System.Drawing.Point(6, 13);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(83, 62);
            this.panel4.TabIndex = 8;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Identificação";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            this.idDataGridViewTextBoxColumn.Width = 80;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            this.statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            this.statusDataGridViewTextBoxColumn.HeaderText = "Estado";
            this.statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            this.statusDataGridViewTextBoxColumn.ReadOnly = true;
            this.statusDataGridViewTextBoxColumn.Width = 80;
            // 
            // ForaHa
            // 
            this.ForaHa.DataPropertyName = "ForaHa";
            this.ForaHa.HeaderText = "Tempo Fora";
            this.ForaHa.Name = "ForaHa";
            this.ForaHa.ReadOnly = true;
            // 
            // IncoerenciaStatus
            // 
            this.IncoerenciaStatus.DataPropertyName = "IncoerenciaStatus";
            this.IncoerenciaStatus.HeaderText = "Incoerência";
            this.IncoerenciaStatus.Name = "IncoerenciaStatus";
            this.IncoerenciaStatus.ReadOnly = true;
            this.IncoerenciaStatus.Width = 70;
            // 
            // hasLostDataGridViewCheckBoxColumn
            // 
            this.hasLostDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.hasLostDataGridViewCheckBoxColumn.DataPropertyName = "IsLost";
            this.hasLostDataGridViewCheckBoxColumn.HeaderText = "Perdido";
            this.hasLostDataGridViewCheckBoxColumn.Name = "hasLostDataGridViewCheckBoxColumn";
            this.hasLostDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // PerdidoHa
            // 
            this.PerdidoHa.DataPropertyName = "PerdidoHa";
            this.PerdidoHa.HeaderText = "Tempo Perdido";
            this.PerdidoHa.Name = "PerdidoHa";
            this.PerdidoHa.ReadOnly = true;
            this.PerdidoHa.Width = 110;
            // 
            // HasRemocao
            // 
            this.HasRemocao.DataPropertyName = "HasRemocao";
            this.HasRemocao.HeaderText = "Etiqueta Removida";
            this.HasRemocao.Name = "HasRemocao";
            this.HasRemocao.ReadOnly = true;
            this.HasRemocao.Width = 110;
            // 
            // RemocaoHa
            // 
            this.RemocaoHa.DataPropertyName = "RemocaoHa";
            this.RemocaoHa.HeaderText = "Tempo de Remoçao";
            this.RemocaoHa.Name = "RemocaoHa";
            this.RemocaoHa.ReadOnly = true;
            this.RemocaoHa.Width = 130;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(864, 391);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "RFID - Segurança";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dualTagObjectBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource dualTagObjectBindingSource;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ForaHa;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IncoerenciaStatus;
        private System.Windows.Forms.DataGridViewCheckBoxColumn hasLostDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PerdidoHa;
        private System.Windows.Forms.DataGridViewCheckBoxColumn HasRemocao;
        private System.Windows.Forms.DataGridViewTextBoxColumn RemocaoHa;
    }
}

