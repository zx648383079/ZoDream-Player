using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Player.ViewModels
{
    public class SettingViewModel : BindableBase
    {
        public SettingViewModel(): this(new AppOption())
        {
        }

        public SettingViewModel(AppOption option)
        {
            Source = option;
        }

        private readonly AppOption Source;



        public AppOption ToOption()
        {

            return Source;
        }
    }
}
