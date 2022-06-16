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
    /// SettingWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow(SettingViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            DataContext = viewModel;
        }

        public SettingViewModel ViewModel;

        private void InstallBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PluginInstall((sender as Button).DataContext as PluginItem);
        }

        private void UnInstallBtn_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.PluginUnInstall((sender as Button).DataContext as PluginItem);
        }

        private void ImportBtn_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                RestoreDirectory = true,
                Filter = "DLL|*.dll|All|*.*",
                Multiselect = true,
            };
            if (picker.ShowDialog() != true)
            {
                return;
            }
            ViewModel.PluginImport(picker.FileNames);
        }
    }
}
