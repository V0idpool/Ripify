namespace Ripify
{
    partial class About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            groupBox1 = new GroupBox();
            linkLabel3 = new LinkLabel();
            linkLabel2 = new LinkLabel();
            linkLabel1 = new LinkLabel();
            label1 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(linkLabel3);
            groupBox1.Controls.Add(linkLabel2);
            groupBox1.Controls.Add(linkLabel1);
            groupBox1.Controls.Add(label1);
            groupBox1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.FromArgb(192, 0, 0);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(545, 307);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "About Ripify!";
            // 
            // linkLabel3
            // 
            linkLabel3.AutoSize = true;
            linkLabel3.LinkColor = Color.FromArgb(192, 0, 0);
            linkLabel3.Location = new Point(9, 116);
            linkLabel3.Name = "linkLabel3";
            linkLabel3.Size = new Size(198, 17);
            linkLabel3.TabIndex = 3;
            linkLabel3.TabStop = true;
            linkLabel3.Text = "https://developer.spotify.com/";
            linkLabel3.LinkClicked += linkLabel3_LinkClicked;
            // 
            // linkLabel2
            // 
            linkLabel2.AutoSize = true;
            linkLabel2.LinkColor = Color.FromArgb(192, 0, 0);
            linkLabel2.Location = new Point(9, 168);
            linkLabel2.Name = "linkLabel2";
            linkLabel2.Size = new Size(211, 17);
            linkLabel2.TabIndex = 2;
            linkLabel2.TabStop = true;
            linkLabel2.Text = "https://github.com/yt-dlp/yt-dlp";
            linkLabel2.LinkClicked += linkLabel2_LinkClicked;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = Color.FromArgb(192, 0, 0);
            linkLabel1.Location = new Point(9, 236);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(246, 17);
            linkLabel1.TabIndex = 1;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "https://buymeacoffee.com/byteknight";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.White;
            label1.Location = new Point(9, 29);
            label1.Name = "label1";
            label1.Size = new Size(527, 255);
            label1.TabIndex = 0;
            label1.Text = resources.GetString("label1.Text");
            // 
            // About
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(569, 331);
            Controls.Add(groupBox1);
            ForeColor = Color.FromArgb(192, 0, 0);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "About";
            Text = "About Ripify";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private GroupBox groupBox1;
        private LinkLabel linkLabel1;
        private Label label1;
        private LinkLabel linkLabel2;
        private LinkLabel linkLabel3;
    }
}
