using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Security;
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
using System.Windows.Forms;
using Player.Models;
using System.Windows.Controls.Primitives;

namespace Player.User_Controls
{
    /// <summary>
    /// Interakční logika pro Setting.xaml
    /// </summary>
    public partial class Setting : System.Windows.Controls.UserControl
    {
        private FolderBrowserDialog Musicfiles_dialog = new FolderBrowserDialog();
        public List<String> SourceFolders = new List<String>();

        public bool Changed;

        public Setting(double x, List<string> y)
        {
            InitializeComponent();
            SourceFolders = y;
            sliVolume.Value = x;
            Sources_List.ItemsSource = SourceFolders;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {

            DialogResult result = Musicfiles_dialog.ShowDialog();

            if (result == DialogResult.OK)
            {               
                SourceFolders.Add(Musicfiles_dialog.SelectedPath);                 
                Sources_List.ItemsSource = SourceFolders;
                Sources_List.Items.Refresh();
                Changed = true;
            }
            
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var i = Sources_List.Items.IndexOf(Sources_List.SelectedItem);
            if (i >= 0)
            {
                SourceFolders.RemoveAt(i);
                Sources_List.ItemsSource = SourceFolders;
                Sources_List.Items.Refresh();
                Changed = true;
            }
        }

        //Slider na hlasitost

        public event EventHandler ValueChanged;
        private void sliVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }
    }
}
