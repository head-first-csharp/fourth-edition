using System;
using System.Collections.Generic;
using System.Text;

namespace PassGuyByReference
{
    class Guy
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public override string ToString() => $"a {Age}-year-old named {Name}";
    }

}
