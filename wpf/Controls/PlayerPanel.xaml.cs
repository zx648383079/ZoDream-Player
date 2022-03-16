using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZoDream.Player.Controls
{
    /// <summary>
    /// PlayerPanel.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerPanel : UserControl
    {
        public PlayerPanel()
        {
            InitializeComponent();
            MouseEnter += PlayerPanel_MouseEnter;
            MouseLeave += PlayerPanel_MouseLeave;
        }

        private void PlayerPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            ActionBox.Visibility = Visibility.Collapsed;
        }

        private void PlayerPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            ActionBox.Visibility = Visibility.Visible;
        }

        public bool Paused
        {
            get { return (bool)GetValue(PausedProperty); }
            set { SetValue(PausedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Paused.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PausedProperty =
            DependencyProperty.Register("Paused", typeof(bool), typeof(PlayerPanel), new PropertyMetadata(true));

        public event ControlEventHandler Play;

        public event ControlEventHandler Stop;

        public event ControlEventHandler Pause;

        public event ControlEventHandler Next;

        public event ControlEventHandler Previous;

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            Previous?.Invoke(this);
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Paused)
            {
                Play?.Invoke(this);
                return;
            }
            else
            {
                Pause?.Invoke(this);
            }
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            Next?.Invoke(this);
        }
    }
}
