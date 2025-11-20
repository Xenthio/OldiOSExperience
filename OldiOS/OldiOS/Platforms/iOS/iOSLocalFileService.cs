using System;
using System.IO;
using System.Threading.Tasks;
using Foundation;
using UIKit;
using OldiOS.Services.Platforms.iOS;

namespace OldiOS.Shared.Services
{
    /// <summary>
    /// iOS-specific implementation of ILocalFileService that handles both file system and iOS media library images
    /// </summary>
    public class iOSLocalFileService : ILocalFileService
    {
        public async Task<string> GetDataUrlAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            try
            {
                // Check if this is an iOS media library reference
                if (filePath.StartsWith("ios-media://"))
                {
                    return await GetMediaLibraryImageAsDataUrl(filePath);
                }

                // Otherwise treat as regular file
                return await GetFileAsDataUrl(filePath);
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task<string> GetMediaLibraryImageAsDataUrl(string mediaUri)
        {
            return await Task.Run(() =>
            {
                try
                {
                    // Extract persistent ID from uri (format: ios-media://123456789)
                    var idString = mediaUri.Replace("ios-media://", "");
                    if (!ulong.TryParse(idString, out var persistentId))
                        return string.Empty;

                    // Get artwork image from iOS media library
                    var image = iOSMediaLibraryService.GetArtworkImage(persistentId, 180);
                    if (image == null)
                        return string.Empty;

                    // Convert UIImage to PNG data
                    var pngData = image.AsPNG();
                    if (pngData == null)
                        return string.Empty;

                    // Convert to base64
                    var base64 = pngData.GetBase64EncodedString(NSDataBase64EncodingOptions.None);
                    return $"data:image/png;base64,{base64}";
                }
                catch
                {
                    return string.Empty;
                }
            });
        }

        private async Task<string> GetFileAsDataUrl(string filePath)
        {
            try
            {
                // Read the file
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                
                // Determine MIME type
                var mimeType = GetMimeType(filePath);
                
                // Convert to base64 data URL
                var base64 = Convert.ToBase64String(fileBytes);
                return $"data:{mimeType};base64,{base64}";
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".svg" => "image/svg+xml",
                _ => "image/jpeg" // default to JPEG
            };
        }
    }
}
