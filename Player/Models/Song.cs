using System;
using System.Collections.Generic;
using System.Text;

namespace Player.Models
{
    public class Song : IEquatable<Song>
    {
        public bool IsChecked { get; set; }
        public string SongTitle { get; set; }
        public string SongArtist { get; set; }
        public string SongDate { get; set; }
        public string SongAlbum { get; set; }
        public string SongPath { get; set; }
        public string SongDuration { get; set; }
        public string SongGenre { get; set; }
        public string SongArt { get; set; }


        public bool Equals(Song other)
        {
            if (other == null) return false;
            return (this.SongTitle.Equals(other.SongTitle));
        }
    }
}
