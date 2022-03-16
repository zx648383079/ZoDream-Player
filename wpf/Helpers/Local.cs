using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ZoDream.Player.Models;

namespace ZoDream.Player.Helpers
{
    public static class Local
    {

        /// <summary>
        /// 选择多个文件
        /// </summary>
        /// <returns></returns>
        public static List<string> ChooseFiles(string filter = "音乐文件|*.mp3;*.ape;*.flac;*.wav|所有文件|*.*")
        {
            var files = new List<string>();
            var open = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = filter,
                Title = "选择文件",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            };
            if (open.ShowDialog() == true)
            {
                files.AddRange(open.FileNames);
            }
            return files;
        }

        public static string ChooseFile(string filter = "音乐文件|*.mp3;*.ape;*.flac;*.wav|所有文件|*.*")
        {
            var open = new Microsoft.Win32.OpenFileDialog
            {
                Filter = filter,
                Title = "选择文件",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            };
            if (open.ShowDialog() == true)
            {
                return open.FileName;
            }
            return string.Empty;
        }

        /// <summary>
        /// 读文本文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string Read(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            var fs = new FileStream(file, FileMode.Open);
            var reader = new StreamReader(fs, (new TxtEncoder()).GetEncoding(fs));
            var content = reader.ReadToEnd();
            reader.Close();
            return content;
        }

    }
}
