using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace OldiOS.Shared.Services
{
    /// <summary>
    /// Service for converting local file paths to base64 data URLs for use in Blazor
    /// </summary>
    public interface ILocalFileService
    {
        Task<string> GetDataUrlAsync(string filePath);
    }

    public class LocalFileService : ILocalFileService
    {
        private readonly ConcurrentDictionary<string, string> _cache = new();

        public async Task<string> GetDataUrlAsync(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return string.Empty;

            // Check cache first
            if (_cache.TryGetValue(filePath, out var cachedDataUrl))
                return cachedDataUrl;

            try
            {
                // Read the file
                var fileBytes = await File.ReadAllBytesAsync(filePath);
                
                // Determine MIME type
                var mimeType = GetMimeType(filePath);
                
                // Convert to base64 data URL
                var base64 = Convert.ToBase64String(fileBytes);
                var dataUrl = $"data:{mimeType};base64,{base64}";
                
                // Cache it
                _cache[filePath] = dataUrl;
                
                return dataUrl;
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
