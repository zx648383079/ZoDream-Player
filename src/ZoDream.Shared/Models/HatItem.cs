using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class HatItem
    {
        public double Current { get; set; }

        public double Target { get; set; }

        public double Speed { get; set; }

        public HatItem()
        {

        }

        public HatItem(double current, double target, double speed = .0)
        {
            Current = current;
            Target = target;
            Speed = speed;
        }


    }
}
