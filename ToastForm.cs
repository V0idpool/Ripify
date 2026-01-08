using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Ripify
{
    public partial class ToastForm : Form
    {
        public ToastForm()
        {
            InitializeComponent();
        }
        private Timer timer;
        private Label lbl;
        public ToastForm(string message, int durationMs = 3000, bool autoClose = true)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;
            this.BackColor = Color.Black;
            this.ForeColor = Color.FromArgb(255, 128, 0);
            this.Opacity = 0.8;
            this.Size = new Size(300, 80);
            this.TopMost = true;
            this.ShowInTaskbar = false;

            lbl = new Label()
            {
                Text = message,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 128, 0)
            };
            this.Controls.Add(lbl);

            if (autoClose)
            {
                timer = new Timer { Interval = durationMs };
                timer.Tick += (s, e) => { timer.Stop(); this.Close(); };
                timer.Start();
            }
        }

        public void ShowAt(Point location)
        {
            this.Location = location;
            this.Show();
        }
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        CreateParams cp = base.CreateParams;
        //        cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
        //        return cp;
        //    }
        //}
        public void UpdateMessage(string message, int durationMs = 2000)
        {
            lbl.Text = message;

            // Restart timer with new duration
            if (timer != null)
            {
                timer.Stop();
                timer.Interval = durationMs;
                timer.Start();
            }
            else
            {
                timer = new Timer { Interval = durationMs };
                timer.Tick += (s, e) => { timer.Stop(); this.Close(); };
                timer.Start();
            }
        }
        private void ToastFrm_Load(object sender, EventArgs e)
        {

        }
    }
}
