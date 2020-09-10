using System;
using System.Collections.Generic;
using System.Text;

namespace CEAngela.Random
{
    class Rand
    {
        public static ulong Random64
        {
            get
            {
                var _value = new byte[8];
                var _random = new System.Random();

                _random.NextBytes(_value);

                var _long = (ulong)BitConverter.ToInt64(_value, 0);

                return _long;
            }
        }
    }
}
