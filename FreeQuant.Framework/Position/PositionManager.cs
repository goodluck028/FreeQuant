using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class PositionManager {
        #region 策略持仓
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, int>> mStgPosMap = new ConcurrentDictionary<string, ConcurrentDictionary<string, int>>();

        public static void SetPosition(string stgName, string instID, int pos) {
            ConcurrentDictionary<string, int> dic;
            if (!mStgPosMap.TryGetValue(stgName, out dic)) {
                dic = new ConcurrentDictionary<string, int>();
            }
            dic[instID] = pos;
        }

        public static int GetPosition(string stgName, string instID) {
            ConcurrentDictionary<string, int> dic;
            if (mStgPosMap.TryGetValue(stgName, out dic)) {
                dic = new ConcurrentDictionary<string, int>();
                int i = 0;
                if (dic.TryGetValue(instID, out i)) {
                    return i;
                } else {
                    return 0;
                }
            } else {
                return 0;
            }
        }
        #endregion

        #region broker持仓
        private static Dictionary<Instrument, Position> mPositionMap = new Dictionary<Instrument, Position>();

        public static Position getPosition(Instrument inst) {
            Position bp;
            mPositionMap.TryGetValue(inst, out bp);
            if (bp == null) {
                bp = new Position(inst);
                mPositionMap.Add(inst, bp);
            }
            return bp;
        }
        public static void UpdatePosition(BrokerPosition position) {
            getPosition(position.Instrument).UpdatePosition(position);
        }

        public static void UpdatePosition(BrokerTrade trade) {
            getPosition(trade.Instrument).UpdatePosition(trade);
        }

        public static void AutoClose(Order order) {
            getPosition(order.Instrument).AutoClose(order);
        }

        public static void AddOrder(Order order) {
            getPosition(order.Instrument).AddOrder(order);
        }
        #endregion
    }
}
