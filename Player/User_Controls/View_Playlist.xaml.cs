using Player.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using System.Linq;

namespace Player.User_Controls
{
    /// <summary>
    /// Interakční logika pro View_Playlist.xaml
    /// </summary>
    /// 


    public partial class View_Playlist : UserControl
    {
        public Song song = new Song();
        public List<Song> ToPlay = new List<Song>();
        public string Playlist_name;
        public List<Song> Songs_to_add = new List<Song>();
        public List<Playlist> PL = new List<Playlist>();

        private string Tittle;
        private bool Titlesorted = false;
        private bool Artistsorted = false;
        private bool Albumsorted = false;
        private bool Yearsorted = false;
        public bool Need_save = false;

        public View_Playlist(List<Song> Play, string title, string autors, List<Playlist> playlists)
        {
            InitializeComponent();

            ToPlay = Play;
            PL = playlists;
            Tittle = title;

            ToPlayList.ItemsSource = ToPlay;
            ToPlayList.Items.Refresh();
            Title.Text = title;
            Autors.Text = autors;
            if (playlists != null)
            {
                foreach (Playlist x in playlists)
                {
                    MenuItem mia = new MenuItem();
                    mia.Header = x.Title;
                    mia.Click += Add_to_playlist;
                    cm.Items.Add(mia);
                }
            }
        }

        public event EventHandler ButtonClick;

        private void ToPlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = ToPlayList.Items.IndexOf(ToPlayList.SelectedItem);
            if (i >= 0)
                song = ToPlay[i];

            if (ButtonClick != null)
            {
                ButtonClick(this, e);
            }
        }

        private static Random rng = new Random();

        public void Shuffle()
        {
            int n = ToPlay.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Song value = ToPlay[k];
                ToPlay[k] = ToPlay[n];
                ToPlay[n] = value;
            }
            ToPlayList.ItemsSource = ToPlay;
            ToPlayList.Items.Refresh();
            ToPlayList.ScrollIntoView(ToPlayList.SelectedItem);
        }


        //mělo by se to řadit na klik, ale místo toho to jde na double click


        private void Title_Sort(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Titlesorted == false)
                {
                    ToPlay = ToPlay.OrderBy(o => o.SongTitle).ToList();
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();

                    Titlesorted = true;
                    Yearsorted = true;
                    Albumsorted = false;
                    Artistsorted = false;
                }
                else
                {
                    ToPlay.Reverse();
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();

                    Titlesorted = false;
                }
            }
            catch
            {

            }
        }

        private void Artist_Sort(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Artistsorted == false)
                {
                    ToPlay = ToPlay.OrderBy(o => o.SongArtist).ToList();
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();

                    Artistsorted = true;
                    Yearsorted = true;
                    Albumsorted = false;
                    Titlesorted = false;
                }
                else
                {
                    ToPlay.Reverse();
                    Artistsorted = false;
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();
                }
            }
            catch
            {

            }
        }

        private void Album_Sort(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Albumsorted == false)
                {
                    ToPlay = ToPlay.OrderBy(o => o.SongAlbum).ToList();
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();

                    Albumsorted = true;
                    Yearsorted = true;
                    Artistsorted = false;
                    Titlesorted = false;
                }
                else
                {
                    ToPlay.Reverse();
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();

                    Albumsorted = false;
                }
            }
            catch
            {

            }
        }

        private void Year_Sort(object sender, RoutedEventArgs e)
        {
            try 
            {
                if (Yearsorted == false)
                {
                    ToPlay = ToPlay.OrderBy(o => o.SongDate).ToList();
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();

                    Yearsorted = true;
                    Albumsorted = false;
                    Artistsorted = false;
                    Titlesorted = false;
                }
                else
                {
                    ToPlay.Reverse();
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();

                    Yearsorted = false;
                }
            }
            catch
            {

            }
        }

        private void Song_Checked(object sender, RoutedEventArgs e)
        {
            var check = ((List<Song>)(ToPlayList.ItemsSource)).Where(x => x.IsChecked).ToList();
            Title.Text = check[0].SongArtist;
        }

        private void Song_Unchecked(object sender, RoutedEventArgs e)
        {
            Title.Text = "dsss";
        }

        private void Delete_Clicked(object sender, RoutedEventArgs e)
        {
            var i = ToPlayList.Items.IndexOf(ToPlayList.SelectedItem);

            if (ToPlay.Where(n => n.IsChecked == true).ToList().Count > 0)
            {
                foreach (var s in ToPlay.Where(n => n.IsChecked == true).ToList<Song>())
                {
                    PL.Find(x => x.Title == Tittle).Songs.Remove(s);
                    ToPlay = PL.Find(x => x.Title == Tittle).Songs;
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();
                }
            }
            else
            {
                if (i >= 0)
                {
                    ToPlayList.SelectedItem = ToPlayList.Items[0];
                    ToPlay.RemoveAt(i);
                    ToPlayList.ItemsSource = ToPlay;
                    ToPlayList.Items.Refresh();
                }
            }
            UnCheck();
        }
        private void Add_to_playlist(object sender, RoutedEventArgs e)
        {
            Songs_to_add = null;
            string y = string.Empty;
            MenuItem val = (MenuItem)sender;
            y = val.Header.ToString();

            var i = ToPlayList.Items.IndexOf(ToPlayList.SelectedItem);

            if (ToPlay.Where(n => n.IsChecked == true).ToList().Count > 0)
            {
                foreach (var s in ToPlay.Where(n => n.IsChecked == true).ToList<Song>())
                {
                    PL.Find(x => x.Title == y).Songs.Add(s);
                }
            }
            else
            {
                if (i >= 0)
                {
                    PL.Find(x => x.Title == y).Songs.Add(ToPlay[i]);
                }
            }
            Need_save = true;
            UnCheck();
        }
        private void UnCheck()
        {
            foreach(var s in ToPlay)
            {
                s.IsChecked = false;
            }
            ToPlayList.ItemsSource = ToPlay;
            ToPlayList.Items.Refresh();
        }
    }
}
