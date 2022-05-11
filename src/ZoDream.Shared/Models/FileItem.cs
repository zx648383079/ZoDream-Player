using System.IO;
using System.Threading.Tasks;
using ZoDream.Shared.Readers;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Shared.Models
{
    public class FileItem : BindableBase
    {
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 音乐文件的名字
        /// </summary>
        public string Title { get; set; } = string.Empty;

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

        private bool isPaused = true;

        public bool IsPaused
        {
            get => isPaused;
            set => Set(ref isPaused, value);
        }

        /// <summary>
        /// 获取歌词
        /// </summary>
        /// <returns></returns>
        public async Task<Lyrics?> LoadLyricsAsync()
        {
            var fileName = GetLyricsFile();
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }
            if (!File.Exists(fileName))
            {
                Lyrics = string.Empty;
                return null;
            }
            var ext = Path.GetExtension(fileName);
            var reader = ext switch
            {
                ".krc" => new KrcReader(),
                ".qrc" => new QrcReader(),
                _ => new LrcReader(),
            };
            return await reader.ReadAsync(fileName);
        }
        /// <summary>
        /// 获取歌词文件
        /// </summary>
        /// <returns></returns>
        private string GetLyricsFile()
        {
            if (!string.IsNullOrEmpty(Lyrics))
            {
                return Lyrics;
            }
            var baseFile = FileName.Replace(Path.GetExtension(FileName), "");
            var lyricsExt = new string[] { ".lrc", ".krc", ".qrc" };
            foreach (var item in lyricsExt)
            {
                var lyricsFile = baseFile + item;
                if (File.Exists(lyricsFile))
                {
                    Lyrics = lyricsFile;
                    break;
                }
            }
            return Lyrics;
        }

        public FileItem()
        {

        }

        public FileItem(string file)
        {
            FileName = file;
            Title = Name = Path.GetFileNameWithoutExtension(file);
            if (Name.IndexOf('-') > 0)
            {
                var temp = Name.Split(new char[] { '-' }, 2);
                Name = temp[0];
                Author = temp[1];
            }
            GetLyricsFile();
        }
    }
}
