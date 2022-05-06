using System.IO;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Shared.Models
{
    public class FileItem : BindableBase
    {
        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;


        private double duration;

        public double Duration
        {
            get { return duration; }
            set
            {
                Set(ref duration, value);
            }
        }


        public string Cover { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Lyrics { get; set; } = string.Empty;

        public FileItem()
        {

        }

        public FileItem(string file)
        {
            FileName = file;
            Name = Path.GetFileNameWithoutExtension(file);
            if (Name.IndexOf('-') > 0)
            {
                var temp = Name.Split(new char[] { '-' }, 2);
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
