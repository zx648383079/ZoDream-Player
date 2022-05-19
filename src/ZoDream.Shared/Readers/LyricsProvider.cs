using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public static class LyricsProvider
    {

        public static async Task<Lyrics?> ReadAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }
            if (!File.Exists(fileName))
            {
                return null;
            }
            var reader = Reader(Path.GetExtension(fileName));
            return await reader.ReadAsync(fileName);
        }

        public static async Task WriteAsync(string fileName, Lyrics lyrics)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return;
            }
            var writer = Writer(Path.GetExtension(fileName));
            await writer.WriteAsync(fileName, lyrics);
        }


        public static ILyricsReader Reader(string ext)
        {
            if (ext[0] == '.')
            {
                ext = ext.Substring(1);
            }
            return ext switch
            {
                "krc" => new KrcReader(),
                "qrc" => new QrcReader(),
                _ => new LrcReader(),
            };
        }

        public static ILyricsWriter Writer(string ext)
        {
            if (ext[0] == '.')
            {
                ext = ext.Substring(1);
            }
            return ext switch
            {
                "krc" => new KrcWriter(),
                "qrc" => new QrcWriter(),
                _ => new LrcWriter(),
            };
        }

        public static ILyricsWriter Writer(ILyricsReader reader)
        {
            if (reader is KrcReader)
            {
                return new KrcWriter();
            }
            if (reader is QrcReader)
            {
                return new QrcWriter();
            }
            if (reader is TrcReader)
            {
                return new TrcWriter();
            }
            return new LrcWriter();
        }
    }
}
