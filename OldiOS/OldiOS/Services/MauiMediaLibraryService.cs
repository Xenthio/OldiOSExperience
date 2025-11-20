using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OldiOS.Shared.Models;
using OldiOS.Shared.Services;
using Microsoft.Maui.Storage;

namespace OldiOS.Services
{
    public class MauiMediaLibraryService : IMediaLibraryService
    {
        private bool _hasPermission = false;
        private List<Song> _cachedSongs = new();
        private List<Album> _cachedAlbums = new();
        private List<Artist> _cachedArtists = new();

        public bool HasPermission => _hasPermission;

        public async Task<bool> RequestPermissionAsync()
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.StorageRead>();
                }

                // On Android 13+, we might need specific media permissions
                if (DeviceInfo.Platform == DevicePlatform.Android && status != PermissionStatus.Granted)
                {
                    // For simplicity in this iteration, we assume basic storage read is enough or user grants it.
                    // In a real app, we'd handle Android 13+ specific media permissions (ReadMediaAudio).
                }

                _hasPermission = status == PermissionStatus.Granted;
                return _hasPermission;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Permission request failed: {ex.Message}");
                return false;
            }
        }

        public async Task<List<Song>> GetAllSongsAsync()
        {
            if (!_hasPermission)
            {
                if (!await RequestPermissionAsync())
                {
                    return new List<Song>();
                }
            }

            if (_cachedSongs.Any()) return _cachedSongs;

            await ScanMusicFolder();
            return _cachedSongs;
        }

        public async Task<List<Album>> GetAlbumsAsync()
        {
            if (!_cachedAlbums.Any()) await GetAllSongsAsync();
            return _cachedAlbums;
        }

        public async Task<List<Artist>> GetArtistsAsync()
        {
            if (!_cachedArtists.Any()) await GetAllSongsAsync();
            return _cachedArtists;
        }

        public Task<List<Playlist>> GetPlaylistsAsync()
        {
            // Playlists not implemented in file system scan yet
            return Task.FromResult(new List<Playlist>());
        }

        private async Task ScanMusicFolder()
        {
            try
            {
                var musicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

                // On Android, this might point to internal storage Music.
                // We can also try to scan common paths if needed, but let's start with the standard system folder.

                if (!Directory.Exists(musicPath))
                {
                    return;
                }

                var files = Directory.GetFiles(musicPath, "*.*", SearchOption.AllDirectories)
                    .Where(f => f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase) ||
                                f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase));

                _cachedSongs.Clear();
                _cachedAlbums.Clear();
                _cachedArtists.Clear();

                var albumsDict = new Dictionary<string, Album>();
                var artistsDict = new Dictionary<string, Artist>();

                foreach (var file in files)
                {
                    // Without TagLib#, we parse filename or use directory structure
                    // Format assumption: Artist - Album - Title.mp3 OR Artist/Album/Title.mp3

                    var fileInfo = new FileInfo(file);
                    var fileName = Path.GetFileNameWithoutExtension(fileInfo.Name);

                    string title = fileName;
                    string artistName = "Unknown Artist";
                    string albumName = "Unknown Album";

                    // Try to guess from directory structure: Music/Artist/Album/Song.mp3
                    var parentDir = fileInfo.Directory;
                    if (parentDir != null)
                    {
                        albumName = parentDir.Name;
                        if (parentDir.Parent != null && parentDir.Parent.FullName != musicPath)
                        {
                            artistName = parentDir.Parent.Name;
                        }
                    }

                    // Create Song
                    var song = new Song
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = title,
                        Artist = artistName,
                        Album = albumName,
                        FilePath = file,
                        Duration = TimeSpan.Zero // Cannot get without reading file headers
                    };
                    _cachedSongs.Add(song);

                    // Process Artist
                    if (!artistsDict.TryGetValue(artistName, out var artist))
                    {
                        artist = new Artist { Name = artistName };
                        artistsDict[artistName] = artist;
                        _cachedArtists.Add(artist);
                    }

                    // Process Album
                    var albumKey = $"{artistName}|{albumName}";
                    if (!albumsDict.TryGetValue(albumKey, out var album))
                    {
                        album = new Album { Title = albumName, Artist = artistName };
                        albumsDict[albumKey] = album;
                        _cachedAlbums.Add(album);

                        // Link album to artist
                        if (!artist.Albums.Contains(album))
                        {
                            artist.Albums.Add(album);
                        }
                    }

                    album.Songs.Add(song);

                    // Try to find cover art if not already found for this album
                    if (string.IsNullOrEmpty(album.CoverArtPath) && parentDir != null)
                    {
                        var coverExtensions = new[] { ".jpg", ".jpeg", ".png" };
                        var coverNames = new[] { "cover", "folder", "artwork", "front", "album" };

                        foreach (var ext in coverExtensions)
                        {
                            foreach (var name in coverNames)
                            {
                                var coverPath = Path.Combine(parentDir.FullName, name + ext);
                                if (File.Exists(coverPath))
                                {
                                    album.CoverArtPath = coverPath; // WebView might need file:// prefix or mapping
                                    break;
                                }
                            }
                            if (!string.IsNullOrEmpty(album.CoverArtPath)) break;
                        }

                        // If still not found, look for ANY image in the folder
                        if (string.IsNullOrEmpty(album.CoverArtPath))
                        {
                            var firstImage = Directory.GetFiles(parentDir.FullName, "*.*")
                                .FirstOrDefault(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                     f.EndsWith(".png", StringComparison.OrdinalIgnoreCase));

                            if (firstImage != null)
                            {
                                album.CoverArtPath = firstImage;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error scanning music: {ex.Message}");
            }

            await Task.CompletedTask;
        }
    }
}
