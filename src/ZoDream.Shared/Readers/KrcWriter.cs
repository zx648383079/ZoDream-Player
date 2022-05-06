using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;

namespace ZoDream.Shared.Readers
{
    public class KrcWriter : LrcWriter, ILyricsWriter
    {

        protected override string RenderTime(LyricsItem lyrics, LyricsItem? next = null)
        {
            return $"{lyrics.Offset},{lyrics.Duration}";
        }

        protected override string RenderWordTime(LyricsWordItem word)
        {
            return $"<{word.Offset},{word.Duration},0>";
        }

        protected override async Task WriteFileAsync(string file, string content)
        {
            await LocationStorage.WriteAsync(file, content);
        }
    }
}
