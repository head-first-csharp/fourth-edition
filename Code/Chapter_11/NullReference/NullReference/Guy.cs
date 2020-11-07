using System;
using System.Collections.Generic;
using System.Text;

namespace NullReference
{
    class Guy
    {
        public string Name { get; private set; }
        public int Age { get; private set; }
        public override string ToString() => $"a {Age}-year-old named {Name}";

        public Guy(int age, string name)
        {
            Age = age;
            Name = name;
        }

    }


}
