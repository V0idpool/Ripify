using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TagLib;

namespace Ripify.Helpers
{
    public static class MetadataHelper
    {
        public static void AddTagsToFile(string filePath, string title, string artist, string album, byte[] albumArtBytes)
        {
            var file = TagLib.File.Create(filePath);

            file.Tag.Title = title;
            file.Tag.Performers = new[] { artist };
            file.Tag.Album = album;

            if (albumArtBytes != null)
            {
                IPicture pic = new Picture
                {
                    Type = PictureType.FrontCover,
                    Description = "Cover",
                    MimeType = "image/jpeg",
                    Data = albumArtBytes
                };

                file.Tag.Pictures = new IPicture[] { pic };
            }

            file.Save();
        }
        public static async Task<byte[]> DownloadImageAsBytes(string imageUrl)
        {
            using HttpClient client = new();
            return await client.GetByteArrayAsync(imageUrl);
        }
    }
}
