using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Player.Core;
using ZoDream.Player.Helpers;
using ZoDream.Player.Models;

namespace ZoDream.Player.ViewModels
{
    public class MainViewModel: BindableBase
    {
        /// <summary>
        /// 歌词是否加载
        /// </summary>
        public bool LyricsLoaded { get; set; } = false;

        private ObservableCollection<FileItem> fileItems = new ObservableCollection<FileItem>();

        public ObservableCollection<FileItem> FileItems
        {
            get => fileItems;
            set => Set(ref fileItems, value);
        }

        private bool paused = true;

        public bool Paused
        {
            get => paused = true;
            set => Set(ref paused, value);
        }

        private FileItem currentFile;

        public FileItem CurrentFile
        {
            get => currentFile;
            set {
                LyricsLoaded = false;
                Set(ref currentFile, value);
            }
        }


        public void Append(IList<string> files)
        {
            foreach (var item in files)
            {
                Append(item);
            }
        }

        public void Append(string file)
        {
            if (IndexOf(file) >= 0)
            {
                return;
            }
            FileItems.Add(new FileItem(file));
        }

        public void Append(IList<FileItem> files)
        {
            foreach (var item in files)
            {
                Append(item);
            }
        }

        public void Append(FileItem file)
        {
            if (IndexOf(file) >= 0)
            {
                return;
            }
            FileItems.Add(file);
        }

        public int IndexOf(string file)
        {
            for (int i = 0; i < FileItems.Count; i++)
            {
                if (file == FileItems[i].FileName)
                {
                    return i;
                }
            }
            return -1;
        }

        public int IndexOf(FileItem file)
        {
            return IndexOf(file.FileName);
        }

        public void Remove(int index)
        {
            FileItems.RemoveAt(index);
        }

        public void Remove(string file)
        {
            var i = IndexOf(file);
            if (i >= 0)
            {
                Remove(i);
            }
        }

        public void Remove(FileItem file)
        {
            Remove(file.FileName);
        }

        public IList<LyricsItem> LoadLyrics()
        {
            if (CurrentFile == null || string.IsNullOrEmpty(CurrentFile.Lyrics))
            {
                return null;
            }
            return LyricsReader.RenderFile(CurrentFile.Lyrics, CurrentFile.Duration);
        }
    }
}
