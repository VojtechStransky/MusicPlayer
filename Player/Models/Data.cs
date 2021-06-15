using System;
using System.Collections.Generic;
using System.Text;

namespace Player.Models
{
    public class Data
    {
        public double Vol { get; set; }
        public List<Playlist> PL = new List<Playlist>();
        public List<string> Sources = new List<string>();
    }
}
