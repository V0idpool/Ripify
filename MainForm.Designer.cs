namespace Ripify
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            playListURL = new TextBox();
            fetchBTN = new Button();
            progressBar1 = new ProgressBar();
            folderBrowserDialog1 = new FolderBrowserDialog();
            trackList = new ListBox();
            downloadSelected = new Button();
            label1 = new Label();
            label2 = new Label();
            progressLbl = new Label();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            currentTaskLabel = new Label();
            etaMbLbl = new Label();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // playListURL
            // 
            playListURL.Location = new Point(133, 38);
            playListURL.Name = "playListURL";
            playListURL.Size = new Size(224, 23);
            playListURL.TabIndex = 0;
            // 
            // fetchBTN
            // 
            fetchBTN.BackColor = SystemColors.Control;
            fetchBTN.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            fetchBTN.ForeColor = Color.Black;
            fetchBTN.Location = new Point(363, 38);
            fetchBTN.Name = "fetchBTN";
            fetchBTN.Size = new Size(106, 23);
            fetchBTN.TabIndex = 1;
            fetchBTN.Text = "Fetch Songs";
            fetchBTN.UseVisualStyleBackColor = false;
            fetchBTN.Click += fetchBTN_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 484);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(457, 23);
            progressBar1.TabIndex = 2;
            // 
            // trackList
            // 
            trackList.BackColor = Color.Black;
            trackList.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            trackList.ForeColor = Color.FromArgb(192, 0, 0);
            trackList.FormattingEnabled = true;
            trackList.ItemHeight = 17;
            trackList.Location = new Point(12, 75);
            trackList.Name = "trackList";
            trackList.SelectionMode = SelectionMode.MultiExtended;
            trackList.Size = new Size(457, 344);
            trackList.TabIndex = 3;
            // 
            // downloadSelected
            // 
            downloadSelected.BackColor = SystemColors.Control;
            downloadSelected.FlatStyle = FlatStyle.System;
            downloadSelected.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            downloadSelected.ForeColor = Color.Black;
            downloadSelected.Location = new Point(12, 430);
            downloadSelected.Name = "downloadSelected";
            downloadSelected.Size = new Size(457, 31);
            downloadSelected.TabIndex = 4;
            downloadSelected.Text = "Download Selected...";
            downloadSelected.UseVisualStyleBackColor = false;
            downloadSelected.Click += downloadSelected_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.75F);
            label1.ForeColor = Color.Red;
            label1.Location = new Point(12, 40);
            label1.Name = "label1";
            label1.Size = new Size(119, 17);
            label1.TabIndex = 5;
            label1.Text = "Album/Playlist URL:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.75F);
            label2.ForeColor = Color.Red;
            label2.Location = new Point(12, 464);
            label2.Name = "label2";
            label2.Size = new Size(63, 17);
            label2.TabIndex = 6;
            label2.Text = "Progress:";
            // 
            // progressLbl
            // 
            progressLbl.AutoSize = true;
            progressLbl.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            progressLbl.ForeColor = Color.Red;
            progressLbl.Location = new Point(12, 511);
            progressLbl.Name = "progressLbl";
            progressLbl.Size = new Size(57, 13);
            progressLbl.TabIndex = 7;
            progressLbl.Text = "Waiting...";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(481, 24);
            menuStrip1.TabIndex = 9;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { settingsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(125, 22);
            settingsToolStripMenuItem.Text = "Settings...";
            settingsToolStripMenuItem.Click += settingsToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // currentTaskLabel
            // 
            currentTaskLabel.AutoSize = true;
            currentTaskLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            currentTaskLabel.ForeColor = Color.Red;
            currentTaskLabel.Location = new Point(74, 467);
            currentTaskLabel.Name = "currentTaskLabel";
            currentTaskLabel.Size = new Size(23, 13);
            currentTaskLabel.TabIndex = 10;
            currentTaskLabel.Text = "0/0";
            // 
            // etaMbLbl
            // 
            etaMbLbl.AutoSize = true;
            etaMbLbl.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            etaMbLbl.ForeColor = Color.Red;
            etaMbLbl.Location = new Point(440, 467);
            etaMbLbl.Name = "etaMbLbl";
            etaMbLbl.Size = new Size(0, 13);
            etaMbLbl.TabIndex = 11;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(481, 531);
            Controls.Add(etaMbLbl);
            Controls.Add(currentTaskLabel);
            Controls.Add(progressLbl);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(downloadSelected);
            Controls.Add(trackList);
            Controls.Add(progressBar1);
            Controls.Add(fetchBTN);
            Controls.Add(playListURL);
            Controls.Add(menuStrip1);
            ForeColor = Color.FromArgb(192, 0, 0);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "MainForm";
            Text = "Ripify";
            Load += MainForm_Load;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox playListURL;
        private Button fetchBTN;
        private ProgressBar progressBar1;
        private FolderBrowserDialog folderBrowserDialog1;
        private ListBox trackList;
        private Button downloadSelected;
        private Label label1;
        private Label label2;
        private Label progressLbl;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Label currentTaskLabel;
        private Label etaMbLbl;
    }
}
