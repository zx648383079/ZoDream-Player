using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class NetItem
    {
        public string Id { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Cover { get; set; } = string.Empty;

        public string Lyrics { get; set; } = string.Empty;

        public Dictionary<NetSoundQuality, string> UriItems { get; set; } = new();
    }

    public class NetPage
    {
        public List<NetItem> Data { get; set; } = new();

        public int Page { get; set; }

        public int PerPage { get; set; }

        public int Total { get; set; }

        public bool HasMore { get; set; }
    }

    public enum NetSoundQuality
    {
        None,
        LowQuality,
        MediumQuality,// 128
        HighQuality,// 320
        SuperQuality,
    }
}
