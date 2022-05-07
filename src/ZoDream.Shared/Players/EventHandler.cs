using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Players
{
    public delegate void ControlEventHandler(object sender);

    public delegate void ControlValueEventHandler<T>(object sender, T value);
}
