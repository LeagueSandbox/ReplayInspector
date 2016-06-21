namespace PcapDecrypt
{
    /*partial class MainWindow
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
            this.SuspendLayout();
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 343);
            this.Name = "MainWindow";
            this.Text = "MainWindow";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion
    }*/
    partial class MainWindow
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.packetFieldsListBox = new System.Windows.Forms.ListBox();
            this.packetListListBox = new System.Windows.Forms.ListBox();
            this.packetHexRichTextBox = new System.Windows.Forms.RichTextBox();
            this.batchPacketListListBox = new System.Windows.Forms.ListBox();
            this.packetListLabel = new System.Windows.Forms.Label();
            this.batchPacketListLabel = new System.Windows.Forms.Label();
            this.packetFieldsLabel = new System.Windows.Forms.Label();
            this.packetHexLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.filterPacketsCheckBox = new System.Windows.Forms.CheckBox();
            this.searchInBatchPacketsCheckBox = new System.Windows.Forms.CheckBox();
            this.packetHeaderComboBox = new System.Windows.Forms.ComboBox();
            this.packetHeaderLabel = new System.Windows.Forms.Label();
            this.packetASCIITextBox = new System.Windows.Forms.TextBox();
            this.packetASCIILabel = new System.Windows.Forms.Label();
            this.filteringOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.filteringOptionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // packetFieldsListBox
            // 
            this.packetFieldsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.packetFieldsListBox.FormattingEnabled = true;
            this.packetFieldsListBox.Location = new System.Drawing.Point(643, 327);
            this.packetFieldsListBox.Name = "packetFieldsListBox";
            this.packetFieldsListBox.Size = new System.Drawing.Size(373, 355);
            this.packetFieldsListBox.TabIndex = 0;
            this.packetFieldsListBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // packetListListBox
            // 
            this.packetListListBox.FormattingEnabled = true;
            this.packetListListBox.Location = new System.Drawing.Point(12, 25);
            this.packetListListBox.Name = "packetListListBox";
            this.packetListListBox.Size = new System.Drawing.Size(312, 303);
            this.packetListListBox.TabIndex = 1;
            this.packetListListBox.SelectedIndexChanged += new System.EventHandler(this.listBox2_SelectedIndexChanged);
            // 
            // packetHexRichTextBox
            // 
            this.packetHexRichTextBox.Location = new System.Drawing.Point(643, 25);
            this.packetHexRichTextBox.Name = "packetHexRichTextBox";
            this.packetHexRichTextBox.Size = new System.Drawing.Size(373, 162);
            this.packetHexRichTextBox.TabIndex = 4;
            this.packetHexRichTextBox.Text = "";
            // 
            // batchPacketListListBox
            // 
            this.batchPacketListListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.batchPacketListListBox.FormattingEnabled = true;
            this.batchPacketListListBox.Location = new System.Drawing.Point(12, 405);
            this.batchPacketListListBox.Name = "batchPacketListListBox";
            this.batchPacketListListBox.Size = new System.Drawing.Size(312, 264);
            this.batchPacketListListBox.TabIndex = 5;
            this.batchPacketListListBox.SelectedIndexChanged += new System.EventHandler(this.listBox3_SelectedIndexChanged);
            // 
            // packetListLabel
            // 
            this.packetListLabel.AutoSize = true;
            this.packetListLabel.Location = new System.Drawing.Point(9, 9);
            this.packetListLabel.Name = "packetListLabel";
            this.packetListLabel.Size = new System.Drawing.Size(59, 13);
            this.packetListLabel.TabIndex = 7;
            this.packetListLabel.Text = "Packet list:";
            // 
            // batchPacketListLabel
            // 
            this.batchPacketListLabel.AutoSize = true;
            this.batchPacketListLabel.Location = new System.Drawing.Point(9, 389);
            this.batchPacketListLabel.Name = "batchPacketListLabel";
            this.batchPacketListLabel.Size = new System.Drawing.Size(169, 13);
            this.batchPacketListLabel.TabIndex = 8;
            this.batchPacketListLabel.Text = "Packets in selected batch packet:";
            // 
            // packetFieldsLabel
            // 
            this.packetFieldsLabel.AutoSize = true;
            this.packetFieldsLabel.Location = new System.Drawing.Point(640, 311);
            this.packetFieldsLabel.Name = "packetFieldsLabel";
            this.packetFieldsLabel.Size = new System.Drawing.Size(64, 13);
            this.packetFieldsLabel.TabIndex = 9;
            this.packetFieldsLabel.Text = "Packet info:";
            // 
            // packetHexLabel
            // 
            this.packetHexLabel.AutoSize = true;
            this.packetHexLabel.Location = new System.Drawing.Point(640, 9);
            this.packetHexLabel.Name = "packetHexLabel";
            this.packetHexLabel.Size = new System.Drawing.Size(44, 13);
            this.packetHexLabel.TabIndex = 10;
            this.packetHexLabel.Text = "Packet:";
            this.packetHexLabel.Click += new System.EventHandler(this.label4_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 331);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "label5";
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 671);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "label6";
            // 
            // filterPacketsCheckBox
            // 
            this.filterPacketsCheckBox.AutoSize = true;
            this.filterPacketsCheckBox.Location = new System.Drawing.Point(6, 19);
            this.filterPacketsCheckBox.Name = "filterPacketsCheckBox";
            this.filterPacketsCheckBox.Size = new System.Drawing.Size(89, 17);
            this.filterPacketsCheckBox.TabIndex = 13;
            this.filterPacketsCheckBox.Text = "Filter packets";
            this.filterPacketsCheckBox.UseVisualStyleBackColor = true;
            this.filterPacketsCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // searchInBatchPacketsCheckBox
            // 
            this.searchInBatchPacketsCheckBox.AutoSize = true;
            this.searchInBatchPacketsCheckBox.Location = new System.Drawing.Point(6, 42);
            this.searchInBatchPacketsCheckBox.Name = "searchInBatchPacketsCheckBox";
            this.searchInBatchPacketsCheckBox.Size = new System.Drawing.Size(142, 17);
            this.searchInBatchPacketsCheckBox.TabIndex = 14;
            this.searchInBatchPacketsCheckBox.Text = "Search in batch packets";
            this.searchInBatchPacketsCheckBox.UseVisualStyleBackColor = true;
            this.searchInBatchPacketsCheckBox.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // packetHeaderComboBox
            // 
            this.packetHeaderComboBox.FormattingEnabled = true;
            this.packetHeaderComboBox.Location = new System.Drawing.Point(92, 63);
            this.packetHeaderComboBox.Name = "packetHeaderComboBox";
            this.packetHeaderComboBox.Size = new System.Drawing.Size(204, 21);
            this.packetHeaderComboBox.Sorted = true;
            this.packetHeaderComboBox.TabIndex = 15;
            this.packetHeaderComboBox.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // packetHeaderLabel
            // 
            this.packetHeaderLabel.AutoSize = true;
            this.packetHeaderLabel.Location = new System.Drawing.Point(6, 66);
            this.packetHeaderLabel.Name = "packetHeaderLabel";
            this.packetHeaderLabel.Size = new System.Drawing.Size(80, 13);
            this.packetHeaderLabel.TabIndex = 17;
            this.packetHeaderLabel.Text = "Packet header:";
            this.packetHeaderLabel.Click += new System.EventHandler(this.label8_Click);
            // 
            // packetASCIITextBox
            // 
            this.packetASCIITextBox.Location = new System.Drawing.Point(643, 206);
            this.packetASCIITextBox.Multiline = true;
            this.packetASCIITextBox.Name = "packetASCIITextBox";
            this.packetASCIITextBox.Size = new System.Drawing.Size(373, 102);
            this.packetASCIITextBox.TabIndex = 18;
            // 
            // packetASCIILabel
            // 
            this.packetASCIILabel.AutoSize = true;
            this.packetASCIILabel.Location = new System.Drawing.Point(640, 190);
            this.packetASCIILabel.Name = "packetASCIILabel";
            this.packetASCIILabel.Size = new System.Drawing.Size(37, 13);
            this.packetASCIILabel.TabIndex = 19;
            this.packetASCIILabel.Text = "ASCII:";
            // 
            // filteringOptionsGroupBox
            // 
            this.filteringOptionsGroupBox.Controls.Add(this.filterPacketsCheckBox);
            this.filteringOptionsGroupBox.Controls.Add(this.searchInBatchPacketsCheckBox);
            this.filteringOptionsGroupBox.Controls.Add(this.packetHeaderLabel);
            this.filteringOptionsGroupBox.Controls.Add(this.packetHeaderComboBox);
            this.filteringOptionsGroupBox.Location = new System.Drawing.Point(330, 25);
            this.filteringOptionsGroupBox.Name = "filteringOptionsGroupBox";
            this.filteringOptionsGroupBox.Size = new System.Drawing.Size(307, 95);
            this.filteringOptionsGroupBox.TabIndex = 20;
            this.filteringOptionsGroupBox.TabStop = false;
            this.filteringOptionsGroupBox.Text = "Filtering options:";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 693);
            this.Controls.Add(this.filteringOptionsGroupBox);
            this.Controls.Add(this.packetASCIILabel);
            this.Controls.Add(this.packetASCIITextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.packetHexLabel);
            this.Controls.Add(this.packetFieldsLabel);
            this.Controls.Add(this.batchPacketListLabel);
            this.Controls.Add(this.packetListLabel);
            this.Controls.Add(this.batchPacketListListBox);
            this.Controls.Add(this.packetHexRichTextBox);
            this.Controls.Add(this.packetListListBox);
            this.Controls.Add(this.packetFieldsListBox);
            this.Name = "MainWindow";
            this.Text = "Packet analyzer";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.filteringOptionsGroupBox.ResumeLayout(false);
            this.filteringOptionsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox packetFieldsListBox;
        private System.Windows.Forms.ListBox packetListListBox;
        private System.Windows.Forms.RichTextBox packetHexRichTextBox;
        private System.Windows.Forms.ListBox batchPacketListListBox;
        private System.Windows.Forms.Label packetListLabel;
        private System.Windows.Forms.Label batchPacketListLabel;
        private System.Windows.Forms.Label packetFieldsLabel;
        private System.Windows.Forms.Label packetHexLabel;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox filterPacketsCheckBox;
        private System.Windows.Forms.CheckBox searchInBatchPacketsCheckBox;
        private System.Windows.Forms.ComboBox packetHeaderComboBox;
        private System.Windows.Forms.Label packetHeaderLabel;
        private System.Windows.Forms.TextBox packetASCIITextBox;
        private System.Windows.Forms.Label packetASCIILabel;
        private System.Windows.Forms.GroupBox filteringOptionsGroupBox;
    }
}