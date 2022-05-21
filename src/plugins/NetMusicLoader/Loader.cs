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
            throw new NotImplementedException();
        }

        public Task<NetPage> SearchAsync(string keywords, long page, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
