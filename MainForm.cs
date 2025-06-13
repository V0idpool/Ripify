using SpotifyAPI.Web;
using System.Diagnostics;
using YoutubeExplode;
using System.Linq;
using Ripify.Helpers;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
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
                    var items = spotify.Paginate(await spotify.Playlists.GetItems(id));
                    await foreach (var item in items)
                    {
                        if (item.Track is FullTrack track)
                        {
                            string query = $"{track.Artists[0].Name} - {track.Name}";
                            trackQueries.Add(query);
                            trackList.Items.Add(query);
                        }
                    }

                    MessageBox.Show($"Fetched {trackQueries.Count} tracks from playlist.");
                }
                else if (type == "album")
                {
                    int offset = 0;
                    const int limit = 50;
                    bool moreItems = true;

                    while (moreItems)
                    {
                        var page = await spotify.Albums.GetTracks(id, new AlbumTracksRequest { Limit = limit, Offset = offset });

                        foreach (var track in page.Items)
                        {
                            string query = $"{track.Artists[0].Name} - {track.Name}";
                            trackQueries.Add(query);
                            trackList.Items.Add(query);
                        }

                        offset += limit;
                        moreItems = page.Next != null;
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
            List<string> failedDownloads = new();
            if (trackList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select at least one track to download.");
                return;
            }

            if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                return;
            if (etaMbLbl.InvokeRequired)
            {
                Invoke(() => etaMbLbl.Text = "0%");
            }
            else
            {
                etaMbLbl.Text = "0%";
            }
            if (currentTaskLabel.InvokeRequired)
            {
                Invoke(() => currentTaskLabel.Text = "0/0");
            }
            else
            {
                currentTaskLabel.Text = "0/0";
            }
            downloadSelected.Enabled = false;

            var saveFolder = folderBrowserDialog1.SelectedPath;
            var youtube = new YoutubeClient();
            int maxConcurrency = 3; // Limit to 3 concurrent downloads TODO: experiment with values
            var semaphore = new SemaphoreSlim(maxConcurrency);

            progressBar1.Maximum = trackList.SelectedItems.Count;
            progressBar1.Value = 0;

            int totalCount = trackList.SelectedItems.Count;
            int completedCount = 0;
            int startedCount = 0;
            var downloadTasks = new List<Task>();

            for (int i = 0; i < totalCount; i++)
            {
                int index = i;
                string query = (string)trackList.SelectedItems[index];

                await semaphore.WaitAsync();

                var task = Task.Run(async () =>
                {
                    try
                    {
                        int startedIndex = index + 1;

                        Invoke(() => currentTaskLabel.Text = $"{startedIndex}/{totalCount}");

                        var searchResults = youtube.Search.GetVideosAsync(query);
                        var video = await searchResults.FirstOrDefaultAsync();

                        if (video == null)
                        {
                            lock (failedDownloads)
                            {
                                failedDownloads.Add($"{query} (No YouTube result)");
                            }
                            ExceptionHandler.LogDownload($"{query} (No YouTube result)");
                            return;
                        }

                        Invoke(() => progressLbl.Text = $"Downloading {video.Title}");

                        await DownloadAudioFromYoutube(video.Url, saveFolder, index + 1, totalCount);

                        lock (failedDownloads)
                        {
                            completedCount++;
                            Invoke(() =>
                            {
                                progressBar1.Value = completedCount;
                                int percent = (int)((completedCount / (double)totalCount) * 100);
                                etaMbLbl.Text = $"{percent}%";
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (failedDownloads)
                        {
                            failedDownloads.Add($"{query} (Error: {ex.Message})");
                        }
                        ExceptionHandler.LogDownload($"{query} (Error: {ex.Message})");
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });

                downloadTasks.Add(task);
            }

            await Task.WhenAll(downloadTasks);

            Invoke(() =>
            {
                progressLbl.Text = "Download complete!";
                currentTaskLabel.Text = $"{totalCount}/{totalCount}";
            });

           
            if (failedDownloads.Count > 0)
            {
                string failedList = string.Join("\n", failedDownloads);
                var result = MessageBox.Show($"Some tracks failed to download.\nWould you like to open the log file for more details?\n\n{failedDownloads.Count} tracks failed:\n\n{failedList}", "Download Completed with Errors", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    string logPath = Path.Combine(Application.StartupPath, "log_file.txt");
                    if (File.Exists(logPath))
                        Process.Start("notepad.exe", logPath);
                }
            }
            else
            {
                var result = MessageBox.Show("All selected tracks downloaded successfully.\nWould you like to open the log file for more details?", "Download Completed", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    string logPath = Path.Combine(Application.StartupPath, "log_file.txt");
                    if (File.Exists(logPath))
                        Process.Start("notepad.exe", logPath);
                }
            }
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
