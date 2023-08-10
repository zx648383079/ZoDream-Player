using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Player.ViewModels
{
    public class MainViewModel: BindableBase
    {

        public MainViewModel()
        {
            Player = new MediaPlayer(this);
        }



        public readonly MediaPlayer Player;

        public AppOption Option = new();

        public Lyrics? Lyrics { get; set; }

        private ObservableCollection<FileItem> fileItems = new();

        public ObservableCollection<FileItem> FileItems
        {
            get => fileItems;
            set => Set(ref fileItems, value);
        }

        private bool isPaused = true;

        public bool IsPaused
        {
            get => isPaused;
            set => Set(ref isPaused, value);
        }

        private string musicName = string.Empty;

        public string MusicName {
            get => musicName;
            set => Set(ref musicName, value);
        }


        public FileItem? AddFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file) || !File.Exists(file))
            {
                return null;
            }
            var item = new FileItem(file);
            Player.Add(item);
            return item;
        }

        public Task AddFilesAsync(IEnumerable<string> files, bool playFirst = false)
        {
            return Task.Factory.StartNew(() =>
            {
                FileItem? first = null;
                foreach (var item in files)
                {
                    var i = AddFile(item);
                    if (i != null && first == null)
                    {
                        first = i;
                    }
                }
                if (playFirst && first != null)
                {
                    _ = Player.PlayAsync(first);
                }
            });
        }

        public Task AddFolderAsync(string folder)
        {
            return Task.Factory.StartNew(() =>
            {
                AddFolder(new DirectoryInfo(folder));
            });
        }

        private void AddFolder(DirectoryInfo folder)
        {
            foreach (var item in folder.GetDirectories())
            {
                AddFolder(item);
            }
            foreach (var item in folder.GetFiles())
            {
                switch (item.Extension)
                {
                    case ".mp3":
                    case ".wav":
                    case ".flac":
                    case ".ape":
                        AddFile(item.FullName);
                        break;
                    default:
                        break;
                }
            }
        }

        public async Task<Lyrics?> LoadLyricsAsync(FileItem item, double duration)
        {
            Lyrics = await item.LoadLyricsAsync();
            Lyrics?.ApplyDuration((int)duration);
            return Lyrics;
        }

        public async Task<AppOption> LoadOptionAsync()
        {
            var option = await AppData.LoadAsync<AppOption>();
            if (option != null)
            {
                Option = option;
            }
            return Option;
        }

        public async Task SaveOptionAsync()
        {
            await AppData.SaveAsync(Option);
        }
    }
}
