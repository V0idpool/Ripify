using SpotifyAPI.Web;
using System.Diagnostics;
using YoutubeExplode;
using System.Linq;
using System.Reflection;
namespace Ripify
{
    public partial class Settings : Form
    {
        private SpotifyClient spotify;
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
                Ripify.Helpers.SaveFiles.SaveToDisk("UserCFG.ini", Application.StartupPath + @"\UserCFG.ini");
            }
            var ini = new Ripify.Helpers.IniHandler();
            ini.Path = Application.StartupPath + @"\UserCFG.ini";
            if (string.IsNullOrEmpty(Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "ClientID")))
            {
                clientID.Text = "Input Spotify Client ID...";
            }
            else
            {
                clientID.Text = Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "ClientID");
            }
            if (string.IsNullOrEmpty(Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "ClientSecret")))
            {
                clientSecret.Text = "Input Spotify Client Secret...";
            }
            else
            {
                clientSecret.Text = Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "ClientSecret");
            }
        }

        private void saveSettingsBtn_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to save your settings?", "Ripify", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                var ini = new Ripify.Helpers.IniHandler();
                ini.Path = Application.StartupPath + @"\UserCFG.ini";
                ini.WriteValue("Settings", "ClientID", clientID.Text, ini.GetPath());
                ini.WriteValue("Settings", "ClientSecret", clientSecret.Text, ini.GetPath());
                this.Close();
                
            }
            else if (dialogResult == DialogResult.No)
            {
                // do nothing, or something else? Don't need this but it's here if needed eventually.
            }
        }
    }
}
