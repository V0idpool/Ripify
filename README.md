# Ripify  
![Ripify](https://img.shields.io/badge/version-1.2-darkred) ![Release](https://img.shields.io/badge/Release-06%2F11%2F2025-blue)

<p align="center">
  <img src="https://github.com/V0idpool/Ripify/blob/main/iconripify.png" alt="Ripify Logo" width="300" height="300">
</p>

**Ripify** is a Windows app that helps you save music from Spotify playlists or albums by finding matching audio on YouTube and downloading MP3s using yt-dlp. It uses the Spotify API and YouTubeExplode under the hood to streamline the whole process of building your personal music collection.

## ⚙️ Version: 1.2
## 📅 Originally Released: 06/11/2025  

---

## 🎵 What Ripify Does

- **Pulls tracks from Spotify**: Just drop in a public playlist or album, and Ripify will grab the track info.
- **Finds audio on YouTube**: It searches YouTube for each track and uses [yt-dlp](https://github.com/yt-dlp/yt-dlp) to download high-quality MP3s.
- **Batch downloading**: Grab multiple songs in one go, complete with progress updates and a max concurrent download of 3 (Can be increased).
- **Pick your folder**: You can choose where your music gets saved.
- **Preview before download**: See the list of songs it found before starting the download.
- **Automatic yt-dlp setup**: No need to install yt-dlp yourself—Ripify handles that.
- **Remembers your setup**: Your Spotify API keys and other settings are saved in a config file.

---

## 🛠️ Getting Started

### 1. Download  
Head over to the [Releases page](https://github.com/V0idpool/Ripify/releases/) and grab the latest version.

### 2. Build from source (if needed, otherwise just download the Release)  
If you're building from source, open `Ripify.sln` in Visual Studio and build it like any standard C# project.
Ensure you download **FFmpeg Full Build (The latest version)** from https://www.gyan.dev/ffmpeg/builds/ and put ffmpeg.exe and ffprobe.exe in the "Executables" Folder.

### 3. First Time Running It  
When you launch Ripify for the first time:
- It’ll create a `UserCFG.ini` file if one doesn’t exist.
- You’ll be asked to enter your Spotify `ClientID` and `ClientSecret`.
- You will need to put a copy of your cookies file containing your logged in youtube account, this is to bypass age restriction or private content checks.
- Use this in chrome browser with youtube.com open, and export the cookies to a text file: https://chromewebstore.google.com/detail/get-cookiestxt-locally/cclelndahbckbenkjhflpdbgdldlbecc
- Name the text file cookies.txt and place it in the same directory as Ripify.

---

## 🧪 How to Use

### 🎧 Load a Spotify Playlist or Album

1. Copy the link to a Spotify playlist or album.
2. Paste it into the app’s input field.
3. Click **Fetch**. Ripify will pull all track names and prep YouTube search queries for each.

### 🎵 Download Your Music

1. Pick the tracks you want, or select all.
2. Choose a folder to save them.
3. Ripify will search YouTube, find the best matches, and download them as MP3s using yt-dlp.

---

## ⚙️ Configuration File (`UserCFG.ini`)

Your settings are saved in this file in the app's folder. Here’s what it looks like:

```ini
[Settings]
ClientID=your_spotify_client_id
ClientSecret=your_spotify_client_secret
