using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class PositionManager {
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, int>> sPositionMap = new ConcurrentDictionary<string, ConcurrentDictionary<string, int>>();

        public static void SetPosition(string stgName, string instId, int pos) {
            ConcurrentDictionary<string, int> dic;
            if (!sPositionMap.TryGetValue(stgName, out dic)) {
                dic = new ConcurrentDictionary<string, int>();
            }
            dic[instId] = pos;
        }

        public static int GetPosition(string stgName, string instId) {
            ConcurrentDictionary<string, int> dic;
            if (sPositionMap.TryGetValue(stgName, out dic)) {
                dic = new ConcurrentDictionary<string, int>();
                int i = 0;
                if (dic.TryGetValue(instId, out i)) {
                    return i;
                } else {
                    return 0;
                }
            } else {
                return 0;
            }
        }
    }
}
