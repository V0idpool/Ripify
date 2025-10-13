namespace Ripify
{
    partial class Settings
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings));
            clientID = new TextBox();
            label1 = new Label();
            label2 = new Label();
            groupBox1 = new GroupBox();
            clientSecret = new TextBox();
            saveSettingsBtn = new Button();
            groupBox2 = new GroupBox();
            concurrentDownloads = new NumericUpDown();
            browseFolder = new Button();
            downloadPath = new TextBox();
            label3 = new Label();
            label4 = new Label();
            downloadFolderDlg = new FolderBrowserDialog();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)concurrentDownloads).BeginInit();
            SuspendLayout();
            // 
            // clientID
            // 
            clientID.Location = new Point(97, 32);
            clientID.Name = "clientID";
            clientID.Size = new Size(354, 25);
            clientID.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(8, 35);
            label1.Name = "label1";
            label1.Size = new Size(59, 17);
            label1.TabIndex = 5;
            label1.Text = "Client ID:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(8, 64);
            label2.Name = "label2";
            label2.Size = new Size(83, 17);
            label2.TabIndex = 6;
            label2.Text = "Client Secret:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(clientSecret);
            groupBox1.Controls.Add(clientID);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.FromArgb(192, 0, 0);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(457, 97);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Spotify API Details";
            // 
            // clientSecret
            // 
            clientSecret.Location = new Point(97, 61);
            clientSecret.Name = "clientSecret";
            clientSecret.Size = new Size(354, 25);
            clientSecret.TabIndex = 7;
            // 
            // saveSettingsBtn
            // 
            saveSettingsBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            saveSettingsBtn.ForeColor = Color.Black;
            saveSettingsBtn.Location = new Point(340, 79);
            saveSettingsBtn.Name = "saveSettingsBtn";
            saveSettingsBtn.Size = new Size(111, 25);
            saveSettingsBtn.TabIndex = 8;
            saveSettingsBtn.Text = "Save Settings...";
            saveSettingsBtn.UseVisualStyleBackColor = true;
            saveSettingsBtn.Click += saveSettingsBtn_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(concurrentDownloads);
            groupBox2.Controls.Add(saveSettingsBtn);
            groupBox2.Controls.Add(browseFolder);
            groupBox2.Controls.Add(downloadPath);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label4);
            groupBox2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox2.ForeColor = Color.FromArgb(192, 0, 0);
            groupBox2.Location = new Point(12, 115);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(457, 114);
            groupBox2.TabIndex = 9;
            groupBox2.TabStop = false;
            groupBox2.Text = "Download Settings";
            // 
            // concurrentDownloads
            // 
            concurrentDownloads.Location = new Point(155, 62);
            concurrentDownloads.Maximum = new decimal(new int[] { 10, 0, 0, 0 });
            concurrentDownloads.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            concurrentDownloads.Name = "concurrentDownloads";
            concurrentDownloads.Size = new Size(39, 25);
            concurrentDownloads.TabIndex = 11;
            concurrentDownloads.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // browseFolder
            // 
            browseFolder.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            browseFolder.ForeColor = Color.Black;
            browseFolder.Location = new Point(421, 32);
            browseFolder.Name = "browseFolder";
            browseFolder.Size = new Size(30, 25);
            browseFolder.TabIndex = 10;
            browseFolder.Text = "...";
            browseFolder.UseVisualStyleBackColor = true;
            browseFolder.Click += browseFolder_Click;
            // 
            // downloadPath
            // 
            downloadPath.Location = new Point(155, 32);
            downloadPath.Name = "downloadPath";
            downloadPath.Size = new Size(260, 25);
            downloadPath.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.75F);
            label3.ForeColor = Color.Red;
            label3.Location = new Point(8, 67);
            label3.Name = "label3";
            label3.Size = new Size(141, 17);
            label3.TabIndex = 6;
            label3.Text = "Concurrent Downloads";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9.75F);
            label4.ForeColor = Color.Red;
            label4.Location = new Point(8, 35);
            label4.Name = "label4";
            label4.Size = new Size(111, 17);
            label4.TabIndex = 5;
            label4.Text = "Download Folder:";
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(481, 241);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            ForeColor = Color.FromArgb(192, 0, 0);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Settings";
            Text = "Ripify Settings";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)concurrentDownloads).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TextBox clientID;
        private Label label1;
        private Label label2;
        private GroupBox groupBox1;
        private TextBox clientSecret;
        private Button saveSettingsBtn;
        private GroupBox groupBox2;
        private TextBox downloadPath;
        private Label label3;
        private Label label4;
        private Button browseFolder;
        private NumericUpDown concurrentDownloads;
        private FolderBrowserDialog downloadFolderDlg;
    }
}
