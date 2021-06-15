using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Player.User_Controls
{
    /// <summary>
    /// Interakční logika pro Add_Playlist.xaml
    /// </summary>
    public partial class Add_Playlist : UserControl
    {
        public string Nname;
        public Add_Playlist()
        {
            InitializeComponent();
        }

        public event EventHandler AddButtonClick;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Nname = Naame.Text;

            if (AddButtonClick != null)
            {
                AddButtonClick(this, e);
            }
        }
    }
}
