using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ripify.Helpers
{
    public class RecentFilesManager
    {
        private const int MaxRecentFiles = 10;
        private readonly ToolStripMenuItem _recentLinksMenuItem;
        private readonly Action<string> _openLinkCallback;
        private readonly IniHandler _ini;
        private List<string> _recentLinksList;

        public RecentFilesManager(ToolStripMenuItem recentFilesMenuItem, Action<string> openLinkCallback, IniHandler ini)
        {
            _recentLinksMenuItem = recentFilesMenuItem;
            _openLinkCallback = openLinkCallback;
            _ini = ini;
            _recentLinksList = new List<string>();
            LoadRecentLinks();
            UpdateMenu();
        }

        private void LoadRecentLinks()
        {
            _recentLinksList.Clear();
            for (int i = 0; i < MaxRecentFiles; i++)
            {
                string urlPath = _ini.ReadValue("RecentLinks", $"Url{i}", null);
                if (!string.IsNullOrEmpty(urlPath))
                {
                    _recentLinksList.Add(urlPath);
                }
            }
        }

        public void AddLink(string linkUrl)
        {
            _recentLinksList.Remove(linkUrl);

            _recentLinksList.Insert(0, linkUrl);

            if (_recentLinksList.Count > MaxRecentFiles)
            {
                _recentLinksList.RemoveRange(MaxRecentFiles, _recentLinksList.Count - MaxRecentFiles);
            }

            SaveRecentLinks();
            UpdateMenu();
        }

        private void SaveRecentLinks()
        {
            _ini.DeleteSection("RecentLinks");

            for (int i = 0; i < _recentLinksList.Count; i++)
            {
                _ini.WriteValue("RecentLinks", $"Url{i}", _recentLinksList[i], _ini.Path);
            }
        }

        private void UpdateMenu()
        {
            Color color1 = SystemColors.Control;
            Color color2 = Color.FromArgb(232, 232, 232);
            _recentLinksMenuItem.DropDownItems.Clear();

            if (_recentLinksList.Count > 0)
            {
                for (int i = 0; i < _recentLinksList.Count; i++)
                {
                    var filePath = _recentLinksList[i];
                    var item = new ToolStripMenuItem(filePath);

                    if (i % 2 == 0)
                    {
                        item.BackColor = color1;
                        item.ForeColor = Color.Black;
                    }
                    else
                    {
                        item.BackColor = color2;
                        item.ForeColor = Color.Black;
                    }

                    item.Tag = filePath;
                    item.Click += OnRecentLinkClick;
                    _recentLinksMenuItem.DropDownItems.Add(item);
                }

                _recentLinksMenuItem.DropDownItems.Add(new ToolStripSeparator());

                var clearItem = new ToolStripMenuItem("Clear All");

                if (_recentLinksList.Count % 2 == 0)
                {
                    clearItem.BackColor = color2;
                }
                else
                {
                    clearItem.BackColor = color1;
                }

                clearItem.Click += OnClearRecentLinkClick;
                _recentLinksMenuItem.DropDownItems.Add(clearItem);
            }
        }

        private void OnRecentLinkClick(object sender, EventArgs e)
        {
            var item = (ToolStripMenuItem)sender;
            string filePath = item.Tag as string;
            if (!string.IsNullOrEmpty(filePath))
            {
                _openLinkCallback?.Invoke(filePath);
            }
        }

        private void OnClearRecentLinkClick(object sender, EventArgs e)
        {
            _recentLinksList.Clear();
            SaveRecentLinks();
            UpdateMenu();
        }
    }
}
