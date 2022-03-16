using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Player.ViewModels;

namespace ZoDream.Player.Models
{
    public class FileItem: BindableBase
    {
        public string Name { get; set; }

        public string FileName { get; set; }


        private double duration;

        public double Duration
        {
            get { return duration; }
            set {
                Set(ref duration, value);
            }
        }


        public string Cover { get; set; }

        public string Author { get; set; }

        public string Lyrics { get; set; }

        public FileItem()
        {

        }

        public FileItem(string file)
        {
            FileName = file;
            Name = Path.GetFileNameWithoutExtension(file);
            if (Name.IndexOf('-') > 0)
            {
                var temp = Name.Split('-', 2);
                Name = temp[0];
                Author = temp[1];
            }
            var lyricsFile = file.Replace(Path.GetExtension(file), ".lrc");
            if (File.Exists(lyricsFile))
            {
                Lyrics = lyricsFile;
            }
        }
    }
}
