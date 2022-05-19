using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ZoDream.Player.ViewModels;

namespace ZoDream.Player
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ViewModel.AddFilesAsync(e.Args, true);
            //var mutex = new Mutex(true, "ZoDream.Player", out var res);
            //if (!res)
            //{
            //    Environment.Exit(0);
            //}
        }


        public static MainViewModel ViewModel = new();
    }
}
