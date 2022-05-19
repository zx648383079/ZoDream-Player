using System;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace NetMusicLoader
{
    public class Loader : INetSource
    {
        public Task<bool> SaveAsync(string fileName, NetItem data, NetSoundQuality quality)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveLyricsAsync(string fileName, NetItem data)
        {
            throw new NotImplementedException();
        }

        public Task<NetPage> SearchAsync(string keywords, int page)
        {
            throw new NotImplementedException();
        }
    }
}
