using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    /// <summary>
    /// 定时器工具
    /// </summary>
    public static class TimerUtil {
        private static Timer sTimer = new Timer(_onTimer, null, 1000 * 60, 1000 * 60);
        private static int sMinites = 0;
        private static void _onTimer(object obj) {
            sMinites++;
            On1Min?.Invoke();
            if (sMinites % 5 == 0) On5Min?.Invoke();
            if (sMinites % 10 == 0) On10Min?.Invoke();
            if (sMinites % 30 == 0) On30Min?.Invoke();
            if (sMinites % 60 == 0) On1Hour?.Invoke();
            if (sMinites % 300 == 0) On5Min?.Invoke();
            if (sMinites % 1440 == 0) On1Day?.Invoke();
        }

        public static Action On1Min { get; set; }
        public static Action On5Min { get; set; }
        public static Action On10Min { get; set; }
        public static Action On30Min { get; set; }
        public static Action On1Hour { get; set; }
        public static Action On5Hour { get; set; }
        public static Action On1Day { get; set; }
    }
}
