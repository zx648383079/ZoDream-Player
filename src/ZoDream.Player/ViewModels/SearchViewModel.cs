using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Player.Plugin;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Player.ViewModels
{
    public class SearchViewModel: BindableBase, IDisposable
    {

        public SearchViewModel()
        {
            Loader = new PluginLoader(App.ViewModel.Option.PluginItems);
        }

        public string SaveFolder { get; set; } = string.Empty;

        private readonly PluginLoader Loader;

        private Array qualityItems = Enum.GetValues(typeof(NetSoundQuality));

        public Array QualityItems
        {
            get => qualityItems;
            set => Set(ref qualityItems, value);
        }

        private string keywords = string.Empty;

        public string Keywords
        {
            get => keywords;
            set => Set(ref keywords, value);
        }

        private NetSoundQuality soundQuality = NetSoundQuality.None;

        public NetSoundQuality SoundQuality
        {
            get => soundQuality;
            set => Set(ref soundQuality, value);
        }


        private ObservableCollection<NetItem> netItems = new();

        public ObservableCollection<NetItem> NetItems
        {
            get => netItems;
            set => Set(ref netItems, value);
        }

        private long page;

        public long Page
        {
            get => page;
            set => Set(ref page, value);
        }

        private long total;

        public long Total
        {
            get => total;
            set => Set(ref total, value);
        }


        private ObservableCollection<DownloadItem> downloadItems = new();

        public ObservableCollection<DownloadItem> DownloadItems
        {
            get => downloadItems;
            set => Set(ref downloadItems, value);
        }

        public void Remove(DownloadItem? item)
        {
            if (item is null)
            {
                return;
            }
            item.DownloadToken.Cancel();
            DownloadItems.Remove(item);
        }

        public void Retry(DownloadItem? item)
        {
            if (item is null)
            {
                return;
            }
            item.Status = DownloadStatus.Downloading;
            item.Progress = 0;
            _ = DownloadAsync(item);
        }

        public void Pause(DownloadItem? item)
        {
            if (item is null)
            {
                return;
            }
            item.Status = DownloadStatus.Paused;
            item.DownloadToken.Cancel();
        }

        public void Continue(DownloadItem? item)
        {
            if (item is null)
            {
                return;
            }
            item.Status = DownloadStatus.Downloading;
            _ = DownloadAsync(item);
        }

        public async Task SearchAsync(string keywords, long page)
        {
            foreach (var item in Loader.InstanceItems)
            {
                try
                {
                    var res = await item.SearchAsync(keywords, page, default);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        Page = res.Page;
                        Total = res.Total;
                        NetItems.Clear();
                        foreach (var item in res.Data)
                        {
                            NetItems.Add(item);
                        }
                    });
                    return;
                }
                catch (Exception)
                {

                }
            }
        }

        public void Add(NetItem? item, NetSoundQuality quality)
        {
            if (item is null)
            {
                return;
            }
            AddMusic(item, quality);
            if (string.IsNullOrEmpty(item.Lyrics))
            {
                return;
            }
            var data = new DownloadItem()
            {
                Name = $"[歌词]{item.Name}",
                Source = item.UriItems[quality],
                FileName = Path.Combine(SaveFolder, item.Name + ".lrc"),
            };
            _ = DownloadAsync(data);
            DownloadItems.Add(data);
        }

        private void AddMusic(NetItem item, NetSoundQuality quality)
        {
            if (!item.UriItems.ContainsKey(quality))
            {
                quality = item.UriItems.Keys.FirstOrDefault();
            }
            var ext = quality switch
            {
                NetSoundQuality.MediumQuality => ".ape",
                NetSoundQuality.HighQuality => ".flac",
                NetSoundQuality.SuperQuality => ".wav",
                _ => ".mp3",
            };
            var data = new DownloadItem()
            {
                Name = item.Name,
                Source = item.UriItems[quality],
                FileName = Path.Combine(SaveFolder, item.Name + ext),
            };
            _ = DownloadAsync(data);
            DownloadItems.Add(data);
        }

        public async Task DownloadAsync(DownloadItem data)
        {
            data.DownloadToken.Cancel();
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            data.DownloadToken = tokenSource;
            foreach (var item in Loader.InstanceItems)
            {
                try
                {
                    var res = await item.SaveAsync(data.FileName, data.Source, (p, t) =>
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            data.Progress = p;
                            data.Length = t;
                        });
                    }, token);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        data.Status = res ? DownloadStatus.Success : DownloadStatus.Failure;
                    });
                    return;
                }
                catch (Exception)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        data.Status = DownloadStatus.Failure;
                    });
                }
            }
        }

        public void Dispose()
        {
            Loader.Dispose();
        }
    }
}
