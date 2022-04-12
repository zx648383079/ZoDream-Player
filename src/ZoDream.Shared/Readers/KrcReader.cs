using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Readers
{
    /// <summary>
    /// 酷狗歌词解析
    /// </summary>
    public class KrcReader : LrcReader, ILyricsReader
    {
        public static readonly uint[] Marry = new uint[] { 64, 71, 97, 119, 94, 50, 116, 71, 81, 54, 49, 45, 206, 210, 110, 105 };
        
        protected override Task<string> ReadFileAsync(string file)
        {
            return Task.Factory.StartNew(() =>
            {
                using var fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                var data = new byte[fs.Length - 4];
                fs.Seek(4, SeekOrigin.Begin);
                fs.Read(data, 0, data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    var old = data[i];
                    data[i] = (byte)(data[i] ^ Marry[i % 16]);
                }
                using var outputStream = new MemoryStream();
                var inflater = new Inflater();
                try
                {
                    inflater.SetInput(data);
                    var buffer = new byte[2048];
                    while (!inflater.IsFinished)
                    {
                        var count = inflater.Inflate(buffer);
                        outputStream.Write(buffer, 0, count);
                    }
                }
                catch (Exception)
                {
                    inflater.Reset();
                }
                return Encoding.UTF8.GetString(outputStream.ToArray());
            });
        }
    }
}
