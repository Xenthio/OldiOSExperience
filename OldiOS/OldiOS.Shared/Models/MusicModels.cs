using System;
using System.Collections.Generic;

namespace OldiOS.Shared.Models
{
    public class Song
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "Unknown Title";
        public string Artist { get; set; } = "Unknown Artist";
        public string Album { get; set; } = "Unknown Album";
        public string FilePath { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public string CoverArtPath { get; set; } = string.Empty; // Path to image file or base64
    }

    public class Album
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "Unknown Album";
        public string Artist { get; set; } = "Unknown Artist";
        public string CoverArtPath { get; set; } = string.Empty;
        public List<Song> Songs { get; set; } = new List<Song>();
    }

    public class Artist
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Unknown Artist";
        public List<Album> Albums { get; set; } = new List<Album>();
    }

    public class Playlist
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "New Playlist";
        public List<Song> Songs { get; set; } = new List<Song>();
    }
}
