using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZoDream.Player.Models;

namespace ZoDream.Player.Controls
{
    /// <summary>
    /// CatalogBox.xaml 的交互逻辑
    /// </summary>
    public partial class CatalogBox : UserControl
    {
        public CatalogBox()
        {
            InitializeComponent();
        }



        public IList<FileItem> Items
        {
            get { return (IList<FileItem>)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(IList<FileItem>), typeof(CatalogBox), new PropertyMetadata(null));



        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedIndex", typeof(int), typeof(CatalogBox), new PropertyMetadata(-1));

        public event SelectionChangedEventHandler SelectionChanged;
        public event ControlEventHandler Open;
        public event ControlEventHandler Close;
        public event ControlEventHandler OnSetting;

        private void MainList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var index = MainList.SelectedIndex;
            SelectionChanged?.Invoke(this, Items[index], index);
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            Open?.Invoke(this);
        }

        private void SettingBtn_Click(object sender, RoutedEventArgs e)
        {
            OnSetting?.Invoke(this);
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close?.Invoke(this);
        }
    }

    public delegate void SelectionChangedEventHandler(object sender, FileItem item, int index);
}
