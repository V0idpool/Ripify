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
        private List<string> failedTrackQueries = new();
        private CancellationTokenSource cts;
        private RecentFilesManager recentLinkManager;
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
            string ytdlpExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
            string qjscExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qjs.exe");

            string ytdlpName = "Ripify.Executables.yt-dlp.exe";
            Ripify.Helpers.SaveFiles.SaveToDisk(ytdlpName, ytdlpExePath);

            string qjscName = "Ripify.Executables.qjs.exe";
            Ripify.Helpers.SaveFiles.SaveToDisk(qjscName, qjscExePath);

            string exeFfmpegName = "Ripify.Executables.ffmpeg.exe";
            Ripify.Helpers.SaveFiles.SaveToDisk(exeFfmpegName, exeFfmpeg);

            string exeFfprobeName = "Ripify.Executables.ffprobe.exe";
            Ripify.Helpers.SaveFiles.SaveToDisk(exeFfprobeName, exeFfprobe);

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
            recentLinkManager = new RecentFilesManager(
          toolStripMenuItem2,
          (link) => { playListURL.Text = link; },
          ini
      );
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
        private async Task<bool> DownloadAudioFromYoutube(string videoUrl, string outputFolder, CancellationToken token, int currentIndex, int totalCount)
        {

            string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");

            string outputTemplate = Path.Combine(outputFolder, "%(title)s.%(ext)s");

            string qjsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "qjs.exe");
            var psi = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = $"--extract-audio --audio-format mp3 " +
            $"-f \"bestaudio/best\" " +
            $"--js-runtimes \"quickjs:{qjsPath}\" " +
            $"--cookies \"{cookiesPath}\" " +
            $"--ffmpeg-location \"{ffmpegFolder}\" " +
            $"--extractor-args \"youtube:player-client=android,web;player-skip=web_embedded\" " +
            $"--sleep-requests 2 --sleep-interval 4 " +
            $"--no-check-certificate --no-warnings " +
            $"--user-agent \"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36\" " +
            $"-o \"{outputTemplate}\" \"{videoUrl}\"",
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

            using var registration = token.Register(() =>
            {
                Task.Run(() =>
                {
                    try { if (!process.HasExited) process.Kill(true); } catch { }
                });
            });

            try
            {
                await process.WaitForExitAsync(token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }

            if (process.ExitCode != 0)
            {
                if (!token.IsCancellationRequested)
                {
                    ExceptionHandler.LogDownload($"yt-dlp failed with exit code {process.ExitCode}: {stderr} | Output: {stdout}");
                }
                return false;
            }
            return true;
        }
        private void UpdateFetchStatus(ProgressForm form, int current, int total, TimeSpan elapsed)
        {
            if (total <= 0) return;

            // Calculate percentage
            int percentage = (int)((double)current / total * 100);
            form.UpdateProgress(percentage);

            // Calculate ETA: (Elapsed Time / Current Count) * Remaining Count
            if (current > 0)
            {
                double milliPerTrack = elapsed.TotalMilliseconds / current;
                double remainingMilli = milliPerTrack * (total - current);
                form.UpdateEstimatedTime(TimeSpan.FromMilliseconds(remainingMilli));
            }
        }
        private async void fetchBTN_Click(object sender, EventArgs e)
        {
            fetchBTN.Enabled = false;
            trackList.Items.Clear();
            trackQueries.Clear();
            
            var progressForm = new ProgressForm();
            progressForm.Show();

            try
            {
                this.recentLinkManager.AddLink(playListURL.Text);
                var (type, id) = ExtractPlaylistId(playListURL.Text);
                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(id))
                {
                    MessageBox.Show("Invalid Spotify playlist or album URL.");
                    return;
                }
                
                await InitializeSpotifyClient();
                Stopwatch sw = Stopwatch.StartNew();

                if (type == "playlist")
                {
                    var firstPage = await spotify.Playlists.GetItems(id);
                    int totalTracks = firstPage.Total ?? 0;

                    var items = spotify.Paginate(firstPage);
                    int currentCount = 0;

                    await foreach (var item in items)
                    {
                        if (item.Track is FullTrack track)
                        {
                            string query = $"{track.Artists[0].Name} - {track.Name}";
                            trackQueries.Add(query);
                            trackList.Items.Add(query);
                            currentCount++;
                          
                            UpdateFetchStatus(progressForm, currentCount, totalTracks, sw.Elapsed);
                        }
                    }

                    var toast = new ToastForm($"Fetched {trackQueries.Count} tracks from playlist.");
                    int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
                    int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

                    int x = (screenWidth - toast.Width) / 2;
                    int y = (screenHeight - toast.Height) / 2;
                    toast.ShowAt(new Point(x, y));
                    toast.Refresh();
                }
                else if (type == "album")
                {
                    var album = await spotify.Albums.Get(id);
                    int totalTracks = album.Tracks.Total ?? 0;
                    int currentCount = 0;

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
                            currentCount++;

                            UpdateFetchStatus(progressForm, currentCount, totalTracks, sw.Elapsed);
                        }

                        offset += limit;
                        moreItems = page.Next != null;
                    }
                    
                    var toast = new ToastForm($"Fetched {trackQueries.Count} tracks from album.");
                    int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
                    int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

                    int x = (screenWidth - toast.Width) / 2;
                    int y = (screenHeight - toast.Height) / 2;
                    toast.ShowAt(new Point(x, y));
                    toast.Refresh();
                }
                else
                {
                    MessageBox.Show("Only playlist and album URLs are supported.");
                }

                progressForm.UpdateProgress(100); 
                await Task.Delay(2000);
                progressForm.Close();
                progressForm.Dispose();
                progressForm = null;
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
            string userfile = @"\UserCFG.ini";
            cts = new CancellationTokenSource();
            var token = cts.Token;

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

            Invoke(() =>
            {
                etaMbLbl.Text = "0%";
                currentTaskLabel.Text = "0/0";
            });

            downloadSelected.Enabled = false;
            cancelDownloads.Enabled = true;

            var youtube = new YoutubeClient();
            int maxConcurrency = concurrentDownloads;
            var semaphore = new SemaphoreSlim(maxConcurrency);

            progressBar1.Maximum = trackList.SelectedItems.Count;
            progressBar1.Value = 0;

            var downloadTasks = new List<Task>();
            var selectedItems = trackList.SelectedItems.Cast<string>().ToList();
            int totalCount = selectedItems.Count;
            int completedCount = 0;

            try
            {
                for (int i = 0; i < totalCount; i++)
                {
                    if (token.IsCancellationRequested) break;
                    string query = selectedItems[i];

                    await semaphore.WaitAsync(token);

                    var task = Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(new Random().Next(2000, 5000), token);
                            int startedIndex = i + 1;

                            Invoke(() => currentTaskLabel.Text = $"{startedIndex}/{totalCount}");

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

                            if (TrackAlreadyExists(artist, title, saveFolder))
                            {
                                Invoke(() => progressLbl.Text = $"Skipping existing: {title}");
                                lock (failedDownloads) { completedCount++; }
                                Invoke(() =>
                                {
                                    progressBar1.Value = completedCount;
                                    etaMbLbl.Text = $"{(int)((completedCount / (double)totalCount) * 100)}%";
                                });
                                return; // This return now safely hits the finally block below
                            }

                            var searchResults = youtube.Search.GetVideosAsync(query).Take(5);
                            bool found = false;

                            await foreach (var video in searchResults)
                            {
                                token.ThrowIfCancellationRequested();
                                if (string.IsNullOrEmpty(video?.Url)) continue;

                                Invoke(() => progressLbl.Text = $"Downloading: {video.Title}");

                                bool success = await DownloadAudioFromYoutube(video.Url, saveFolder, token, i + 1, totalCount);
                                if (success)
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                            {
                                lock (failedDownloads) { failedDownloads.Add($"{query} (No valid YouTube video found)"); }
                                lock (failedTrackQueries) { failedTrackQueries.Add(query); }
                            }

                            lock (failedDownloads)
                            {
                                completedCount++;
                                Invoke(() =>
                                {
                                    progressBar1.Value = completedCount;
                                    etaMbLbl.Text = $"{(int)((completedCount / (double)totalCount) * 100)}%";
                                });
                            }
                        }
                        catch (OperationCanceledException) { /* Task-level cancel */ }
                        catch (Exception ex)
                        {
                            lock (failedDownloads) { failedDownloads.Add($"{query} (Error: {ex.Message})"); }
                            lock (failedTrackQueries) { failedTrackQueries.Add(query); }
                            ExceptionHandler.LogDownload($"{query} (Error: {ex.Message})");
                        }
                        finally
                        {
                            semaphore.Release();
                        }
                    }, token);

                    downloadTasks.Add(task);
                }

                await Task.WhenAll(downloadTasks);
            }
            catch (OperationCanceledException)
            {
                Invoke(() => progressLbl.Text = "Downloads cancelled.");
                Invoke(() => currentTaskLabel.Text = "0/0");
                Invoke(() => etaMbLbl.Text = " ");
                Invoke(() => progressBar1.Value = 0);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Fatal Error: " + ex.Message);
            }
            finally
            {
                Invoke(() =>
                {
                    if (progressLbl.Text != "Downloads cancelled.")
                        progressLbl.Text = "Download complete!";

                    currentTaskLabel.Text = $"{completedCount}/{totalCount}";
                    downloadSelected.Enabled = true;
                    cancelDownloads.Enabled = false;
                });
                Invoke(() => etaMbLbl.Text = " ");
                Invoke(() => progressBar1.Value = 0);

                if (failedDownloads.Count > 0 && !token.IsCancellationRequested)
                {
                    string failedList = string.Join("\n", failedDownloads);
                    var result = MessageBox.Show(
                 $"{failedTrackQueries.Count} tracks failed to download.\n\n" +
                 "Would you like to select the failed tracks in the list to try again?",
                 "Download Errors",
                 MessageBoxButtons.YesNo,
                 MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // Clear current selection
                        trackList.SelectedItems.Clear();

                        // Loop through the list and select the matches
                        foreach (string failedQuery in failedTrackQueries)
                        {
                            int index = trackList.Items.IndexOf(failedQuery);
                            if (index != -1)
                            {
                                trackList.SetSelected(index, true);
                            }
                        }
                    }
                }
                else if (!token.IsCancellationRequested)
                {
                    MessageBox.Show("All selected tracks downloaded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                cts.Dispose();
            }
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
            if (cts != null)
            {
                var toast = new ToastForm("Cancelling, please wait...");
                int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
                int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

                int x = (screenWidth - toast.Width) / 2;
                int y = (screenHeight - toast.Height) / 2;
                toast.ShowAt(new Point(x, y));
                toast.Refresh();
                cts.Cancel();
                
                progressLbl.Text = "Cancelling... please wait.";
                cancelDownloads.Enabled = false;
            }
        }

        private void openLogFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Assuming GetLogFilePath() ensures the directory is created as you defined previously.
                string logFilePath = ExceptionHandler.GetLogFilePath();

                // Check if the log file exists (it may not if no errors have occurred yet)
                if (System.IO.File.Exists(logFilePath))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(logFilePath)
                    {
                        UseShellExecute = true // Crucial for opening non-executable files like .txt
                    });
                }
                else
                {
                    // Optionally, open the containing folder instead if the file is missing
                    string logDirectory = System.IO.Path.GetDirectoryName(logFilePath);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(logDirectory)
                    {
                        UseShellExecute = true
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogMessage($"Could not open log file: {ex.Message}");
                MessageBox.Show($"Could not open log file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void openLogFileFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string logFilePath = ExceptionHandler.GetLogFilePath();
                // Optionally, open the containing folder instead if the file is missing
                string logDirectory = System.IO.Path.GetDirectoryName(logFilePath);
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(logDirectory)
                {
                    UseShellExecute = true
                });
            }

            catch (Exception ex)
            {
                ExceptionHandler.LogMessage($"Could not open log file: {ex.Message}");
                MessageBox.Show($"Could not open log file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
