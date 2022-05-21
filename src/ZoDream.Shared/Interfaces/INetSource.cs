using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface INetSource
    {

        public Task<NetPage> SearchAsync(string keywords, long page, CancellationToken token);

        public Task<bool> SaveAsync(string fileName, string url, NetProgressEventHandler onProgress, CancellationToken token);

    }

    public delegate void NetProgressEventHandler(long progress, long total);
}
