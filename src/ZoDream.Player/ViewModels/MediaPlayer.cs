using ManagedBass;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ZoDream.Shared.Models;
using ZoDream.Shared.Players;

namespace ZoDream.Player.ViewModels
{
    public class MediaPlayer : IPlayer
    {
        public bool IsReady { get; private set; } = false;

        public bool IsPaused { get; private set; } = true;

        public bool IsPlaying => !IsPaused;

        public LoopMode LoopMode { get; set; } = LoopMode.None;

        public int ChannelHandle { get; private set; }

        public bool CanGoNext => ViewModel.FileItems.Count > 1;

        public bool CanGoPrevious => ViewModel.FileItems.Count > 1;

        public int Count => ViewModel.FileItems.Count;

        /// <summary>
        /// 获取设置音量（/100）
        /// </summary>
        public double Volume 
        { 
            get {
                if (!IsReady)
                {
                    return 100;
                }
                return Bass.Volume * 100;
            } 
            set
            {
                Bass.Volume = value <= 0 ? 0 : (value / 100);
            } 
        }

        /// <summary>
        /// /ms
        /// </summary>
        public double Duration
        {
            get
            {
                if (ChannelHandle == 0)
                {
                    return .0;
                }
                return Bass.ChannelBytes2Seconds(ChannelHandle, Bass.ChannelGetLength(ChannelHandle)) * 1000;
            }
        }
        /// <summary>
        /// /ms
        /// </summary>
        public double Progress
        {
            get
            {
                if (ChannelHandle == 0)
                {
                    return 0;
                }
                return Bass.ChannelBytes2Seconds(ChannelHandle, Bass.ChannelGetPosition(ChannelHandle)) * 1000;
            }
        }

        private readonly Timer timer = new();
        private MainViewModel ViewModel;
        private int Current = -1;

        public event ControlValueEventHandler<double>? TimeUpdated;
        public event ControlValueEventHandler<FileItem>? Began;
        public event ControlEventHandler? Ended;
        public event ControlValueEventHandler<double>? VolumeChanged;
        public event ControlEventHandler? OnPlay;
        public event ControlEventHandler? OnPause;
        public event ControlEventHandler? OnStop;

        public MediaPlayer(MainViewModel model)
        {
            ViewModel = model;
            timer.Interval = 50;
            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (!IsReady || ChannelHandle == 0)
            {
                return;
            }
            TimeUpdated?.Invoke(this, Progress);
            if (Bass.ChannelIsActive(ChannelHandle) == PlaybackState.Stopped)
            {
                timer.Stop();
                Ended?.Invoke(this);
                IsPaused = true;
                _ = AutoPlayNextAsync();
                return;
            }
        }

        public void Add(FileItem item)
        {
            if (Contains(item))
            {
                return;
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                ViewModel.FileItems.Add(item);
            });
        }

        public float[] ChannelData(int length)
        {
            var data = new float[length];
            if (!IsReady || ChannelHandle == 0 || IsPaused)
            {
                return data;
            }
            Bass.ChannelGetData(ChannelHandle, data, (int)DataFlags.FFT256);
            return data;
        }

        public bool Contains(FileItem item)
        {
            return IndexOf(item) >= 0;
        }

        public void Dispose()
        {
            timer.Stop();
            IsPaused = true;
            if (ChannelHandle != 0)
            {
                Bass.SampleFree(ChannelHandle);
                Bass.MusicFree(ChannelHandle);
                ChannelHandle = 0;
            }
            Current = -1;
            Bass.Free();
        }

        public int IndexOf(FileItem item)
        {
            for (int i = 0; i < ViewModel.FileItems.Count; i++)
            {
                if (item.FileName == ViewModel.FileItems[i].FileName)
                {
                    return i;
                }
            }
            return -1;
        }

        public void Mute()
        {
            Volume = -1;
            VolumeChanged?.Invoke(this, -0);

        }

        public Task PauseAsync()
        {
            if (ChannelHandle != 0)
            {
                Bass.ChannelPause(ChannelHandle);
            }
            timer.Stop();
            OnPause?.Invoke(this);
            IsPaused = true;
            return Task.CompletedTask;
        }

        public Task PlayAsync()
        {
            if (ChannelHandle == 0)
            {
                return PlayNextAsync();
            }
            Bass.ChannelPlay(ChannelHandle);
            timer.Start();
            OnPlay?.Invoke(this);
            IsPaused = false;
            return Task.CompletedTask;
        }

        public async Task PlayAsync(FileItem item)
        {
            await PlayAsync(IndexOf(item));
        }

        public async Task PlayNextAsync()
        {
            if (ViewModel.FileItems.Count < 1)
            {
                return;
            }
            var i = Current + 1;
            if (i >= ViewModel.FileItems.Count)
            {
                i = 0;
            }
            await PlayAsync(i);
        }

        public async Task PlayPreviousAsync()
        {
            if (ViewModel.FileItems.Count < 1)
            {
                return;
            }
            var i = Current - 1;
            if (i < 0)
            {
                i = ViewModel.FileItems.Count  - 1;
            }
            await PlayAsync(i);
        }


        private async Task PlayAsync(int index)
        {
            if (index == Current)
            {
                await SeekAsync(0);
                await PlayAsync();
                return;
            }
            await StopAsync();
            var item = ViewModel.FileItems[index];
            if ((ChannelHandle = Bass.MusicLoad(item.FileName, Flags: BassFlags.Bass3D | BassFlags.Float)) == 0 &&
                (ChannelHandle = Bass.SampleLoad(item.FileName, 0, 0, 1, BassFlags.Bass3D | BassFlags.Mono | BassFlags.Float)) == 0)
            {
                return;
            }
            item.Duration = Duration;
            Bass.SampleGetChannel(ChannelHandle);
            Began?.Invoke(this, item);
            await PlayAsync();
            Current = index;
        }

        private async Task AutoPlayNextAsync()
        {
            int i;
            switch (LoopMode)
            {
                case LoopMode.None:
                    i = Current + 1;
                    if (i >= Count)
                    {
                        return;
                    }
                    await PlayAsync(i);
                    break;
                case LoopMode.Loop:
                    i = Current + 1;
                    if (i >= Count)
                    {
                        i = 0;
                    }
                    await PlayAsync(i);
                    break;
                case LoopMode.Random:
                    if (Count < 2)
                    {
                        return;
                    }
                    await PlayAsync(GetRandom(Count, Current));
                    break;
                case LoopMode.SingleLoop:
                    await PlayAsync(Current);
                    break;
                case LoopMode.Single:
                    break;
                default:
                    break;
            }
        }

        private static int GetRandom(int count, int extra)
        {
            var rnd = new Random();
            var res = rnd.Next(count - 1);
            return res >= extra ? res + 1 : res;
        }

        public async Task ReadyAsync()
        {
            if (IsReady)
            {
                return;
            }
            await Task.Factory.StartNew(() =>
            {
                var baseName = "bass.dll";
                var isX64 = Environment.Is64BitOperatingSystem;
                var baseFolder = AppDomain.CurrentDomain.BaseDirectory;
                var bassFolder = Path.Combine(baseFolder, "bass", (isX64 ? "x64" : "x86"));
                var bassFile = Path.Combine(baseFolder, baseName);
                if (!File.Exists(bassFile))
                {
                    File.Copy(Path.Combine(bassFolder, baseName), bassFile);
                }
                Bass.Init(Flags: DeviceInitFlags.Latency);
                var theFolder = new DirectoryInfo(bassFolder);
                if (!theFolder.Exists)
                {
                    return;
                }
                var items = theFolder.GetFiles();
                foreach (var item in items)
                {
                    if (item.Name == baseName)
                    {
                        continue;
                    }
                    Bass.PluginLoad(item.FullName);
                }
                IsReady = true;
            });
        }

        public void Remove(FileItem item)
        {
            var i = IndexOf(item);
            if (i < 0)
            {
                return;
            }
            App.Current.Dispatcher.Invoke(() =>
            {
                ViewModel.FileItems.RemoveAt(i);
            });
            if (i != Current)
            {
                return;
            }
            if (IsPaused)
            {
                StopAsync();
                return;
            }
            _ = PlayPreviousAsync();
        }

        public Task SeekAsync(double progress)
        {
            return Task.Factory.StartNew(() =>
            {
                if (ChannelHandle == 0)
                {
                    return;
                }
                Bass.ChannelSetPosition(ChannelHandle, Bass.ChannelSeconds2Bytes(ChannelHandle, progress / 1000));
            });
        }

        public Task StopAsync()
        {
            timer.Stop();
            IsPaused = true;
            if (ChannelHandle != 0)
            {
                Bass.SampleFree(ChannelHandle);
                Bass.MusicFree(ChannelHandle);
                ChannelHandle = 0;
                OnStop?.Invoke(this);
            }
            Current = -1;
            return Task.CompletedTask;
        }
    }
}
