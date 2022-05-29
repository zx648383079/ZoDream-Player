using System;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace NetMusicLoader
{
    public class Loader : INetSource
    {
        public Task<bool> SaveAsync(string fileName, string url, NetProgressEventHandler onProgress, CancellationToken token)
        {
            return Task.FromResult(true);
        }

        public Task<NetPage> SearchAsync(string keywords, long page, CancellationToken token)
        {
            return Task.FromResult(new NetPage());
        }
    }
}
