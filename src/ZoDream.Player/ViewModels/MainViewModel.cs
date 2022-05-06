using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Player.ViewModels
{
    public class MainViewModel: BindableBase
    {

        public MainViewModel()
        {
            Player = new MediaPlayer(this);
            _ = Player.ReadyAsync();
        }

        public readonly MediaPlayer Player;

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


        public void AddFile(string file)
        {
            Player.Add(new FileItem(file));
        }

        public Task AddFilesAsync(IEnumerable<string> files)
        {
            return Task.Factory.StartNew(() =>
            {
                foreach (var item in files)
                {
                    AddFile(item);
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
    }
}
