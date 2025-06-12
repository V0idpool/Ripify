using SpotifyAPI.Web;
using System.Diagnostics;
using YoutubeExplode;
using System.Linq;
namespace Ripify
{
    public partial class MainForm : Form
    {
        private SpotifyClient spotify;
        private List<string> trackQueries = new();
        private string clientID;
        private string clientSecret;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string exePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");

            if (!File.Exists(exePath))
            {
                string resourceName = "Ripify.Executables.yt-dlp.exe";
                Ripify.Helpers.SaveFiles.SaveToDisk(resourceName, exePath);
            }
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
                clientID = "Input Spotify Client ID...";
            }
            else
            {
                clientID = Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "ClientID");
            }
            if (string.IsNullOrEmpty(Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "ClientSecret")))
            {
                clientSecret = "Input Spotify Client Secret...";
            }
            else
            {
                clientSecret = Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "ClientSecret");
            }
        }
        private async Task InitializeSpotifyClient()
        {
            if (spotify != null) return;

            var config = SpotifyClientConfig.CreateDefault();

            var request = new ClientCredentialsRequest(clientID, clientSecret);
            var response = await new OAuthClient(config).RequestToken(request);

            spotify = new SpotifyClient(config.WithToken(response.AccessToken));
        }
        private (string Type, string Id) ExtractPlaylistId(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.Segments;
                if (segments.Length >= 3)
                {
                    string type = segments[1].Trim('/');
                    string id = segments[2].Trim('/');
                    return (type, id);
                }
            }
            catch { }

            return (null, null);
        }
        private async Task DownloadAudioFromYoutube(string videoUrl, string outputFolder, int currentIndex, int totalCount)
        {
            if (currentTaskLabel.InvokeRequired)
            {
                currentTaskLabel.Invoke(() => currentTaskLabel.Text = $"{currentIndex}/{totalCount}");
            }
            else
            {
                currentTaskLabel.Text = $"{currentIndex}/{totalCount}";
            }
            string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
            string outputTemplate = Path.Combine(outputFolder, "%(title)s.%(ext)s");

            var psi = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = $"--extract-audio --audio-format mp3 -o \"{outputTemplate}\" \"{videoUrl}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };

            

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
        }
        private async void fetchBTN_Click(object sender, EventArgs e)
        {
            fetchBTN.Enabled = false;
            trackList.Items.Clear();
            trackQueries.Clear();

            try
            {
                var (type, id) = ExtractPlaylistId(playListURL.Text);
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(id))
                {
                    MessageBox.Show("Invalid Spotify playlist or album URL.");
                    return;
                }

                await InitializeSpotifyClient();

                if (type == "playlist")
                {
                    var playlist = await spotify.Playlists.Get(id);

                    foreach (var item in playlist.Tracks.Items)
                    {
                        if (item.Track is FullTrack track)
                        {
                            string query = $"{track.Name} {track.Artists[0].Name}";
                            trackQueries.Add(query);
                            trackList.Items.Add(query);
                        }
                    }

                    MessageBox.Show($"Fetched {trackQueries.Count} tracks from playlist.");
                }
                else if (type == "album")
                {
                    var album = await spotify.Albums.Get(id);

                    foreach (var track in album.Tracks.Items)
                    {
                        string query = $"{track.Name} {track.Artists[0].Name}";
                        trackQueries.Add(query);
                        trackList.Items.Add(query);
                    }

                    MessageBox.Show($"Fetched {trackQueries.Count} tracks from album.");
                }
                else
                {
                    MessageBox.Show("Only playlist and album URLs are supported.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching playlist: " + ex.Message);
            }
            finally
            {
                fetchBTN.Enabled = true;
            }
        }

        private async void downloadSelected_Click(object sender, EventArgs e)
        {
            if (trackList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select at least one track to download.");
                return;
            }

            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;
            downloadSelected.Enabled = false;
            var saveFolder = folderBrowserDialog1.SelectedPath;

            var youtube = new YoutubeClient();

            progressBar1.Maximum = trackList.SelectedItems.Count;
            progressBar1.Value = 0;

            int totalCount = trackList.SelectedItems.Count;
            int currentIndex = 1;
            foreach (string query in trackList.SelectedItems)
            {
                progressLbl.Text = $"Searching YouTube for {query}";
                var searchResults = youtube.Search.GetVideosAsync(query);

                var video = await searchResults.FirstOrDefaultAsync();
                if (video == null)
                {
                    MessageBox.Show($"No YouTube video found for {query}");
                    continue;
                }

                progressLbl.Text = $"Downloading {video.Title}";
                await DownloadAudioFromYoutube(video.Url, saveFolder, currentIndex, totalCount);
               
                progressBar1.Value++;
                int percent = (int)((progressBar1.Value / (double)progressBar1.Maximum) * 100);
                etaMbLbl.Text = $"{percent}%";
                currentIndex++;
            }

            progressLbl.Text = "Download complete!";
            MessageBox.Show("Selected downloads finished.");
            downloadSelected.Enabled = true;
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var settings = new Settings();
            settings.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var about = new About();
            about.ShowDialog();
        }
    }
}
