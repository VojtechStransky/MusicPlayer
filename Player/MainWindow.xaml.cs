using Player.Models;
using Player.User_Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using TagLib;

namespace Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ///              1. obrázek z písničny na stop-play tlačítku a měnící se piktogram čarky/trojúhelník
        ///              
        ///              2. udělat zálohu a zkusit ovládání ze sluchátek s tou knihovnou
        /// 
        ///              3. design    na all playlist brát ignorovat složky, aby nebyl error
        
        string x, y;
        List<Song> z;

        public MediaPlayer mediaPlayer = new MediaPlayer();
        
        public List<Playlist> Playlists = new List<Playlist>();
        private List<Song> SongsList = new List<Song>();


        Data data = new Data();

        Setting setting;
        private UserControl1 Player_element = new UserControl1();
        private Add_Playlist add_Playlist = new Add_Playlist();
        View_Playlist view_Playlist;

        private string Patth;

        bool IsPlaying = false;
        bool IsMuted = false;
        bool userIsDraggingSlider = false;
        bool Seeded = false;

        public MainWindow()
        {
            
            InitializeComponent();

            
            //zobrazeni prehravace a playlistu a změna trvání hudby, se vybere
            this.contentControlPlayer.Content = Player_element;
            mediaPlayer.MediaOpened += Text;

            //okolo načtení ze souboru a vybrání "vše" playlistu
            Load();
            Playlists_ListBox.ItemsSource = Playlists;
            Playlists_ListBox.SelectedItem = Playlists_ListBox.Items[0];

            setting = new Setting(data.Vol, data.Sources);
            mediaPlayer.Volume = data.Vol;

            //když se zavírá zavolá na uložení do xml
            this.Closed += new EventHandler(MainWindow_Closed);

            //věci k nastavování hlasitosti
            setting.sliVolume.Minimum = 0;
            setting.sliVolume.Maximum = 1;

            setting.ValueChanged += (Sender, e) =>
            {
                mediaPlayer.Volume = setting.sliVolume.Value;
            };

            add_Playlist.AddButtonClick += (Sender, e) =>
            {
                if (add_Playlist.Nname != "") 
                {
                    Playlists.Add(new Playlist() { Title = add_Playlist.Nname });
                    add_Playlist.Naame.Text = "";
                    Playlists_ListBox.ItemsSource = Playlists;
                    Playlists_ListBox.Items.Refresh();
                    Playlists_ListBox.SelectedItem = Playlists_ListBox.Items[Playlists.Count - 1];
                    Playlists_ListBox.ScrollIntoView(Playlists_ListBox.SelectedItem);
                }
            };
        }

        //změna nápisu délky celé skladby
        private void Text(object sender, EventArgs e)
        {
            try
            {
                Player_element.ToEnd.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            }
            catch { }
        }

        //Context menu - vymazat
        private void Delete_Clicked(object sender, RoutedEventArgs e)
        {
            var i = Playlists_ListBox.Items.IndexOf(Playlists_ListBox.SelectedItem);
            if (i >= 1)
            {
                Playlists_ListBox.SelectedItem = Playlists_ListBox.Items[0];
                Playlists.RemoveAt(i);
                Playlists_ListBox.ItemsSource = Playlists;
                Playlists_ListBox.Items.Refresh();
            }
        }

        //inicializace tlačítek na přidání playlistu a nastavení
        private void NewPlaylist_button_Click(object sender, RoutedEventArgs e)
        {
            this.contentControl.Content = add_Playlist;
        }

        private void Settings_button_Click(object sender, RoutedEventArgs e)
        {
            this.contentControl.Content = setting;
        }

        //Vytvoření all playlistu
        private void CreateAllPlaylist()
        {
            string Artist = "";
            SongsList.Clear();
            
            foreach (string Sourcefolder in setting.SourceFolders)
            {
                try
                {
                    DirectoryInfo d = new DirectoryInfo(Sourcefolder);
                    FileInfo[] Files = d.GetFiles("*.mp3");
                    foreach (var file in Files)
                    {
                        string Sfile = file.ToString();
                        try
                        {
                            TagLib.File tagFile = TagLib.File.Create(Sfile);

                            try
                            {
                                Artist = tagFile.Tag.FirstArtist;
                            }
                            catch
                            {
                                try
                                {
                                    Artist = tagFile.Tag.FirstAlbumArtist;
                                }
                                catch
                                {

                                }
                            }

                            SongsList.Add(new Song()
                            {
                                SongPath = Sfile,
                                SongArtist = Artist,
                                SongDuration = tagFile.Properties.Duration.ToString(@"mm\:ss"),
                                SongAlbum = tagFile.Tag.Album,
                                SongTitle = tagFile.Tag.Title,
                                SongDate = tagFile.Tag.Year.ToString(),
                                SongGenre = tagFile.Tag.FirstGenre
                            });
                        }
                        catch
                        {

                        }
                    }
                }
                catch { }
            }
            var Item = Playlists.Find(r => r.Title == "Vše");
            Item.Songs = SongsList;
        }
        
        //okolo výběru playlistu
        private void ListBoxItem_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.contentControl.Content = view_Playlist;
            if (setting.Changed == true)
            {
                mediaPlayer.Close();
                CreateAllPlaylist();
                setting.Changed = false;
                view_Playlist_prepare();
            }
        }
        private void Selection_Changed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (view_Playlist.Need_save == true)
                {
                    Playlists = view_Playlist.PL;
                }
            }
            catch { }

            int i = Playlists_ListBox.Items.IndexOf(Playlists_ListBox.SelectedItem);
            
            if (Playlists[i].Title != "Podle autorů")   //tady případně změnit jeméno toho pochybnýho playlistu, co nebude playlist
            {
                view_Playlist_prepare();

                Player_element.RightToLeftMarquee();

                if (Seeded == false)
                {
                    Seed_Events();
                }
            }
            else
            {
                //když to bude záložka s autorama - musí se udělat novej usercontrol a věchno s tím
            }
            if (setting.Changed == true)
            {
                CreateAllPlaylist();
                setting.Changed = false;
                view_Playlist_prepare();
            }
            
        }

        //načíst vybraný playlist do viewplaylistu a zobrazit
        private void view_Playlist_prepare()
        {
            view_Playlist = new View_Playlist(null, null, null, null);
            x = "";
            var i = Playlists_ListBox.Items.IndexOf(Playlists_ListBox.SelectedItem);
            
            y = Playlists[i].Title;
            z = Playlists[i].Songs;
            

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);

            if (z.Count > 0)
            {

                foreach (Song song in z)
                {
                    if (x.Contains(song.SongArtist) == false)
                    {
                        x += (song.SongArtist + ", ");
                    }
                }
            }

            View_Playlist view_Playlistt = new View_Playlist(z, y, x, Playlists);
            view_Playlist = view_Playlistt;
            this.contentControl.Content = view_Playlist;

            timer.Tick += timer_Tick;
            timer.Start();

            view_Playlist.ButtonClick += (Sender, e) =>
            {
                mediaPlayer.Open(new Uri(view_Playlist.song.SongPath));
                Patth = view_Playlist.song.SongPath;
                mediaPlayer.Play();

                mediaPlayer.Play();
                IsPlaying = true;
                BitmapImage bi4 = new BitmapImage();
                bi4.BeginInit();
                bi4.UriSource = new Uri("Stop.png", UriKind.Relative);
                bi4.EndInit();
                Player_element.image.Source = bi4;
                this.contentControlPlayer.Content = Player_element;

                try
                {
                    TagLib.File tagFile = TagLib.File.Create(Patth);
                    if (tagFile.Tag.Pictures.Length >= 1)
                    {
                        var bin = (byte[])(tagFile.Tag.Pictures[0].Data.Data);
                        var c = Image.FromStream(new MemoryStream(bin));
                        BitmapImage d;

                        using (var ms = new MemoryStream())
                        {
                            c.Save(ms, ImageFormat.Bmp);
                            ms.Seek(0, SeekOrigin.Begin);

                            var bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = ms;
                            bitmapImage.EndInit();

                            d = bitmapImage;
                        }

                        ImageBrush ib = new ImageBrush();

                        ib.ImageSource = d;

                        Player_element.Stop_button.Background = ib;
                        this.contentControlPlayer.Content = Player_element;
                    }
                }
                catch { }

                Player_element.Title.Text = view_Playlist.song.SongTitle;
                Player_element.RightToLeftMarquee();
                IsPlaying = true;
                mediaPlayer.MediaEnded += (sender, eventArgs) =>
                {
                    int i = view_Playlist.ToPlayList.SelectedIndex + 1;
                    try
                    {
                        view_Playlist.ToPlayList.SelectedItem = view_Playlist.ToPlayList.Items[i];
                    }
                    catch
                    {
                        view_Playlist.ToPlayList.SelectedItem = view_Playlist.ToPlayList.Items[0];
                    }

                    view_Playlist.ToPlayList.ScrollIntoView(view_Playlist.ToPlayList.SelectedItem);
                };
                Thread.Sleep(30);
            };

            void timer_Tick(object sender, EventArgs e)
            {
                if ((mediaPlayer.Source != null) && (mediaPlayer.NaturalDuration.HasTimeSpan) && (!userIsDraggingSlider))
                {
                    Player_element.sliProgress.Minimum = 0;
                    Player_element.sliProgress.Maximum = mediaPlayer.NaturalDuration.TimeSpan.TotalSeconds;
                    Player_element.sliProgress.Value = mediaPlayer.Position.TotalSeconds;
                }
            }
        }

        //inicializace ovládání přehravače
        private void Seed_Events()
        {
            var brush = new ImageBrush();
            //brush.ImageSource = new BitmapImage(new Uri(""));
            //Player_element.Stop_button.Background = brush;

            Player_element.StopButtonClick += (Sender, e) =>
            {
                if (IsPlaying == true)
                {
                    mediaPlayer.Pause();
                    IsPlaying = false;
                    BitmapImage bi3 = new BitmapImage();

                    bi3.BeginInit();
                    bi3.UriSource = new Uri("Play.png", UriKind.Relative);
                    bi3.EndInit();
                    Player_element.image.Source = bi3;
                    this.contentControlPlayer.Content = Player_element;
                }
                else
                {
                    mediaPlayer.Play();
                    IsPlaying = true;
                    BitmapImage bi4 = new BitmapImage();
                    bi4.BeginInit();
                    bi4.UriSource = new Uri("Stop.png", UriKind.Relative);
                    bi4.EndInit();
                    Player_element.image.Source = bi4;
                    this.contentControlPlayer.Content = Player_element;
                }
            };

            Player_element.PreviousButtonClick += (Sender, e) =>
            {
                int i = view_Playlist.ToPlayList.SelectedIndex - 1;
                try
                {
                    view_Playlist.ToPlayList.SelectedItem = view_Playlist.ToPlayList.Items[i];
                }
                catch
                {
                    view_Playlist.ToPlayList.SelectedItem = view_Playlist.ToPlayList.Items[view_Playlist.ToPlayList.Items.Count - 1];
                }
                view_Playlist.ToPlayList.ScrollIntoView(view_Playlist.ToPlayList.SelectedItem);
            };
            Player_element.NextButtonClick += (Sender, e) =>
            {
                int i = view_Playlist.ToPlayList.SelectedIndex + 1;
                try
                {
                    view_Playlist.ToPlayList.SelectedItem = view_Playlist.ToPlayList.Items[i];
                }
                catch
                {
                    view_Playlist.ToPlayList.SelectedItem = view_Playlist.ToPlayList.Items[0];
                }
                view_Playlist.ToPlayList.ScrollIntoView(view_Playlist.ToPlayList.SelectedItem);
            };
            Player_element.MuteButtonClick += (Sender, e) =>
            {
                if (IsMuted == false)
                {
                    mediaPlayer.Volume = 0;
                    IsMuted = true;
                    BitmapImage bi4 = new BitmapImage();
                    bi4.BeginInit();
                    bi4.UriSource = new Uri("Down.png", UriKind.Relative);
                    bi4.EndInit();
                    Player_element.image3.Source = bi4;
                }
                else
                {
                    mediaPlayer.Volume = setting.sliVolume.Value;
                    IsMuted = false;
                    BitmapImage bi4 = new BitmapImage();
                    bi4.BeginInit();
                    bi4.UriSource = new Uri("Up.png", UriKind.Relative);
                    bi4.EndInit();
                    Player_element.image3.Source = bi4;
                }
            };
            Player_element.ShuffleButtonClick += (Sender, e) =>
            {
                view_Playlist.Shuffle();
            };

            Player_element.DragStarted += (Sender, e) =>
            {
                userIsDraggingSlider = true;
            };

            Player_element.DragCompleted += (Sender, e) =>
            {
                userIsDraggingSlider = false;
                mediaPlayer.Position = TimeSpan.FromSeconds(Player_element.sliProgress.Value);
            };

            Player_element.ValueChanged += (Sender, e) =>
            {
                Player_element.FromStart.Text = TimeSpan.FromSeconds(Player_element.sliProgress.Value).ToString(@"mm\:ss");
            };            

            Seeded = true;
        }

        //load a save do xml
        private void Load()
        {
            try
            {
                string file = @"PLdata.xml";
                XmlSerializer formatter = new XmlSerializer(data.GetType());
                FileStream aFile = new FileStream(file, FileMode.Open);
                byte[] buffer = new byte[aFile.Length];
                aFile.Read(buffer, 0, (int)aFile.Length);
                MemoryStream stream = new MemoryStream(buffer);
                data = (Data)formatter.Deserialize(stream);

                Playlists = data.PL;

                setting = new Setting(data.Vol, data.Sources);
                }
            catch
            {
                MessageBox.Show("Nebylo možno načíst hudbu. Nejspíše došlo k poškození skladovacího dokumentu.");
            }
        }

        private void Save()
        {
            Data data = new Data() { Vol = setting.sliVolume.Value, Sources = setting.SourceFolders, PL = Playlists };
            string path = @"PLdata.xml";
            FileStream outFile = System.IO.File.Create(path);
            XmlSerializer formatter = new XmlSerializer(data.GetType());
            formatter.Serialize(outFile, data);
        }

        //při zavření okna
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Save();
        }
    }
}
