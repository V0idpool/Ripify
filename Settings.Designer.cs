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
            saveSettingsBtn = new Button();
            clientSecret = new TextBox();
            groupBox1.SuspendLayout();
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
            groupBox1.Controls.Add(saveSettingsBtn);
            groupBox1.Controls.Add(clientSecret);
            groupBox1.Controls.Add(clientID);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.FromArgb(192, 0, 0);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(457, 130);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "Spotify API Details";
            // 
            // saveSettingsBtn
            // 
            saveSettingsBtn.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            saveSettingsBtn.ForeColor = Color.Black;
            saveSettingsBtn.Location = new Point(340, 92);
            saveSettingsBtn.Name = "saveSettingsBtn";
            saveSettingsBtn.Size = new Size(111, 25);
            saveSettingsBtn.TabIndex = 8;
            saveSettingsBtn.Text = "Save Settings...";
            saveSettingsBtn.UseVisualStyleBackColor = true;
            saveSettingsBtn.Click += saveSettingsBtn_Click;
            // 
            // clientSecret
            // 
            clientSecret.Location = new Point(97, 61);
            clientSecret.Name = "clientSecret";
            clientSecret.Size = new Size(354, 25);
            clientSecret.TabIndex = 7;
            // 
            // Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(481, 151);
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
            ResumeLayout(false);
        }

        #endregion

        private TextBox clientID;
        private Label label1;
        private Label label2;
        private GroupBox groupBox1;
        private TextBox clientSecret;
        private Button saveSettingsBtn;
    }
}
