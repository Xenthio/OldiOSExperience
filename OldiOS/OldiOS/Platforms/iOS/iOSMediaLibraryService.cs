using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundation;
using MediaPlayer;
using OldiOS.Shared.Models;
using OldiOS.Shared.Services;

namespace OldiOS.Services.Platforms.iOS
{
    /// <summary>
    /// iOS-specific implementation of IMediaLibraryService using the MediaPlayer framework
    /// </summary>
    public class iOSMediaLibraryService : IMediaLibraryService
    {
        private bool _hasPermission = false;
        private List<Song> _cachedSongs = new();
        private List<Album> _cachedAlbums = new();
        private List<Artist> _cachedArtists = new();
        private List<Playlist> _cachedPlaylists = new();

        public bool HasPermission => _hasPermission;

        public async Task<bool> RequestPermissionAsync()
        {
            var tcs = new TaskCompletionSource<bool>();

            MPMediaLibrary.RequestAuthorization((status) =>
            {
                _hasPermission = status == MPMediaLibraryAuthorizationStatus.Authorized;
                tcs.SetResult(_hasPermission);
            });

            return await tcs.Task;
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

            // MediaPlayer queries must run on the main thread
            var items = await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(() =>
            {
                var query = MPMediaQuery.SongsQuery;
                return query.Items;
            });

            if (items == null || items.Length == 0)
            {
                return _cachedSongs;
            }

            // Process items on background thread
            await Task.Run(() =>
            {
                foreach (var item in items)
                {
                    var song = ConvertToSong(item);
                    if (song != null)
                    {
                        _cachedSongs.Add(song);
                    }
                }
            });

            return _cachedSongs;
        }

        public async Task<List<Album>> GetAlbumsAsync()
        {
            if (!_hasPermission)
            {
                if (!await RequestPermissionAsync())
                {
                    return new List<Album>();
                }
            }

            if (_cachedAlbums.Any()) return _cachedAlbums;

            // MediaPlayer queries must run on the main thread
            var collections = await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(() =>
            {
                var query = MPMediaQuery.AlbumsQuery;
                return query.Collections;
            });

            if (collections == null || collections.Length == 0)
            {
                return _cachedAlbums;
            }

            // Process collections on background thread
            await Task.Run(() =>
            {
                foreach (var collection in collections)
                {
                    var album = ConvertToAlbum(collection);
                    if (album != null)
                    {
                        _cachedAlbums.Add(album);
                    }
                }
            });

            return _cachedAlbums;
        }

        public async Task<List<Artist>> GetArtistsAsync()
        {
            if (!_hasPermission)
            {
                if (!await RequestPermissionAsync())
                {
                    return new List<Artist>();
                }
            }

            if (_cachedArtists.Any()) return _cachedArtists;

            // MediaPlayer queries must run on the main thread
            var collections = await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(() =>
            {
                var query = MPMediaQuery.ArtistsQuery;
                return query.Collections;
            });

            if (collections == null || collections.Length == 0)
            {
                return _cachedArtists;
            }

            // Process collections on background thread
            await Task.Run(() =>
            {
                foreach (var collection in collections)
                {
                    var artist = ConvertToArtist(collection);
                    if (artist != null)
                    {
                        _cachedArtists.Add(artist);
                    }
                }
            });

            return _cachedArtists;
        }

        public async Task<List<Playlist>> GetPlaylistsAsync()
        {
            if (!_hasPermission)
            {
                if (!await RequestPermissionAsync())
                {
                    return new List<Playlist>();
                }
            }

            if (_cachedPlaylists.Any()) return _cachedPlaylists;

            // MediaPlayer queries must run on the main thread
            var collections = await Microsoft.Maui.ApplicationModel.MainThread.InvokeOnMainThreadAsync(() =>
            {
                var query = MPMediaQuery.PlaylistsQuery;
                return query.Collections;
            });

            if (collections == null || collections.Length == 0)
            {
                return _cachedPlaylists;
            }

            // Process collections on background thread
            await Task.Run(() =>
            {
                foreach (var collection in collections)
                {
                    if (collection is MPMediaPlaylist playlist)
                    {
                        var pl = ConvertToPlaylist(playlist);
                        if (pl != null)
                        {
                            _cachedPlaylists.Add(pl);
                        }
                    }
                }
            });

            return _cachedPlaylists;
        }

        private Song? ConvertToSong(MPMediaItem item)
        {
            try
            {
                var title = item.Title ?? "Unknown Title";
                var artist = item.Artist ?? "Unknown Artist";
                var album = item.AlbumTitle ?? "Unknown Album";
                var duration = TimeSpan.FromSeconds(item.PlaybackDuration);

                var song = new Song
                {
                    Title = title,
                    Artist = artist,
                    Album = album,
                    Duration = duration
                };

                // Get album artwork if available
                var artwork = item.Artwork;
                if (artwork != null)
                {
                    // Store a reference to get the artwork later
                    // We'll use the persistent ID as a key to retrieve artwork
                    song.CoverArtPath = $"ios-media://{item.PersistentID}";
                }

                return song;
            }
            catch
            {
                return null;
            }
        }

        private Album? ConvertToAlbum(MPMediaItemCollection collection)
        {
            try
            {
                var representativeItem = collection.RepresentativeItem;
                if (representativeItem == null) return null;

                var album = new Album
                {
                    Title = representativeItem.AlbumTitle ?? "Unknown Album",
                    Artist = representativeItem.AlbumArtist ?? representativeItem.Artist ?? "Unknown Artist"
                };

                // Get album artwork
                var artwork = representativeItem.Artwork;
                if (artwork != null)
                {
                    album.CoverArtPath = $"ios-media://{representativeItem.AlbumPersistentID}";
                }

                // Add songs from this album
                var items = collection.Items;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var song = ConvertToSong(item);
                        if (song != null)
                        {
                            album.Songs.Add(song);
                        }
                    }
                }

                return album;
            }
            catch
            {
                return null;
            }
        }

        private Artist? ConvertToArtist(MPMediaItemCollection collection)
        {
            try
            {
                var representativeItem = collection.RepresentativeItem;
                if (representativeItem == null) return null;

                var artist = new Artist
                {
                    Name = representativeItem.Artist ?? "Unknown Artist"
                };

                // Group songs by album
                var items = collection.Items;
                if (items != null)
                {
                    var albumGroups = items.GroupBy(i => i.AlbumPersistentID);
                    foreach (var albumGroup in albumGroups)
                    {
                        var albumCollection = new MPMediaItemCollection(albumGroup.ToArray());
                        var album = ConvertToAlbum(albumCollection);
                        if (album != null)
                        {
                            artist.Albums.Add(album);
                        }
                    }
                }

                return artist;
            }
            catch
            {
                return null;
            }
        }

        private Playlist? ConvertToPlaylist(MPMediaPlaylist playlist)
        {
            try
            {
                var pl = new Playlist
                {
                    Name = playlist.Name ?? "Unknown Playlist"
                };

                var items = playlist.Items;
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        var song = ConvertToSong(item);
                        if (song != null)
                        {
                            pl.Songs.Add(song);
                        }
                    }
                }

                return pl;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the album artwork image for a given iOS media item ID
        /// </summary>
        public static UIKit.UIImage? GetArtworkImage(ulong persistentId, nfloat size = default(nfloat))
        {
            try
            {
                if (size == default(nfloat))
                    size = new nfloat(180);

                var predicate = MPMediaPropertyPredicate.PredicateWithValue(
                    NSNumber.FromUInt64(persistentId),
                    MPMediaItem.PersistentIDProperty);

                var query = new MPMediaQuery();
                query.AddFilterPredicate(predicate);

                var items = query.Items;
                if (items != null && items.Length > 0)
                {
                    var artwork = items[0].Artwork;
                    if (artwork != null)
                    {
                        return artwork.ImageWithSize(new CoreGraphics.CGSize(size, size));
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
