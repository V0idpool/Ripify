using SpotifyAPI.Web;
using System.Diagnostics;
using YoutubeExplode;
using System.Linq;
using System.Reflection;
namespace Ripify
{
    public partial class About : Form
    {


        public About()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://buymeacoffee.com/byteknight") { UseShellExecute = true });
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://github.com/yt-dlp/yt-dlp") { UseShellExecute = true });
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo("https://developer.spotify.com/") { UseShellExecute = true });
        }
    }
}
