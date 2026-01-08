using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ripify
{
    public partial class ProgressForm : Form
    {
        private int _lastProgress = 0;
        private bool _isCompleted = false;
        private readonly object _progressLock = new object();
        public ProgressForm()
        {
            InitializeComponent();
        }

        private void ProcessingForm_Load(object sender, EventArgs e)
        {

        }
        public void UpdateProgress(int percentage)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)(() => UpdateProgress(percentage)));
                return;
            }

            if (percentage < 0) percentage = 0;
            if (percentage > 100) percentage = 100;

            nsProgressBar1.Value = percentage;
        }
        public void UpdateEstimatedTime(TimeSpan estimatedTime)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)(() => UpdateEstimatedTime(estimatedTime)));
                return;
            }

            estimatedTimeLbl.Text = estimatedTime.ToString(@"hh\:mm\:ss");
        }
    }
}
