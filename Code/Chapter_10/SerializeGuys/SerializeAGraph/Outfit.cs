using System;
using System.Collections.Generic;
using System.Text;

namespace SerializeGuys
{
    class Outfit
    {
        public string Top { get; set; }
        public string Bottom { get; set; }
        public override string ToString() => $"{Top} and {Bottom}";
    }
}
