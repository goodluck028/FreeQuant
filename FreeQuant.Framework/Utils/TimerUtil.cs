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
            mOn1Min?.Invoke();
            if (sMinites % 5 == 0) mOn5Min?.Invoke();
            if (sMinites % 10 == 0) mOn10Min?.Invoke();
            if (sMinites % 30 == 0) mOn30Min?.Invoke();
            if (sMinites % 60 == 0) mOn1Hour?.Invoke();
            if (sMinites % 300 == 0) mOn5Min?.Invoke();
            if (sMinites % 1440 == 0) mOn1Day?.Invoke();
        }

        private static Action mOn1Min;
        public static event Action On1Min {
            add {
                mOn1Min -= value;
                mOn1Min += value;
            }
            remove {
                mOn1Min -= value;
            }
        }
        private static Action mOn5Min;
        public static event Action On5Min {
            add {
                mOn5Min -= value;
                mOn5Min += value;
            }
            remove {
                mOn5Min -= value;
            }
        }
        private static Action mOn10Min;
        public static event Action On10Min {
            add {
                On10Min -= value;
                On10Min += value;
            }
            remove {
                On10Min -= value;
            }
        }
        private static Action mOn30Min;
        public static event Action On30Min {
            add {
                On30Min -= value;
                On30Min += value;
            }
            remove {
                On30Min -= value;
            }
        }
        private static Action mOn1Hour;
        public static event Action On1Hour {
            add {
                mOn1Hour -= value;
                mOn1Hour += value;
            }
            remove {
                mOn1Hour -= value;
            }
        }
        private static Action mOn5Hour;
        public static event Action On5Hour {
            add {
                mOn5Hour -= value;
                mOn5Hour += value;
            }
            remove {
                mOn5Hour -= value;
            }
        }
        private static Action mOn1Day;
        public static event Action On1Day {
            add {
                mOn1Day -= value;
                mOn1Day += value;
            }
            remove {
                mOn1Day -= value;
            }
        }
    }
}
