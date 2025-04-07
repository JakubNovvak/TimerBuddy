using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuasarIT.TimerBuddy.App.Helpers
{
    static class TimerStringDecorators
    {
        public static string FormatTimeString(TimeSpan timerTime)
        {
            return timerTime.Hours != 0
                ? $"{timerTime.Hours:D2}:{timerTime.Minutes:D2}:{timerTime.Seconds:D2}"
                : $"{timerTime.Minutes:D2}:{timerTime.Seconds:D2}";
        }
    }
}
