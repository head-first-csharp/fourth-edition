using System;
using System.Text;

namespace PowersOfTwo
{
    using System;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    class PowersOfTwo : IEnumerable<int>
    {
        public IEnumerator<int> GetEnumerator()
        {
            var maxPower = Math.Round(Math.Log(int.MaxValue, 2));
            for (int power = 0; power < maxPower; power++)
                yield return (int)Math.Pow(2, power);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

}
