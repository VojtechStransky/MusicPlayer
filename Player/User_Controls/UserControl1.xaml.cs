using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Player.User_Controls
{
    /// <summary>
    /// Interakční logika pro UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : UserControl
    {
        public UserControl1()
        {
            
            InitializeComponent();
        }

        public void RightToLeftMarquee()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = -Title.ActualWidth;
            doubleAnimation.To = canMain.ActualWidth;
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;
            doubleAnimation.Duration = new Duration(TimeSpan.FromSeconds(5));
            Title.BeginAnimation(Canvas.RightProperty, doubleAnimation);
        }

        public event EventHandler StopButtonClick;
        private void Stop_button_Click(object sender, RoutedEventArgs e)
        {
            if (StopButtonClick != null)
            {
                StopButtonClick(this, e);
            }
        }

        public event EventHandler PreviousButtonClick;
        private void Previous_button_Click(object sender, RoutedEventArgs e)
        {
            if (PreviousButtonClick != null)
            {
                PreviousButtonClick(this, e);
            }
        }

        public event EventHandler NextButtonClick;
        private void Next_button_Click(object sender, RoutedEventArgs e)
        {
            if (NextButtonClick != null)
            {
                NextButtonClick(this, e);
            }
        }

        public event EventHandler MuteButtonClick;
        private void Mute_button_Click(object sender, RoutedEventArgs e)
        {
            if (MuteButtonClick != null)
            {
                MuteButtonClick(this, e);
            }
        }

        public event EventHandler ShuffleButtonClick;
        private void Shuffle_button_Click(object sender, RoutedEventArgs e)
        {
            if (ShuffleButtonClick != null)
            {
                ShuffleButtonClick(this, e);
            }
        }

        public event EventHandler DragStarted;
        private void sliProgress_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (DragStarted != null)
            {
                DragStarted(this, e);
            }
        }

        public event EventHandler DragCompleted;
        private void sliProgress_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (DragCompleted != null)
            {
                DragCompleted(this, e);
            }
        }

        public event EventHandler ValueChanged;
        private void sliProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }
    }
}
