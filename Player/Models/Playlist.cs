using System;
using System.Collections.Generic;
using System.Text;

namespace Player.Models
{
    public class Playlist
    {
        public string Title { get; set; }
        public List<Song> Songs = new List<Song>();


    }
}
