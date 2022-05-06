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
    /// CatalogWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CatalogWindow : Window
    {
        public CatalogWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public MainViewModel ViewModel = App.ViewModel;

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Link;
            e.Handled = true;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = ((IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop));
                ViewModel.AddFilesAsync(files);
            }
        }

        private void FileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenPicker();
        }

        public void OpenPicker()
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                RestoreDirectory = true,
                Filter = "Music|*.mp3;*.wav;*.ape;*.flac|All|*.*",
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            ViewModel.AddFilesAsync(picker.FileNames);
        }

        private void FolderBtn_Click(object sender, RoutedEventArgs e)
        {
            var folder = new System.Windows.Forms.FolderBrowserDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic)
            };
            if (folder.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            ViewModel.AddFolderAsync(folder.SelectedPath);
        }

        private void ListBoxItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListBoxItem)?.Content as FileItem;
            if (item == null)
            {
                return;
            }
            _ = ViewModel.Player.PlayAsync(item);
        }
    }
}
