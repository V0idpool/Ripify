using System.Diagnostics;
using YoutubeExplode;
using System.Linq;
using Ripify.Helpers;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Forms.Application;
using System.Text;
using AngleSharp.Text;
using System.Text.Json;
namespace Ripify
{
    public partial class MainForm : Form
    {
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
        // TODO: Utilize GdiPlusLock for new Custom Theme
        public static readonly object GdiPlusLock = new object();
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
                Arguments = $"--extract-audio --audio-format mp3 --audio-quality 320k " +
            $"--js-runtimes \"quickjs:{qjsPath}\" " +
            $"--cookies \"{cookiesPath}\" " +
            $"--ffmpeg-location \"{ffmpegFolder}\" " +
            $"--extractor-args \"youtube:player-client=android,web;player-skip=web_embedded,tv,ios,mweb\" " +
            $"-f \"ba/b\" " +
            $"--sleep-requests 3 --sleep-interval 5 " +
            $"--no-check-certificate --no-warnings " +
            $"--user-agent \"com.google.android.youtube/19.29.37 (Linux; U; Android 14) gzip\" " +
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

            int percentage = (int)((double)current / total * 100);
            form.UpdateProgress(percentage);

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
            Invoke(() => progressLbl.Text = "Waiting...");
            Invoke(() => currentTaskLabel.Text = "0/0");
            Invoke(() => etaMbLbl.Text = " ");
            Invoke(() => progressBar1.Value = 0);
            string inputUrl = playListURL.Text.Trim();
            var selectedItems = trackList.SelectedItems.Cast<string>().ToList();
            var youtube = new YoutubeClient();
            int currentCount = 0;
            var progressForm = new ProgressForm();

            progressForm.Show();

            if (inputUrl.Contains("youtube.com") || inputUrl.Contains("youtu.be"))
            {
                Stopwatch swYT = Stopwatch.StartNew();
                this.recentLinkManager.AddLink(inputUrl);

                try
                {
                    if (inputUrl.Contains("watch?v=") || inputUrl.Contains("youtu.be/"))
                    {
                        var videoInfo = await youtube.Videos.GetAsync(inputUrl);
                        trackQueries.Add(videoInfo.Title);
                        trackList.Items.Add(videoInfo.Title);
                        currentCount++;

                        UpdateFetchStatus(progressForm, currentCount, 1, swYT.Elapsed);
                    }
                    else if (inputUrl.Contains("list="))
                    {
                        Invoke(() => progressLbl.Text = "Fetching YouTube Playlist...");

                        await foreach (var video in youtube.Playlists.GetVideosAsync(inputUrl))
                        {
                            trackQueries.Add(video.Title);
                            trackList.Items.Add(video.Title);
                            currentCount++;

                            UpdateFetchStatus(progressForm, currentCount, currentCount + 1, swYT.Elapsed);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Could not identify a valid video or playlist ID in the YouTube URL.");
                        progressForm.Close();
                        fetchBTN.Enabled = true;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching YouTube link: " + ex.Message);
                }

                await Task.Delay(1000);
                progressForm.Close();
                fetchBTN.Enabled = true;
                return;
            }
            try
            {
                this.recentLinkManager.AddLink(inputUrl);
                var (type, id) = ExtractPlaylistId(inputUrl);

                if (string.IsNullOrEmpty(type) || string.IsNullOrEmpty(id))
                {
                    MessageBox.Show("Invalid Spotify playlist, album, or track URL.");
                    progressForm.Close();
                    return;
                }

                Stopwatch sw = Stopwatch.StartNew();

                await ProcessSpotifyPlaylist(inputUrl, progressForm, sw);

                var toast = new ToastForm($"Fetched {trackQueries.Count} tracks successfully.");
                int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
                int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
                int x = (screenWidth - toast.Width) / 2;
                int y = (screenHeight - toast.Height) / 2;
                toast.ShowAt(new Point(x, y));
                toast.Refresh();

                progressForm.UpdateProgress(100);
                await Task.Delay(2000);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching metadata: " + ex.Message);
            }
            finally
            {
                progressForm?.Close();
                progressForm?.Dispose();
                fetchBTN.Enabled = true;
            }
        }
        private async Task ProcessSpotifyPlaylist(string url, ProgressForm form, Stopwatch sw)
        {
            string ytDlpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");

            if (!File.Exists(ytDlpPath))
                throw new FileNotFoundException("yt-dlp.exe is missing from the application folder.");

            Invoke(() => progressLbl.Text = "Fetching tracks...");

            var psi = new ProcessStartInfo
            {
                FileName = ytDlpPath,
                Arguments = $"--flat-playlist --dump-json --no-warnings --playlist-items :  \"{url}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();
            var (type, id) = ExtractPlaylistId(url);
            int currentCount = 0;
            if (type == "track")
            {
                Invoke(() => progressLbl.Text = "Executing Scraper for single track...");
                currentCount = await SpotifyScraper(type, id, form, sw);

                if (currentCount == 0)
                    throw new Exception("Failed to scrape single track details.");

                UpdateFetchStatus(form, currentCount, currentCount, sw.Elapsed);
                return;
            }
            while (!process.StandardOutput.EndOfStream)
            {
                string line = await process.StandardOutput.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    using JsonDocument doc = JsonDocument.Parse(line);
                    JsonElement root = doc.RootElement;

                    string title = root.TryGetProperty("title", out var tProp) ? tProp.GetString() : "";
                    string uploader = root.TryGetProperty("uploader", out var uProp) ? uProp.GetString() : "";

                    if (!string.IsNullOrEmpty(title))
                    {
                        string formattedQuery = !string.IsNullOrEmpty(uploader) ? $"{uploader} - {title}" : title;

                        trackQueries.Add(formattedQuery);
                        Invoke(() => trackList.Items.Add(formattedQuery));
                        currentCount++;
                        Invoke(() => progressLbl.Text = $"Fetched {currentCount} tracks...");
                        Invoke(() => currentTaskLabel.Text = $"{currentCount} tracks");
                        UpdateFetchStatus(form, currentCount, currentCount + 1, sw.Elapsed);
                    }
                }
                catch { /*Ignore unparseable lines */ }
            }

            await process.WaitForExitAsync();

            if (currentCount == 0)
            {
                Invoke(() => progressLbl.Text = "Executing Scraper...");

                if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(id))
                {
                    currentCount = await SpotifyScraper(type, id, form, sw);
                }

                if (currentCount == 0)
                {
                    throw new Exception("Both yt-dlp and the fallback failed. Spotify likely updated their UI. Download the latest release of yt-dlp.exe from GitHub and replace the file in your folder.");
                }
            }

            UpdateFetchStatus(form, currentCount, currentCount, sw.Elapsed);
        }
        private async Task<int> SpotifyScraper(string type, string id, ProgressForm form, Stopwatch sw)
        {
            int count = 0;
            Invoke(() => progressLbl.Text = "Initializing browser...");

            var webView = new Microsoft.Web.WebView2.WinForms.WebView2();

            webView.Size = new System.Drawing.Size(1920, 1080);
            webView.Location = new System.Drawing.Point(-3000, -3000);
            this.Controls.Add(webView);

            try
            {
                await webView.EnsureCoreWebView2Async(null);

                string url = "https://open.spotify.com/" + type + "/" + id;

                var tcs = new TaskCompletionSource<bool>();
                EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs> navHandler = null;
                navHandler = (sender, args) =>
                {
                    webView.CoreWebView2.NavigationCompleted -= navHandler;
                    tcs.SetResult(true);
                };
                webView.CoreWebView2.NavigationCompleted += navHandler;

                Invoke(() => progressLbl.Text = "Loading playlist page...");
                webView.CoreWebView2.Navigate(url);
                await tcs.Task;

                await Task.Delay(4000);

                var dataTcs = new TaskCompletionSource<string>();
                EventHandler<Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs> msgHandler = null;
                msgHandler = (sender, args) =>
                {
                    webView.CoreWebView2.WebMessageReceived -= msgHandler;
                    dataTcs.TrySetResult(args.TryGetWebMessageAsString());
                };
                webView.CoreWebView2.WebMessageReceived += msgHandler;

                Invoke(() => progressLbl.Text = "Scraping tracks...");

                string js = @"
            (async function() {
                try {
                    var tracks = new Set();
                    var consecutiveNoScroll = 0;
                    var lastScrollTop = -1;

                    var banners = document.querySelectorAll('#onetrust-banner-sdk, .NavBarFooter');
                    banners.forEach(function(b) { b.remove(); });

                    var scrollNode = document.querySelector('.main-view-container__scroll-node [data-overlayscrollbars-viewport]');

                    if (!scrollNode) {
                        window.chrome.webview.postMessage(JSON.stringify({ type: 'done', success: false, error: 'Could not find the main playlist scroll container.' }));
                        return;
                    }

                    scrollNode.scrollTop = 0;
                    await new Promise(r => setTimeout(r, 1000));

                    while (consecutiveNoScroll < 10) {
                        var rows = document.querySelectorAll('div[data-testid=""tracklist-row""]');
                        
                        if (rows.length > 0) {
                            rows.forEach(function(row) {
                                var titleEl = row.querySelector('a[data-testid=""internal-track-link""]');
                                var artistEls = row.querySelectorAll('a[href^=""/artist/""]');

                                if (titleEl) {
                                    var title = titleEl.textContent ? titleEl.textContent.trim() : '';
                                    var artist = 'Unknown';
                                    
                                    if (artistEls.length > 0) {
                                        var artists = [];
                                        artistEls.forEach(function(a) { artists.push(a.textContent.trim()); });
                                        artist = artists.join(', ');
                                    }
                                    
                                    if (title !== '') {
                                        tracks.add(artist + ' - ' + title);
                                    }
                                }
                            });
                        }

                        scrollNode.scrollBy({ top: 1500, behavior: 'auto' });

                        await new Promise(r => setTimeout(r, 700));

                        if (Math.abs(scrollNode.scrollTop - lastScrollTop) < 5) {
                            consecutiveNoScroll++;
                            
                            var currentRows = document.querySelectorAll('div[data-testid=""tracklist-row""]');
                            if (currentRows.length > 0) {
                                currentRows[currentRows.length - 1].scrollIntoView({ behavior: 'auto', block: 'end' });
                            }
                            await new Promise(r => setTimeout(r, 300));
                        } else {
                            consecutiveNoScroll = 0;
                            lastScrollTop = scrollNode.scrollTop;
                            window.chrome.webview.postMessage(JSON.stringify({ type: 'progress', count: tracks.size }));
                        }
                    }

                    window.chrome.webview.postMessage(JSON.stringify({ type: 'done', success: true, data: Array.from(tracks) }));
                } catch (err) {
                    window.chrome.webview.postMessage(JSON.stringify({ type: 'done', success: false, error: err.message }));
                }
            })();
        ";

                await webView.CoreWebView2.ExecuteScriptAsync(js);

                string finalResult = null;
                while (true)
                {
                    string msg = await dataTcs.Task;
                    using JsonDocument doc = JsonDocument.Parse(msg);
                    string msgType = doc.RootElement.GetProperty("type").GetString();

                    if (msgType == "progress")
                    {
                        int currentScraped = doc.RootElement.GetProperty("count").GetInt32();
                        Invoke(() => progressLbl.Text = $"Scanning playlist... ({currentScraped} tracks found)");

                        UpdateFetchStatus(form, currentScraped, currentScraped + 1, sw.Elapsed);

                        dataTcs = new TaskCompletionSource<string>();
                        webView.CoreWebView2.WebMessageReceived += msgHandler;
                    }
                    else if (msgType == "done")
                    {
                        finalResult = msg;
                        break;
                    }
                }

                using JsonDocument finalDoc = JsonDocument.Parse(finalResult);
                if (finalDoc.RootElement.GetProperty("success").GetBoolean())
                {
                    var tracksArray = finalDoc.RootElement.GetProperty("data");
                    Invoke(() => trackList.BeginUpdate());

                    foreach (JsonElement element in tracksArray.EnumerateArray())
                    {
                        string query = element.GetString();
                        trackQueries.Add(query);
                        Invoke(() => {
                            trackList.Items.Add(query);
                            currentTaskLabel.Text = $"{++count} tracks";
                        });
                    }
                }
                else
                {
                    string err = finalDoc.RootElement.GetProperty("error").GetString();
                    throw new Exception($"Scraper Error: {err}");
                }
            }
            catch (Exception ex)
            {
                Ripify.Helpers.ExceptionHandler.LogMessage($"Critical error: {ex.Message}");
                throw;
            }
            finally
            {
                Invoke(() => {
                    trackList.EndUpdate();
                    this.Controls.Remove(webView);
                    webView.Dispose();
                });
            }

            Invoke(() => progressLbl.Text = $"Fetched all {count} tracks!");
            return count;
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
                                return;
                            }
                            bool isDirectLink = query.Contains("youtube.com") || query.Contains("youtu.be");

                            if (isDirectLink)
                            {
                                var videoInfo = await youtube.Videos.GetAsync(query);
                                Invoke(() => progressLbl.Text = $"Downloading: {videoInfo.Title}");
                              
                                bool success = await DownloadAudioFromYoutube(query, saveFolder, token, i + 1, totalCount);

                                // Logic updated to match the 'else' block
                                lock (failedDownloads)
                                {
                                    completedCount++;
                                    Invoke(() =>
                                    {
                                        // Use the shared variables so progress moves correctly
                                        progressBar1.Maximum = totalCount;
                                        progressBar1.Value = completedCount;
                                        etaMbLbl.Text = $"{(int)((completedCount / (double)totalCount) * 100)}%";
                                    });

                                    if (!success)
                                    {
                                        failedDownloads.Add($"{query} (Download Failed)");
                                    }
                                }
                            }
                            else
                            {
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
                //Invoke(() => etaMbLbl.Text = " ");
                //Invoke(() => progressBar1.Value = 0);

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
                        trackList.SelectedItems.Clear();

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

        private void playListURL_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
