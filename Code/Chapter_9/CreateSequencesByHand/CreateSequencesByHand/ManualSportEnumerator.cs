using System;
using System.Collections.Generic;
using System.Text;

namespace CreateSequencesByHand
{
    using System.Collections.Generic;
    class ManualSportEnumerator : IEnumerator<Sport>
    {
        int current = -1;
        public Sport Current { get { return (Sport)current; } }
        public void Dispose() { return; } // You’ll meet the Dispose method in Chapter 10
        object System.Collections.IEnumerator.Current { get { return Current; } }
        public bool MoveNext()
        {
            var maxEnumValue = Enum.GetValues(typeof(Sport)).Length;
            if ((int)current >= maxEnumValue - 1)
                return false;
            current++;
            return true;
        }
        public void Reset() { current = 0; }
    }

}
