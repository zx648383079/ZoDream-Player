using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface INetSource
    {

        public Task<NetPage> SearchAsync(string keywords, int page);

        public Task<bool> SaveAsync(string fileName, NetItem data, NetSoundQuality quality);

        public Task<bool> SaveLyricsAsync(string fileName, NetItem data);
    }
}
