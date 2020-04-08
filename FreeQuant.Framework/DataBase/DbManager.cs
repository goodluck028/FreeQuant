using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Data;
using System.Reflection;

namespace FreeQuant.Framework {
    public static class DbManager {
        //strategy
        private static ConcurrentDictionary<string, T_Strategy> mStgDic;
        private static void loadStrategy() {
            if (mStgDic == null) {
                mStgDic = new ConcurrentDictionary<string, T_Strategy>();
                using (MyDbContext ctx = new MyDbContext()) {
                    List<T_Strategy> stgs = ctx.Strategys.ToList();
                    foreach (T_Strategy stg in stgs) {
                        mStgDic[stg.ClassName] = stg;
                    }
                }
            }
        }

        public static void UpdateStrategy(BaseStrategy stg) {
            loadStrategy();
            using (MyDbContext ctx = new MyDbContext()) {
                T_Strategy tStg;
                if (!mStgDic.TryGetValue(stg.GetType().FullName, out tStg)) {
                    tStg = new T_Strategy();
                    tStg.Name = stg.Name;
                    tStg.ClassName = stg.GetType().FullName;
                    tStg.Enable = stg.Enable;
                    ctx.Strategys.Add(tStg);
                    mStgDic[tStg.ClassName] = tStg;
                }
                tStg.Enable = stg.Enable;
                ctx.SaveChanges();
            }
        }

        internal static List<T_Strategy> Strategys {
            get {
                loadStrategy();
                return mStgDic.Values.ToList();
            }
        }

        internal static T_Strategy GetStrategy(string fullName) {
            return mStgDic[fullName];
        }

        //position
        public static void UpdatePosition(StrategyPosition pos) {
            T_Strategy stg = mStgDic[pos.Strategy.GetType().FullName];
            using (MyDbContext ctx = new MyDbContext()) {
                T_Position tp = (from p in stg.Positions
                                 where p.InstrumentID == pos.Instrument.InstrumentID
                                 select p).First();
                if (tp == null) {
                    tp = new T_Position();
                    tp.InstrumentID = pos.Instrument.InstrumentID;
                    tp.Strategy = stg;
                    stg.Positions.Add(tp);
                }
                tp.Position = pos.Vol;
                tp.LastTime = DateTime.Now;
                ctx.SaveChanges();
            }
        }

        //order
        public static void UpdateOrder(Order order) {
            T_Strategy stg = mStgDic[order.Strategy.GetType().FullName];
            using (MyDbContext ctx = new MyDbContext()) {
                T_Order to = (from o in stg.Orders
                              where o.OrderId == order.OrderId
                              select o).First();
                if (to == null) {
                    to = new T_Order();
                    to.Direction = order.Direction.ToString();
                    to.InstrumentID = order.Instrument.InstrumentID;
                    to.OrderId = order.OrderId;
                    to.OrderTime = order.OrderTime;
                    to.Price = order.Price;
                    to.Strategy = stg;
                    to.Volume = order.Qty;
                    stg.Orders.Add(to);
                }
                to.VolumeTraded = order.QtyTraded;
                ctx.SaveChanges();
            }
        }
    }
}
