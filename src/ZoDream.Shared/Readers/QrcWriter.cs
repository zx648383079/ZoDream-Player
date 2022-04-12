using ICSharpCode.SharpZipLib.Zip.Compression;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public class QrcWriter : LrcWriter, ILyricsWriter
    {
        protected override async Task WriteFileAsync(string file, string content)
        {
            await Storage.File.WriteAsync(file, content);
        }
    }
}
