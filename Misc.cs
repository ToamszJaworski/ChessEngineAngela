using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CEAngela.Tools
{
    public static class Misc
    {
        public static int GetTimeInMs()
        {
            var _hh = int.Parse(DateTime.Now.ToString("hh"));
            var _mm = int.Parse(DateTime.Now.ToString("mm"));
            var _ss = int.Parse(DateTime.Now.ToString("ss"));
            var _mili = int.Parse(DateTime.Now.Millisecond.ToString());

            var _timeInSeconds = (_hh * 3600) + (_mm * 60) + (_ss);
            return _timeInSeconds * 1000 + _mili;
        }
    }
}
