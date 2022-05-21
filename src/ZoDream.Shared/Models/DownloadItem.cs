using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Shared.Models
{
    public class DownloadItem: BindableBase
    {

        public string Name { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        private DownloadStatus status = DownloadStatus.None;

        public DownloadStatus Status
        {
            get => status;
            set {
                if (value != status && value == DownloadStatus.Downloading)
                {
                    LastTime = DateTime.MinValue;
                }
                Set(ref status, value);
            }
        }

        private long length;

        public long Length
        {
            get => length;
            set => Set(ref length, value);
        }

        private long progress;

        public long Progress
        {
            get => progress;
            set
            {
                UpdateSpeed(value, progress);
                Set(ref progress, value);
            }
        }


        private long speed = 0;

        public long Speed
        {
            get => speed;
            set => Set(ref speed, value);
        }

        public CancellationTokenSource DownloadToken { get; set; } = new();
        private DateTime LastTime = DateTime.MinValue;

        public void UpdateSpeed(long newProgress, long oldProgress = 0)
        {
            var now = DateTime.Now;
            if (LastTime == DateTime.MinValue)
            {
                Speed = newProgress;
                LastTime = now;
                return;
            }
            if (newProgress < oldProgress)
            {
                LastTime = now;
                return;
            }
            var diff = (now - LastTime).TotalSeconds;
            LastTime = now;
            if (diff <= 0)
            {
                Speed = 0;
                return;
            }
            Speed = (long)Math.Ceiling((newProgress - oldProgress) / diff);
        }

    }

    public enum DownloadStatus
    {
        None,
        Waiting,
        Paused,
        Downloading,
        Cancel,
        Success,
        Failure,
    }
}
