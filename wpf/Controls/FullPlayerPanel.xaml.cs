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
using ZoDream.Player.Converters;
using ZoDream.Player.Models;

namespace ZoDream.Player.Controls
{
    /// <summary>
    /// FullPlayerPanel.xaml 的交互逻辑
    /// </summary>
    public partial class FullPlayerPanel : UserControl
    {
        public FullPlayerPanel()
        {
            InitializeComponent();
        }

        public bool Paused
        {
            get { return (bool)GetValue(PausedProperty); }
            set { SetValue(PausedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Paused.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PausedProperty =
            DependencyProperty.Register("Paused", typeof(bool), typeof(FullPlayerPanel), new PropertyMetadata(true));



        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Volume.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(FullPlayerPanel), new PropertyMetadata(100.0));



        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Progress.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(FullPlayerPanel), new PropertyMetadata(0.0, OnProgressChanged));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FullPlayerPanel).RefreshProgress();
        }

        public FileItem Source
        {
            get { return (FileItem)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(FileItem), typeof(FullPlayerPanel), new PropertyMetadata(null, OnFileChanged));

        private static void OnFileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as FullPlayerPanel).RefreshFile();
        }

        private void RefreshFile()
        {
            CoverImage.Visibility = Source != null && !string.IsNullOrEmpty(Source.Cover) ? Visibility.Visible : Visibility.Collapsed;
            CoverImage.Source = CoverImage.Visibility == Visibility.Visible ? new BitmapImage(new Uri(Source.Cover, UriKind.Absolute)) : null;
            NameTb.Text = Source != null ? Source.Name : string.Empty;
            AuthorTb.Text = Source != null ? Source.Author : string.Empty;
        }

        private void RefreshProgress()
        {
            ProgressLabel.Text = TimeConverter.FormatTime(Progress) + "/" + TimeConverter.FormatTime(Source == null ? 0 : Source.Duration);
            ProgressSlider.Value = Source == null || Source.Duration < 1 || Progress < 1 ? 0 : (Progress * 100 / Source.Duration);
        }

        public event ControlEventHandler Play;

        public event ControlEventHandler Stop;

        public event ControlEventHandler Pause;

        public event ControlEventHandler Next;

        public event ControlEventHandler Previous;

        public event ControlValueEventHandler ProgressChanged;

        public event ControlValueEventHandler VolumeChanged;

        private void PlayBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Paused)
            {
                Play?.Invoke(this);
                return;
            } else
            {
                Pause?.Invoke(this);
            }
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (Source == null || Source.Duration < 1)
            {
                return;
            }
            ProgressChanged?.Invoke(this, (sender as Slider).Value * Source.Duration / 100);
        }

        private double oldVolume = 0;

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Volume = (sender as Slider).Value;
            VolumeChanged?.Invoke(this, Volume);
        }

        private void VolumeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Volume > 0)
            {
                oldVolume = Volume;
                VolumeChanged?.Invoke(this, Volume = -1);
                return;
            }
            Volume = oldVolume < 1 ? 50 : oldVolume;
            VolumeChanged?.Invoke(this, Volume);
        }

        private void PreviousBtn_Click(object sender, RoutedEventArgs e)
        {
            Previous?.Invoke(this);
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            Next?.Invoke(this);
        }
    }

    public delegate void ControlEventHandler(object sender);
    
    public delegate void ControlValueEventHandler(object sender, double value = 0);
}
