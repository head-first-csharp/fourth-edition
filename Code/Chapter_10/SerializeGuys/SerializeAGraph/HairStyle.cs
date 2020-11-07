using System;
using System.Collections.Generic;
using System.Text;

namespace SerializeGuys
{
    class HairStyle
    {
        public HairColor Color { get; set; }
        public float Length { get; set; }
        public override string ToString() => $"{Length:0.0} inch {Color} hair";
    }
}
