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
using ZoDream.Player.Pages;
using ZoDream.Player.ViewModels;

namespace ZoDream.Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public MainViewModel ViewModel = App.ViewModel;
        private readonly CatalogWindow Catalog = new();
        private int SpectRefreshTime = 0;



        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var p = e.GetPosition(this);
            if (p.Y < 50)
            {
                DragMove();
            }
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Catalog.Close();
            Window_Unloaded(sender, e);
            Close();
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingMenu.IsOpen = true;
        }

        private void CatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Catalog.Visibility == Visibility.Visible)
            {
                Catalog.Hide();
            } else
            {
                Catalog.Show();
                Catalog.Left = Left + Width - 10;
                Catalog.Top = Top;
            }
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Player.Dispose();
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.FileItems.Count < 1)
            {
                OpenIfEmpty();
                return;
            }
            _ = ViewModel.Player.PlayPreviousAsync();
        }

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.FileItems.Count < 1)
            {
                OpenIfEmpty();
                return;
            }
            _ = ViewModel.Player.PlayAsync();
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = ViewModel.Player.PauseAsync();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.FileItems.Count < 1)
            {
                OpenIfEmpty();
                return;
            }
            _ = ViewModel.Player.PlayNextAsync();
        }

        private void OpenIfEmpty()
        {
            Catalog.OpenPicker();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Player.Began += Player_Began;
            ViewModel.Player.Ended += Player_Ended;
            ViewModel.Player.OnPause += Player_OnPause;
            ViewModel.Player.OnPlay += Player_OnPlay;
            ViewModel.Player.TimeUpdated += Player_TimeUpdated;
            ViewModel.Player.OnStop += Player_OnStop;
        }

        private void Player_OnStop(object sender)
        {
            ViewModel.IsPaused = true;
            ProgressBar.Value = 0;
        }

        private void Player_TimeUpdated(object sender, double value = 0)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ProgressBar.Value = value;
                if (SpectRefreshTime < 1)
                {
                    SpectPanel.Items = ViewModel.Player.ChannelData(128);
                    SpectRefreshTime = 5;
                }
                SpectRefreshTime--;
            });
        }

        private void Player_OnPlay(object sender)
        {
            ViewModel.IsPaused = false;
            SpectRefreshTime = 0;
        }

        private void Player_OnPause(object sender)
        {
            ViewModel.IsPaused = true;
        }

        private void Player_Ended(object sender)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ViewModel.IsPaused = true;
                ProgressBar.Value = 0;
            });
        }

        private void Player_Began(object sender)
        {
            ProgressBar.Max = ViewModel.Player.Duration;
            ViewModel.IsPaused = false;
        }


    }
}
