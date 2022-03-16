using ManagedBass;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Markup;
using ZoDream.Player.Controls;

namespace ZoDream.Player.Core
{
    public class BassPlayer
    {
        private static bool _initialized = false;
        private readonly Timer timer;
        /// <summary>
        /// 刷新时间
        /// </summary>
        const int FPS_TIMEOUT = 50;
        /// <summary>
        /// 频谱刷新时间
        /// </summary>
        const int SPECTRUM_TIMEOUT = 150;
        private int SpectrumTime = 0;
        public int ChannelHandle { get; private set; }
        /// <summary>
        /// 进度更新操作UI 请使用 Application.Current?.Dispatcher?.Invoke
        /// </summary>
        public event ElapsedEventHandler TimeUpdated;
        /// <summary>
        /// 播放结束 新操作UI 请使用 Application.Current?.Dispatcher?.Invoke
        /// </summary>
        public event ElapsedEventHandler Ended;
        /// <summary>
        /// 频率刷新
        /// </summary>
        public event ElapsedEventHandler SpectrumUpdated;

        public event ControlEventHandler OnPlay;
        public event ControlEventHandler OnPause;
        public event ControlEventHandler OnStop;

        public BassPlayer()
        {
            timer = new Timer(FPS_TIMEOUT);
            timer.Elapsed += Timer_Elapsed;
            Initialize();
            //var info = new BassInfo();
            //Bass.GetInfo(out info);
        }
        /// <summary>
        /// 初始化加载插件
        /// </summary>
        private void Initialize()
        {
            if (_initialized)
            {
                return;
            }
            _initialized = true;
            var isX64 = Environment.Is64BitOperatingSystem;
            Bass.Init(Flags: DeviceInitFlags.Latency);
            var dir = "bass\\" + (isX64 ? "x64" : "x86");
            var theFolder = new DirectoryInfo(dir);
            if (!theFolder.Exists)
            {
                return;
            }
            var items = theFolder.GetFiles();
            foreach (var item in items)
            {
                if (item.Name == "bass.dll")
                {
                    continue;
                }
                Bass.PluginLoad(item.FullName);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimeUpdated?.Invoke(this, e);
            if (ChannelHandle != 0 && Bass.ChannelIsActive(ChannelHandle) == PlaybackState.Stopped)
            {
                timer.Stop();
                Ended?.Invoke(this, e);
                SpectrumUpdated?.Invoke(this, e);
                return;
            }
            SpectrumTime -= FPS_TIMEOUT;
            if (SpectrumTime <= 0)
            {
                SpectrumUpdated?.Invoke(this, e);
                SpectrumTime = SPECTRUM_TIMEOUT;
            }
        }
        /// <summary>
        /// 获取总时长（s）
        /// </summary>
        public double Duration
        {
            get
            {
                if (ChannelHandle == 0)
                {
                    return 0;
                }
                return Bass.ChannelBytes2Seconds(ChannelHandle, Bass.ChannelGetLength(ChannelHandle));
            }
        }
        /// <summary>
        /// 获取设置当前进度（s）
        /// </summary>
        public double Position
        {
            get
            {
                if (ChannelHandle == 0)
                {
                    return 0;
                }
                return Bass.ChannelBytes2Seconds(ChannelHandle, Bass.ChannelGetPosition(ChannelHandle));
            }
            set
            {
                if (ChannelHandle == 0)
                {
                    return;
                }
                Bass.ChannelSetPosition(ChannelHandle, Bass.ChannelSeconds2Bytes(ChannelHandle, value));
            }
        }
        /// <summary>
        /// 是否停止状态
        /// </summary>
        public bool Paused
        {
            get {
                if (ChannelHandle == 0)
                {
                    return true;
                }
                return Bass.ChannelIsActive(ChannelHandle) != PlaybackState.Playing; 
            }
        }
        /// <summary>
        /// 获取设置音量（/100）
        /// </summary>
        public double Volume
        {
            get { 
                return Bass.Volume * 100;
            }
            set
            {
                Bass.Volume = value <= 0 ? 0 : (value / 100);
            }
        }

        /// <summary>
        /// 播放
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public int Play(string fileName)
        {
            Stop();
            if ((ChannelHandle = Bass.MusicLoad(fileName)) == 0 &&
                (ChannelHandle = Bass.SampleLoad(fileName, 0, 0, 1, BassFlags.Bass3D | BassFlags.Mono)) == 0)
            {
                return 0;
            }
            Bass.SampleGetChannel(ChannelHandle);
            return Play();
        }
        /// <summary>
        /// 继续播放
        /// </summary>
        /// <returns></returns>
        public int Play()
        {
            if (ChannelHandle == 0)
            {
                return 0;
            }
            Bass.ChannelPlay(ChannelHandle);
            timer.Start();
            SpectrumTime = 0;
            OnPlay?.Invoke(this);
            return ChannelHandle;
        }
        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (ChannelHandle != 0)
            {
                Bass.ChannelPause(ChannelHandle);
            }
            timer.Stop();
            OnPause?.Invoke(this);
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (ChannelHandle != 0)
            {
                Bass.SampleFree(ChannelHandle);
                Bass.MusicFree(ChannelHandle);
                ChannelHandle = 0;
            }
            timer.Stop();
            OnStop?.Invoke(this);
        }

        /// <summary>
        /// 获取频谱数据
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public byte[] ChannelData(int length)
        {
            var data = new byte[length];
            if (ChannelHandle == 0 || Paused)
            {
                return data;
            }
            Bass.ChannelGetData(ChannelHandle, data, length);
            return data;
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Free()
        {
            Stop();
            Bass.Free();
        }
    }
}
