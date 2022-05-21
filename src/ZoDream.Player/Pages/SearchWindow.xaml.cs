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
using System.Windows.Shapes;
using ZoDream.Player.ViewModels;
using ZoDream.Shared.Models;

namespace ZoDream.Player.Pages
{
    /// <summary>
    /// SearchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public SearchViewModel ViewModel = new();

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DownloadPopup.IsOpen)
            {
                DownloadPopup.IsOpen = false;
                return;
            }
            DownloadPopup.PlacementTarget = sender as Button;
            DownloadPopup.IsOpen = true;
        }

        private void RemoveBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Remove((sender as Button).DataContext as DownloadItem);
        }

        private void RetryBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Retry((sender as Button).DataContext as DownloadItem);
        }

        private void PauseBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Pause((sender as Button).DataContext as DownloadItem);
        }

        private void ContinueBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Continue((sender as Button).DataContext as DownloadItem);
        }

        private void Pagination_PageChanged(object sender, RoutedPropertyChangedEventArgs<long> e)
        {
            _ = ViewModel.SearchAsync(KeywordsTb.Text.Trim(), e.NewValue);
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            _ = ViewModel.SearchAsync(KeywordsTb.Text.Trim(), 1);
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ViewModel.SaveFolder))
            {
                var folder = new System.Windows.Forms.FolderBrowserDialog
                {
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
                };
                if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    return;
                }
                ViewModel.SaveFolder = folder.SelectedPath;
            }
            ViewModel.Add((sender as ListBoxItem)?.Content as NetItem, ViewModel.SoundQuality);
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Dispose();
        }

        private void KeywordsTb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                _ = ViewModel.SearchAsync(KeywordsTb.Text.Trim(), 1);
            }
        }

        private void DownloadPopup_MouseEnter(object sender, MouseEventArgs e)
        {
            DownloadPopup.StaysOpen = false;
        }
    }
}
