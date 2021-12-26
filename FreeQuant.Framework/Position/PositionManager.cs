using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework
{
    /// <summary>
    /// 持仓管理器
    /// </summary>
    public class PositionManager
    {

        //只能内部初始化
        internal PositionManager()
        {

        }

        //策略持仓
        #region
        //存储策略持仓的字典
        private ConcurrentDictionary<string, ConcurrentDictionary<string, int>> mStgPosMap = new ConcurrentDictionary<string, ConcurrentDictionary<string, int>>();

        //设置持仓
        internal void SetPosition(string stgName, string instID, int pos)
        {
            ConcurrentDictionary<string, int> dic;
            if (!mStgPosMap.TryGetValue(stgName, out dic))
            {
                dic = new ConcurrentDictionary<string, int>();
            }
            dic[instID] = pos;
        }

        //获取持仓
        public int GetPosition(string stgName, string instID)
        {
            ConcurrentDictionary<string, int> dic;
            if (mStgPosMap.TryGetValue(stgName, out dic))
            {
                dic = new ConcurrentDictionary<string, int>();
                int i = 0;
                if (dic.TryGetValue(instID, out i))
                {
                    return i;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion

        //broker持仓
        #region 
        //存储broker持仓
        private Dictionary<Instrument, Position> mPositionMap = new Dictionary<Instrument, Position>();

        //获取持仓
        public Position getPosition(Instrument inst)
        {
            Position bp;
            mPositionMap.TryGetValue(inst, out bp);
            if (bp == null)
            {
                bp = new Position(inst);
                mPositionMap.Add(inst, bp);
            }
            return bp;
        }

        //更新持仓
        public void UpdatePosition(BrokerPosition position)
        {
            getPosition(position.Instrument).UpdatePosition(position);
        }

        public void UpdatePosition(BrokerTrade trade)
        {
            getPosition(trade.Instrument).UpdatePosition(trade);
        }

        //自动平仓
        public void AutoClose(Order order)
        {
            getPosition(order.Instrument).AutoClose(order);
        }

        //添加持仓
        public void AddOrder(Order order)
        {
            getPosition(order.Instrument).AddOrder(order);
        }
        #endregion
    }
}
