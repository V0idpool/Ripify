using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;

namespace Ripify
{
    partial class ToastForm
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
            SuspendLayout();
            // 
            // ToastFrm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(300, 80);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Name = "ToastFrm";
            Opacity = 0.8D;
            Text = "RustForge [Notification]";
            TopMost = true;
           
            ResumeLayout(false);
        }

        #endregion
    }
}