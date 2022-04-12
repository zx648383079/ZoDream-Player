using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public interface ILyricsWriter
    {
        public Task WriteAsync(string file, Lyrics data);
        public Task<string> WriteToStringAsync(Lyrics data);
    }
}
