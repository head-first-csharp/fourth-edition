using System;
using System.Collections.Generic;
using System.Text;

namespace BetterSportSequence
{
    using System.Collections.Generic;
    class BetterSportSequence : IEnumerable<Sport>
    {
        public IEnumerator<Sport> GetEnumerator()
        {
            int maxEnumValue = Enum.GetValues(typeof(Sport)).Length - 1;
            for (int i = 0; i <= maxEnumValue; i++)
            {
                yield return (Sport)i;
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Sport this[int index]
        {
            get => (Sport)index;
        }
    }

}
