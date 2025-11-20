using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OldiOS.Shared.Models;
using OldiOS.Shared.Services;

namespace OldiOS.Web
{
    /// <summary>
    /// Web implementation of media library service with mock data.
    /// Provides sample music library for demonstration purposes.
    /// </summary>
    public class WebMediaLibraryService : IMediaLibraryService
    {
        private List<Song> _songs = null!;
        private List<Album> _albums = null!;
        private List<Artist> _artists = null!;
        private List<Playlist> _playlists = null!;

        public bool HasPermission => true; // Web doesn't need storage permission for mock data

        public WebMediaLibraryService()
        {
            InitializeMockData();
        }

        public Task<bool> RequestPermissionAsync()
        {
            return Task.FromResult(true);
        }

        public Task<List<Song>> GetAllSongsAsync()
        {
            return Task.FromResult(_songs);
        }

        public Task<List<Album>> GetAlbumsAsync()
        {
            return Task.FromResult(_albums);
        }

        public Task<List<Artist>> GetArtistsAsync()
        {
            return Task.FromResult(_artists);
        }

        public Task<List<Playlist>> GetPlaylistsAsync()
        {
            return Task.FromResult(_playlists);
        }

        private void InitializeMockData()
        {
            _songs = new List<Song>();
            _albums = new List<Album>();
            _artists = new List<Artist>();
            _playlists = new List<Playlist>();

            // Create mock artists
            var beatles = new Artist { Name = "The Beatles" };
            var pinkFloyd = new Artist { Name = "Pink Floyd" };
            var queen = new Artist { Name = "Queen" };
            var ledZeppelin = new Artist { Name = "Led Zeppelin" };
            var theRollingStones = new Artist { Name = "The Rolling Stones" };

            _artists.AddRange(new[] { beatles, pinkFloyd, queen, ledZeppelin, theRollingStones });

            // The Beatles - Abbey Road
            var abbeyRoad = new Album
            {
                Title = "Abbey Road",
                Artist = "The Beatles",
                CoverArtPath = "_content/OldiOS.Shared/images/albums/abbey-road.jpg"
            };
            abbeyRoad.Songs.AddRange(new[]
            {
                new Song { Title = "Come Together", Artist = "The Beatles", Album = "Abbey Road", Duration = TimeSpan.FromSeconds(259) },
                new Song { Title = "Something", Artist = "The Beatles", Album = "Abbey Road", Duration = TimeSpan.FromSeconds(182) },
                new Song { Title = "Maxwell's Silver Hammer", Artist = "The Beatles", Album = "Abbey Road", Duration = TimeSpan.FromSeconds(207) },
                new Song { Title = "Oh! Darling", Artist = "The Beatles", Album = "Abbey Road", Duration = TimeSpan.FromSeconds(206) },
                new Song { Title = "Octopus's Garden", Artist = "The Beatles", Album = "Abbey Road", Duration = TimeSpan.FromSeconds(171) },
                new Song { Title = "I Want You (She's So Heavy)", Artist = "The Beatles", Album = "Abbey Road", Duration = TimeSpan.FromSeconds(467) },
                new Song { Title = "Here Comes the Sun", Artist = "The Beatles", Album = "Abbey Road", Duration = TimeSpan.FromSeconds(185) },
            });

            // Pink Floyd - The Dark Side of the Moon
            var darkSide = new Album
            {
                Title = "The Dark Side of the Moon",
                Artist = "Pink Floyd",
                CoverArtPath = "_content/OldiOS.Shared/images/albums/dark-side.jpg"
            };
            darkSide.Songs.AddRange(new[]
            {
                new Song { Title = "Speak to Me", Artist = "Pink Floyd", Album = "The Dark Side of the Moon", Duration = TimeSpan.FromSeconds(90) },
                new Song { Title = "Breathe", Artist = "Pink Floyd", Album = "The Dark Side of the Moon", Duration = TimeSpan.FromSeconds(163) },
                new Song { Title = "On the Run", Artist = "Pink Floyd", Album = "The Dark Side of the Moon", Duration = TimeSpan.FromSeconds(216) },
                new Song { Title = "Time", Artist = "Pink Floyd", Album = "The Dark Side of the Moon", Duration = TimeSpan.FromSeconds(413) },
                new Song { Title = "The Great Gig in the Sky", Artist = "Pink Floyd", Album = "The Dark Side of the Moon", Duration = TimeSpan.FromSeconds(276) },
                new Song { Title = "Money", Artist = "Pink Floyd", Album = "The Dark Side of the Moon", Duration = TimeSpan.FromSeconds(382) },
                new Song { Title = "Us and Them", Artist = "Pink Floyd", Album = "The Dark Side of the Moon", Duration = TimeSpan.FromSeconds(462) },
            });

            // Queen - A Night at the Opera
            var nightOpera = new Album
            {
                Title = "A Night at the Opera",
                Artist = "Queen",
                CoverArtPath = "_content/OldiOS.Shared/images/albums/night-opera.jpg"
            };
            nightOpera.Songs.AddRange(new[]
            {
                new Song { Title = "Death on Two Legs", Artist = "Queen", Album = "A Night at the Opera", Duration = TimeSpan.FromSeconds(223) },
                new Song { Title = "Lazing on a Sunday Afternoon", Artist = "Queen", Album = "A Night at the Opera", Duration = TimeSpan.FromSeconds(70) },
                new Song { Title = "I'm in Love with My Car", Artist = "Queen", Album = "A Night at the Opera", Duration = TimeSpan.FromSeconds(193) },
                new Song { Title = "You're My Best Friend", Artist = "Queen", Album = "A Night at the Opera", Duration = TimeSpan.FromSeconds(170) },
                new Song { Title = "'39", Artist = "Queen", Album = "A Night at the Opera", Duration = TimeSpan.FromSeconds(210) },
                new Song { Title = "Sweet Lady", Artist = "Queen", Album = "A Night at the Opera", Duration = TimeSpan.FromSeconds(244) },
                new Song { Title = "Bohemian Rhapsody", Artist = "Queen", Album = "A Night at the Opera", Duration = TimeSpan.FromSeconds(355) },
            });

            // Led Zeppelin - Led Zeppelin IV
            var lzIV = new Album
            {
                Title = "Led Zeppelin IV",
                Artist = "Led Zeppelin",
                CoverArtPath = "_content/OldiOS.Shared/images/albums/lz-iv.jpg"
            };
            lzIV.Songs.AddRange(new[]
            {
                new Song { Title = "Black Dog", Artist = "Led Zeppelin", Album = "Led Zeppelin IV", Duration = TimeSpan.FromSeconds(296) },
                new Song { Title = "Rock and Roll", Artist = "Led Zeppelin", Album = "Led Zeppelin IV", Duration = TimeSpan.FromSeconds(220) },
                new Song { Title = "The Battle of Evermore", Artist = "Led Zeppelin", Album = "Led Zeppelin IV", Duration = TimeSpan.FromSeconds(351) },
                new Song { Title = "Stairway to Heaven", Artist = "Led Zeppelin", Album = "Led Zeppelin IV", Duration = TimeSpan.FromSeconds(482) },
                new Song { Title = "Misty Mountain Hop", Artist = "Led Zeppelin", Album = "Led Zeppelin IV", Duration = TimeSpan.FromSeconds(278) },
                new Song { Title = "Four Sticks", Artist = "Led Zeppelin", Album = "Led Zeppelin IV", Duration = TimeSpan.FromSeconds(284) },
            });

            // The Rolling Stones - Sticky Fingers
            var stickyFingers = new Album
            {
                Title = "Sticky Fingers",
                Artist = "The Rolling Stones",
                CoverArtPath = "_content/OldiOS.Shared/images/albums/sticky-fingers.jpg"
            };
            stickyFingers.Songs.AddRange(new[]
            {
                new Song { Title = "Brown Sugar", Artist = "The Rolling Stones", Album = "Sticky Fingers", Duration = TimeSpan.FromSeconds(229) },
                new Song { Title = "Sway", Artist = "The Rolling Stones", Album = "Sticky Fingers", Duration = TimeSpan.FromSeconds(231) },
                new Song { Title = "Wild Horses", Artist = "The Rolling Stones", Album = "Sticky Fingers", Duration = TimeSpan.FromSeconds(341) },
                new Song { Title = "Can't You Hear Me Knocking", Artist = "The Rolling Stones", Album = "Sticky Fingers", Duration = TimeSpan.FromSeconds(439) },
                new Song { Title = "Dead Flowers", Artist = "The Rolling Stones", Album = "Sticky Fingers", Duration = TimeSpan.FromSeconds(244) },
            });

            // Add albums to list
            _albums.AddRange(new[] { abbeyRoad, darkSide, nightOpera, lzIV, stickyFingers });

            // Link albums to artists
            beatles.Albums.Add(abbeyRoad);
            pinkFloyd.Albums.Add(darkSide);
            queen.Albums.Add(nightOpera);
            ledZeppelin.Albums.Add(lzIV);
            theRollingStones.Albums.Add(stickyFingers);

            // Collect all songs
            foreach (var album in _albums)
            {
                foreach (var song in album.Songs)
                {
                    song.CoverArtPath = album.CoverArtPath;
                    _songs.Add(song);
                }
            }

            // Create sample playlists
            var favoritesPlaylist = new Playlist
            {
                Name = "Favorites"
            };
            favoritesPlaylist.Songs.AddRange(new[]
            {
                _songs.First(s => s.Title == "Come Together"),
                _songs.First(s => s.Title == "Bohemian Rhapsody"),
                _songs.First(s => s.Title == "Stairway to Heaven"),
                _songs.First(s => s.Title == "Money"),
                _songs.First(s => s.Title == "Here Comes the Sun"),
            });

            var rockClassicsPlaylist = new Playlist
            {
                Name = "Rock Classics"
            };
            rockClassicsPlaylist.Songs.AddRange(new[]
            {
                _songs.First(s => s.Title == "Black Dog"),
                _songs.First(s => s.Title == "Rock and Roll"),
                _songs.First(s => s.Title == "Brown Sugar"),
            });

            _playlists.AddRange(new[] { favoritesPlaylist, rockClassicsPlaylist });
        }
    }
}
