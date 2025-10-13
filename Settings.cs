using SpotifyAPI.Web;
using System.Diagnostics;
using YoutubeExplode;
using System.Linq;
using System.Reflection;
using Ripify.Helpers;
using System.Windows.Forms;
namespace Ripify
{
    public partial class Settings : Form
    {
        private List<string> trackQueries = new();

        public Settings()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string userfile;
            userfile = @"\UserCFG.ini";
            string userconfigs;
            userconfigs = Application.StartupPath + @"\UserCFG.ini";
            if (!System.IO.File.Exists(userconfigs))
            {
                MessageBox.Show("UserCFG.ini not found in Application Directory, Creating file...");
                SaveFiles.SaveToDisk("UserCFG.ini", Application.StartupPath + @"\UserCFG.ini");
            }
            var ini = new IniHandler();
            ini.Path = Application.StartupPath + @"\UserCFG.ini";
            if (string.IsNullOrEmpty(IniHandler.UserSettings(Application.StartupPath + userfile, "ClientID")))
            {
                clientID.Text = "Input Spotify Client ID...";
            }
            else
            {
                clientID.Text = IniHandler.UserSettings(Application.StartupPath + userfile, "ClientID");
            }
            if (string.IsNullOrEmpty(IniHandler.UserSettings(Application.StartupPath + userfile, "ClientSecret")))
            {
                clientSecret.Text = "Input Spotify Client Secret...";
            }
            else
            {
                clientSecret.Text = IniHandler.UserSettings(Application.StartupPath + userfile, "ClientSecret");
            }
            if (string.IsNullOrEmpty(IniHandler.UserSettings(Application.StartupPath + userfile, "DownloadPath")))
            {
                downloadPath.Text = "Folder To Store Downloads...";
            }
            else
            {
                downloadPath.Text = IniHandler.UserSettings(Application.StartupPath + userfile, "DownloadPath");
            }
            if (string.IsNullOrEmpty(IniHandler.UserSettings(Application.StartupPath + userfile, "MaxDownloads")))
            {
                concurrentDownloads.Value = 3;
            }
            else
            {
                if (int.TryParse(IniHandler.UserSettings(Application.StartupPath + userfile, "MaxDownloads"), out int maxDownloads))
                {
                    concurrentDownloads.Value = maxDownloads;
                }
                else
                {
                    concurrentDownloads.Value = 3;
                }
            }

        }

        private void saveSettingsBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to save your settings?", "Ripify", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var ini = new IniHandler();
                ini.Path = Application.StartupPath + @"\UserCFG.ini";
                ini.WriteValue("Settings", "ClientID", clientID.Text, ini.GetPath());
                ini.WriteValue("Settings", "ClientSecret", clientSecret.Text, ini.GetPath());
                ini.WriteValue("Settings", "DownloadPath", downloadPath.Text, ini.GetPath());
                ini.WriteValue("Settings", "MaxDownloads", concurrentDownloads.Value.ToString(), ini.GetPath());
                this.Close();

            }
            else if (dialogResult == DialogResult.No)
            {
                // do nothing, or something else? Don't need this but it's here if needed eventually.
            }
        }

        private void browseFolder_Click(object sender, EventArgs e)
        {
            if (downloadFolderDlg.ShowDialog() != DialogResult.OK)
                return;
            downloadPath.Text = downloadFolderDlg.SelectedPath;
        }
    }
}
