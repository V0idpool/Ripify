using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Ripify
{
    partial class ProgressForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            nsProgressBar1 = new ProgressBar();
            estimatedTimeLbl = new Label();
            label3 = new Label();
            label1 = new Label();
            SuspendLayout();
            // 
            // nsProgressBar1
            // 
            nsProgressBar1.Location = new Point(24, 50);
            nsProgressBar1.Name = "nsProgressBar1";
            nsProgressBar1.Size = new Size(420, 23);
            nsProgressBar1.TabIndex = 6;
            nsProgressBar1.Text = "nsProgressBar1";
            // 
            // estimatedTimeLbl
            // 
            estimatedTimeLbl.AutoSize = true;
            estimatedTimeLbl.ForeColor = Color.White;
            estimatedTimeLbl.Location = new Point(120, 86);
            estimatedTimeLbl.Name = "estimatedTimeLbl";
            estimatedTimeLbl.Size = new Size(76, 15);
            estimatedTimeLbl.TabIndex = 8;
            estimatedTimeLbl.Text = "Calculating...";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.White;
            label3.Location = new Point(24, 86);
            label3.Name = "label3";
            label3.Size = new Size(97, 15);
            label3.TabIndex = 7;
            label3.Text = "Time Remaining:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.White;
            label1.Location = new Point(24, 23);
            label1.Name = "label1";
            label1.Size = new Size(242, 15);
            label1.TabIndex = 9;
            label1.Text = "Ripify is fetching your track list, Please wait...";
            // 
            // ProgressForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(469, 124);
            Controls.Add(label1);
            Controls.Add(nsProgressBar1);
            Controls.Add(estimatedTimeLbl);
            Controls.Add(label3);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ProgressForm";
            Text = "Ripify [Loading Tracks...]";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label2;
        private ProgressBar nsProgressBar1;
        public Label estimatedTimeLbl;
        public Label label3;
        public Label label1;
    }
}