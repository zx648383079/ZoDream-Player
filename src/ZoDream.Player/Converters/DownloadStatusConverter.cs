using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using ZoDream.Shared.Models;

namespace ZoDream.Player.Converters
{
    public class DownloadStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (DownloadStatus)value;
            switch (parameter)
            {
                case 1:
                    return status == DownloadStatus.Paused ? Visibility.Visible : Visibility.Collapsed;
                case 2:
                    return status == DownloadStatus.Downloading ? Visibility.Visible : Visibility.Collapsed;
                case 3:
                    return status == DownloadStatus.Cancel || status == DownloadStatus.Failure ? Visibility.Visible : Visibility.Collapsed;
                case 4:
                    return status == DownloadStatus.Downloading ||
                       status == DownloadStatus.Paused
                        ? Visibility.Visible : Visibility.Collapsed;
            }
            return status switch
            {
                DownloadStatus.Waiting => "队列中",
                DownloadStatus.Paused => "已暂停",
                DownloadStatus.Downloading => "下载中",
                DownloadStatus.Cancel => "已取消",
                DownloadStatus.Success => "下载完成",
                DownloadStatus.Failure => "下载失败",
                _ => "-",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
