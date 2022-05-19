using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public enum SpectrumType
    {
        Columnar,
        SymmetryColumnar, // 上下对称
        InverseColumnar, // 左右镜像
        Ring,
        SymmetryRing,
        RingLine,
        Polyline,
        InversePolyline,
        PolylineRing,
        InversePolylineRing,
    }
}
