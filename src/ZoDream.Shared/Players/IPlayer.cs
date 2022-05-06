using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Players
{
    public interface IPlayer: IDisposable
    {
        public event ControlValueEventHandler? TimeUpdated;
        public event ControlEventHandler? Began;
        public event ControlEventHandler? Ended;
        public event ControlValueEventHandler? VolumeChanged;

        public event ControlEventHandler? OnPlay;
        public event ControlEventHandler? OnPause;
        public event ControlEventHandler? OnStop;

        public bool IsPaused { get; }
        public bool IsPlaying { get; }
        public LoopMode LoopMode { get; set; }

        public bool CanGoNext { get; }
        public bool CanGoPrevious { get; }

        public double Volume { get; set; }

        public double Duration { get; }
        public double Progress { get; }

        public Task ReadyAsync();

        public Task PlayAsync();
        public Task PlayAsync(FileItem item);

        public Task PauseAsync();

        public Task StopAsync();

        public Task PlayNextAsync();
        public Task PlayPreviousAsync();

        public Task SeekAsync(double progress);

        public void Mute();

        public float[] ChannelData(int length);

        public void Add(FileItem item);
        public void Remove(FileItem item);

        public int IndexOf(FileItem item);
        public bool Contains(FileItem item);
    }
}
