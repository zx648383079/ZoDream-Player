using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Readers
{
    public interface ILyricsReader
    {
        public Task<Lyrics?> ReadAsync(string file);
        public Task<Lyrics?> ReadFromStringAsync(string content);
    }
}
