using System;
using System.Collections.Generic;
using System.Text;
using ZoDream.Shared.ViewModels;

namespace ZoDream.Shared.Models
{
    public class PluginItem: BindableBase
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public string FileName { get; set; } = string.Empty;

        private bool isActive;

        public bool IsActive
        {
            get => isActive;
            set => Set(ref isActive, value);
        }

    }
}
