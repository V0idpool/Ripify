using SpotifyAPI.Web;
using System.Diagnostics;
using YoutubeExplode;
using System.Linq;
using Ripify.Helpers;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
using System.Text;
using AngleSharp.Text;
namespace Ripify
{
    public partial class MainForm : Form
    {
        private SpotifyClient spotify;
        private List<string> trackQueries = new();
        private string clientID;
        private string clientSecret;
        public int concurrentDownloads;
        public string downloadsPath;
        public string saveFolder;
        private string exeFfmpeg = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg.exe");
        string ffmpegFolder = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
        private string exeFfprobe = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffprobe.exe");
        private string cookiesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies.txt");
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
            if (!File.Exists(exeFfmpeg))
            {
                string resourceName = "Ripify.Executables.ffmpeg.exe";
                Ripify.Helpers.SaveFiles.SaveToDisk(resourceName, exeFfmpeg);
            }
            if (!File.Exists(exeFfprobe))
            {
                string resourceName = "Ripify.Executables.ffprobe.exe";
                Ripify.Helpers.SaveFiles.SaveToDisk(resourceName, exeFfprobe);
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
            if (string.IsNullOrEmpty(Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "DownloadPath")))
            {
                downloadsPath = "Input Download Path Here...";
            }
            else
            {
                downloadsPath = Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "DownloadPath");
            }
            if (string.IsNullOrEmpty(Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "MaxDownloads")))
            {
                concurrentDownloads = 3;
            }
            else
            {
                string maxDownloads = Helpers.IniHandler.UserSettings(Application.StartupPath + userfile, "MaxDownloads");

                if (!int.TryParse(maxDownloads, out concurrentDownloads) || concurrentDownloads <= 0)
                {
                    concurrentDownloads = 3;
                }
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
        private bool TrackAlreadyExists(string artist, string title, string outputFolder)
        {
            string[] files = Directory.GetFiles(outputFolder, "*.mp3");

            string normArtist = Normalize(artist);
            string normTitle = Normalize(title);

            foreach (var file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                string normFile = Normalize(fileName);

                // Match both artist + title inside the filename
                if (normFile.Contains(normArtist) && normFile.Contains(normTitle))
                    return true;
            }

            return false;
        }

        private string Normalize(string input)
        {
            return input
                .ToLower()
                .Replace("_", " ")
                .Replace("-", " ")
                .Replace("(", " ")
                .Replace(")", " ")
                .Replace("[", " ")
                .Replace("]", " ")
                .Replace("official video", "")
                .Replace("lyrics", "")
                .Replace("audio", "")
                .Replace("hd", "")
                .Replace("hq", "")
                .Trim();
        }
        private async Task<bool> DownloadAudioFromYoutube(string videoUrl, string outputFolder, int currentIndex, int totalCount)
        {

            string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
            string outputTemplate = Path.Combine(outputFolder, "%(title)s.%(ext)s");

            var psi = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = $"--extract-audio --audio-format mp3 --restrict-filenames --cookies \"{cookiesPath}\" --ffmpeg-location \"{ffmpegFolder}\" -o \"{outputTemplate}\" \"{videoUrl}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };

            StringBuilder stderr = new();
            StringBuilder stdout = new();
            var errorBuilder = new StringBuilder();
            process.OutputDataReceived += (sender, e) => { if (e.Data != null) stdout.AppendLine(e.Data); };
            process.ErrorDataReceived += (sender, e) => { if (e.Data != null) stderr.AppendLine(e.Data); };


            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
            // Check if process was successful
            if (process.ExitCode != 0)
            {
                ExceptionHandler.LogDownload($"yt-dlp failed with exit code {process.ExitCode}: {stderr} | Output: {stdout}");
                return false;
            }
            return true;
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
            string userfile;
            userfile = @"\UserCFG.ini";
            if (trackList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select at least one track to download.");
                return;
            }
            if (!File.Exists(cookiesPath))
            {
                var result = MessageBox.Show(
                      "Missing cookies.txt file, this is required to download tracks from YouTube.\n\n" +
                      "To fix this:\n" +
                      "1. Install the 'Get cookies.txt LOCALLY' Chrome extension.\n" +
                      "2. Use it to export your YouTube cookies.\n" +
                      "3. Save the exported file as cookies.txt in the same folder as this app.\n\n" +
                      "Would you like to open the Chrome extension page now?",
                      "Missing cookies.txt",
                      MessageBoxButtons.YesNo,
                      MessageBoxIcon.Error
                  );
                if (result == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://chromewebstore.google.com/detail/get-cookiestxt-locally/cclelndahbckbenkjhflpdbgdldlbecc",
                        UseShellExecute = true
                    });
                }
                return;
            }

            saveFolder = IniHandler.UserSettings(Application.StartupPath + userfile, "DownloadPath");

            if (string.IsNullOrWhiteSpace(saveFolder))
            {
                if (folderBrowserDialog1.ShowDialog() != DialogResult.OK)
                    return;
                saveFolder = folderBrowserDialog1.SelectedPath;
            }
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

            var youtube = new YoutubeClient();
            int maxConcurrency = concurrentDownloads; // Limit to 3 concurrent downloads TODO: experiment with values
            var semaphore = new SemaphoreSlim(maxConcurrency);

            progressBar1.Maximum = trackList.SelectedItems.Count;
            progressBar1.Value = 0;

            int startedCount = 0;
            var downloadTasks = new List<Task>();
            var selectedItems = trackList.SelectedItems.Cast<string>().ToList();
            int totalCount = selectedItems.Count;
            int completedCount = 0;

            for (int i = 0; i < totalCount; i++)
            {
                string query = selectedItems[i];

                await semaphore.WaitAsync();

                var task = Task.Run(async () =>
                {
                    try
                    {
                        int startedIndex = i + 1;

                        Invoke(() => currentTaskLabel.Text = $"{startedIndex}/{totalCount}");
                        // Extract artist + title ONCE per query
                        string artist = "";
                        string title = "";
                        int dashIdx = query.IndexOf("-");

                        if (dashIdx > 0)
                        {
                            artist = query[..dashIdx].Trim();
                            title = query[(dashIdx + 1)..].Trim();
                        }
                        else
                        {
                            title = query;
                        }

                        // Check BEFORE searching YouTube
                        if (TrackAlreadyExists(artist, title, saveFolder))
                        {
                            Invoke(() => progressLbl.Text = $"Skipping existing: {title}");

                            // Count as completed but DO NOT search/download
                            lock (failedDownloads)
                            {
                                completedCount++;
                            }

                            Invoke(() =>
                            {
                                progressBar1.Value = completedCount;
                                etaMbLbl.Text = $"{(int)((completedCount / (double)totalCount) * 100)}%";
                            });

                            return; // IMPORTANT → avoid double semaphore release & double progress
                        }
                        var searchResults = youtube.Search.GetVideosAsync(query).Take(5);
                        bool found = false;
                        await foreach (var video in searchResults)
                        {
                            if (string.IsNullOrEmpty(video?.Url)) continue;

                            Invoke(() => progressLbl.Text = $"Downloading: {video.Title}");

                            bool success = await DownloadAudioFromYoutube(video.Url, saveFolder, i + 1, totalCount);
                            if (success)
                            {
                                found = true;
                                break;
                            }
                            else
                            {
                                ExceptionHandler.LogDownload($"Failed to download {video.Title}, trying next result...");
                            }
                        }

                        if (!found)
                        {
                            lock (failedDownloads)
                            {
                                failedDownloads.Add($"{query} (No valid YouTube video found)");
                            }
                        }

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
                var result = MessageBox.Show($"Some tracks failed to download.\nWould you like to open the log file for more details?\n\n{failedDownloads.Count} tracks failed!", "Download Completed with Errors", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    string logPath = Path.Combine(Application.StartupPath, "log_file.txt");
                    if (File.Exists(logPath))
                        ExceptionHandler.LogInternalError($"{failedDownloads.Count} tracks failed:\n\n{failedList}");
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

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < trackList.Items.Count; i++)
            {
                trackList.SetSelected(i, true);
            }
        }

        private void cancelDownloads_Click(object sender, EventArgs e)
        {

        }
    }
}
