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
    public class DbManager {

        //
        public void SetPostion(Position pos) {
            updatePosition(pos);
        }

        //
        public void SetOrder(Order order) {
            updateOrder(order);
        }

        //strategy
        private static ConcurrentDictionary<string, T_Strategy> stgDic;
        private static void loadStrategy() {
            if (stgDic == null) {
                stgDic = new ConcurrentDictionary<string, T_Strategy>();
                using (MyDbContext ctx = new MyDbContext()) {
                    List<T_Strategy> stgs = ctx.Strategys.ToList();
                    foreach (T_Strategy stg in stgs) {
                        stgDic[stg.ClassName] = stg;
                    }
                }
            }
        }

        private void AddStrategy(BaseStrategy stg) {
            loadStrategy();
            if (stgDic.ContainsKey(stg.GetType().FullName))
                return;
            //
            T_Strategy tStg = new T_Strategy();
            tStg.Name = stg.Name;
            tStg.ClassName = stg.GetType().FullName;
            tStg.Enable = stg.Enable;
            //
            using (MyDbContext ctx = new MyDbContext()) {
                tStg = ctx.Strategys.Add(tStg);
                ctx.SaveChanges();
                stgDic[tStg.ClassName] = tStg;
            }
        }
        static List<T_Strategy> Strategys {
            get {
                loadStrategy();
                return stgDic.Values.ToList();
            }
        }
        static T_Strategy GetStrategy(string fullName) {
            return stgDic[fullName];
        }

        //position
        private void updatePosition(Position pos) {
            T_Strategy stg = stgDic[pos.Strategy.GetType().FullName];
            using (MyDbContext ctx = new MyDbContext()) {
                T_Position tp = (from p in stg.Positions
                                 where p.InstrumentId == pos.Instrument.InstrumentID
                                 select p).First();
                if(tp == null) {
                    tp = new T_Position();
                    tp.InstrumentId = pos.Instrument.InstrumentID;
                    tp.Strategy = stg;
                    stg.Positions.Add(tp);
                }
                tp.Position = pos.Vol;
                tp.LastTime = DateTime.Now;
                ctx.SaveChanges();
            }
        }

        //order
        private void updateOrder(Order order) {
            T_Strategy stg = stgDic[order.Strategy.GetType().FullName];
            using(MyDbContext ctx = new MyDbContext()) {
                T_Order to = (from o in stg.Orders
                              where o.OrderId == order.OrderId
                              select o).First();
                if(to == null) {
                    to = new T_Order();
                    to.Direction = order.Direction.ToString();
                    to.InstrumentId = order.Instrument.InstrumentID;
                    to.OrderId = order.OrderId;
                    to.OrderTime = order.OrderTime;
                    to.Price = order.Price;
                    to.Strategy = stg;
                    to.Volume = order.Volume;
                    stg.Orders.Add(to);
                }
                to.VolumeTraded = order.VolumeTraded;
                ctx.SaveChanges();
            }
        }
    }
}
