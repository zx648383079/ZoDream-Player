using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using ZoDream.Player.Core;
using ZoDream.Player.Helpers;
using ZoDream.Player.Models;
using ZoDream.Player.ViewModels;
using ZoDream.Player.Views;

namespace ZoDream.Player
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly BassPlayer player;
        public MainViewModel ViewModel = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
            player = new BassPlayer();
            player.SpectrumUpdated += Player_SpectrumUpdated;
            player.TimeUpdated += Player_TimeUpdated;
            player.Ended += Player_Ended;
            ControlPanel.Volume = player.Volume;
        }

        public bool CatalogOpen
        {
            get
            {
                return FileBox.Visibility == Visibility.Visible;
            }
            set
            {
                FileBox.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                PanelBorder.CornerRadius = value ? new CornerRadius(20) : new CornerRadius(20, 20, 0, 0);
                CatalogBtn.IsChecked = value;
            }
        }

        public bool PanelFlip
        {
            get
            {
                return MiniPanel.Visibility == Visibility.Visible;
            }
            set
            {
                FullPanel.Visibility = value ? Visibility.Collapsed : Visibility.Visible;
                MiniPanel.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                LyricsBtn.IsChecked = value;
                LoadLyrics();
            }
        }

        private void Player_Ended(object sender, ElapsedEventArgs e)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                ViewModel.Paused = true;
                ControlPanel_Next(null);
            });
        }

        private void Player_TimeUpdated(object sender, ElapsedEventArgs e)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                ControlPanel.Progress = player.Position;
                if (PanelFlip)
                {
                    lyricsBox.CurrentTime = ControlPanel.Progress;
                }
            });
        }

        private void Player_SpectrumUpdated(object sender, ElapsedEventArgs e)
        {
            Application.Current?.Dispatcher?.Invoke(() =>
            {
                SpectrumBox.Items = player.ChannelData(SpectrumBox.MaxLength);
            });
        }

        ~MainWindow()
        {
            player.Free();
        }

        private void MiniBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void QuitBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ControlPanel_Play(object sender)
        {
            if (ViewModel.FileItems.Count > 0)
            {
                if (player.ChannelHandle != 0)
                {
                    player.Play();
                    ViewModel.Paused = false;
                    return;
                }
                play(ViewModel.FileItems[0]);
                FileBox.SelectedIndex = 0;
                return;
            }
            OpenFiles();
        }

        private void OpenFiles()
        {
            var files = Local.ChooseFiles();
            ViewModel.Append(files);
        }

        private void play(FileItem item)
        {
            ControlPanel_Stop(null);
            if (player.Play(item.FileName) == 0)
            {
                ViewModel.Paused = true;
                return;
            }
            item.Duration = player.Duration;
            ViewModel.Paused = false;
            ViewModel.CurrentFile = item;
            lyricsBox.CurrentTime = ControlPanel.Progress = 0;
            LoadLyrics();
            if (PanelFlip && lyricsBox.Items?.Count < 1)
            {
                PanelFlip = false;
            }
        }

        private void LoadLyrics()
        {
            if (ViewModel.LyricsLoaded || !PanelFlip)
            {
                return;
            }
            ViewModel.LyricsLoaded = true;
            lyricsBox.Items = ViewModel.LoadLyrics();
        }

        private void ControlPanel_VolumeChanged(object sender, double value)
        {
            player.Volume = value;
        }

        private void ControlPanel_ProgressChanged(object sender, double value)
        {
            player.Position = value;
            ViewModel.Paused = player.Paused;
        }

        private void ControlPanel_Pause(object sender)
        {
            player.Pause();
            ControlPanel.Paused = true;
        }

        private void FileBox_SelectionChanged(object sender, FileItem item, int index)
        {
            play(item);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ControlPanel_Next(object sender)
        {
            if (ViewModel.FileItems.Count < 2)
            {
                return;
            }
            FileBox.SelectedIndex = FileBox.SelectedIndex >= ViewModel.FileItems.Count - 1 ? 0 : (FileBox.SelectedIndex + 1);
            play(ViewModel.FileItems[FileBox.SelectedIndex]);
        }

        private void ControlPanel_Stop(object sender)
        {
            player.Stop();
            ViewModel.Paused = true;
        }

        private void ControlPanel_Previous(object sender)
        {
            if (ViewModel.FileItems.Count < 2)
            {
                return;
            }
            FileBox.SelectedIndex = FileBox.SelectedIndex < 1 ? ViewModel.FileItems.Count - 1 : (FileBox.SelectedIndex - 1);
            play(ViewModel.FileItems[FileBox.SelectedIndex]);
        }

        private void FileBox_Open(object sender)
        {
            OpenFiles();
        }

        private void FileBox_Close(object sender)
        {
            CatalogOpen = false;
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingMenu.IsOpen = true;
        }

        private void LyricsBtn_Click(object sender, RoutedEventArgs e)
        {
            PanelFlip = !PanelFlip;
        }

        private void CatalogBtn_Click(object sender, RoutedEventArgs e)
        {
            CatalogOpen = !CatalogOpen;
        }

        private void LyricsEditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Paused)
            {
                return;
            }
            var page = new LyricsEditView();
            page.Player = player;
            page.Show();
        }
    }
}
