using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    public partial class BaseStrategy {
        internal event Action<BaseStrategy, Instrument> OnSubscribeInstrument;
        internal event Action<Order> OnSendOrder;
        internal event Action<Order> OnCancelOrder;
        internal event Action<Position> OnChangePosition;
    }

    //用户实现
    public partial class BaseStrategy {
        public virtual void OnStart() { }
        public virtual void OnStop() { }
        public virtual void OnTick(Tick tick) { }
        public virtual void OnPositionChanged(Position position) { }
    }

    //行情
    public partial class BaseStrategy {

        //合约列表
        private Instrument mMainInstrument;
        private HashSet<Instrument> mInstrumentSet = new HashSet<Instrument>();

        //最新tick
        private Dictionary<Instrument, Tick> lastTickDic = new Dictionary<Instrument, Tick>();

        //订阅行情
        public void SubscribInstrument(Instrument instrument) {
            OnSubscribeInstrument?.Invoke(this, instrument);
            mMainInstrument = mMainInstrument ?? instrument;
            mInstrumentSet.Add(instrument);
        }

        //启停信号
        internal void SendStart() {
            OnStart();
        }
        internal void SendStop()
        {
            OnStop();
        }

        //发送tick
        internal void SendTick(Tick tick) {
            lastTickDic[tick.Instrument] = tick;
            OnTick(tick);
        }
    }

    //交易
    public partial class BaseStrategy {
        //持仓
        private Dictionary<Instrument, int> mPositionMap = new Dictionary<Instrument, int>();
        public int GetPosition(Instrument instrument) {
            if (mPositionMap.ContainsKey(instrument)) {
                return mPositionMap[instrument];
            } else {
                return 0;
            }
        }
        internal void AddPosition(Instrument instrument, int vol) {
            mPositionMap[instrument] = mPositionMap.ContainsKey(instrument) ? mPositionMap[instrument] + vol : vol;
            Position p = new Position(this, instrument, mPositionMap[instrument], DateTime.Now);
            OnChangePosition?.Invoke(p);
        }

        //订单
        private List<Order> orderList = new List<Order>();
        private List<Order> activeOrderList = new List<Order>();
        private List<Order> doneOrderList = new List<Order>();

        //发送订单
        internal void SendOrder(Order order) {
            orderList.Add(order);
            activeOrderList.Add(order);
            if (mTdProvider != null) {
                Tick t;
                double uper = 0;
                double lower = 0;
                if (lastTickDic.TryGetValue(order.InstrumentID, out t)) {
                    uper = t.UpperLimitPrice;
                    lower = t.LowerLimitPrice;
                }
                //
                Instrument inst;
                double priceTick = 0;
                if (mTdProvider.TryGetInstrument(order.InstrumentID, out inst)) {
                    priceTick = inst.PriceTick;
                }
                //
                if (priceTick != 0) {
                    order.Price = ((int)(order.Price / priceTick)) * priceTick;
                }
                if (order.Price > uper && uper != 0) {
                    order.Price = uper;
                } else if (order.Price < lower) {
                    order.Price = lower;
                }
                //
                mTdProvider.SendOrder(order);
            }
        }

        //撤单
        internal void CancleOrder(Order order) {
            OnCancelOrder?.Invoke(order);
        }

        //更新订单
        internal void UpdateOrder(Order order) {
            if (order.Status == OrderStatus.Canceled
                || order.Status == OrderStatus.Error
                || order.Status == OrderStatus.Filled) {
                activeOrderList.Remove(order);
                doneOrderList.Add(order);
            }
        }

        //获取合约
        public bool TryGetInstrument(string instrumentID, out Instrument inst) {
            return mTdProvider.TryGetInstrument(instrumentID, out inst);
        }

        //订单生成函数
        public Order BuyOrder(int vol) {
            if (lastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = lastTickDic[mMainInstrument].UpperLimitPrice;
                return BuyOrder(vol, price);
            } else {
                return BuyOrder(0, 0);
            }

        }
        public Order BuyOrder(int vol, double price) {
            return BuyOrder(vol, price, mMainInstrument);
        }
        public Order BuyOrder(int vol, double price, string instrumentID) {
            if (vol < 0) {
                return BuyOrder(0, price, instrumentID);
            }
            Order order = new Order(this, instrumentID, DirectionType.Buy, price, vol);
            return order;
        }
        public Order SellOrder(int vol) {
            if (lastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = lastTickDic[mMainInstrument].LowerLimitPrice;
                return SellOrder(vol, price);
            } else {
                return SellOrder(0, 0);
            }

        }
        public Order SellOrder(int vol, double price) {
            return SellOrder(vol, price, mMainInstrument);
        }
        public Order SellOrder(int vol, double price, string instrumentID) {
            if (vol < 0) {
                return SellOrder(0, price, instrumentID);
            }
            Order order = new Order(this, instrumentID, DirectionType.Sell, price, vol);
            return order;
        }
        public Order ToPositionOrder(int position) {
            int myPos = GetPosition(mMainInstrument);
            if (position > myPos && lastTickDic.ContainsKey(mMainInstrument)) {
                return BuyOrder(position - myPos, lastTickDic[mMainInstrument].UpperLimitPrice, mMainInstrument);
            } else if (position < myPos && lastTickDic.ContainsKey(mMainInstrument)) {
                return SellOrder(myPos - position, lastTickDic[mMainInstrument].LowerLimitPrice, mMainInstrument);
            } else {
                return BuyOrder(0);
            }
        }
        public Order ToPositionOrder(int position, double price) {
            return ToPositionOrder(position, price, mMainInstrument);
        }
        public Order ToPositionOrder(int position, double price, string instrumentID) {
            int myPos = GetPosition(mMainInstrument);
            if (position > myPos) {
                return BuyOrder(position - myPos, price, instrumentID);
            } else if (position < myPos) {
                return SellOrder(myPos - position, price, instrumentID);
            } else {
                return BuyOrder(0);
            }
        }
    }

    //其他
    public partial class BaseStrategy {
        //写日志
        public void Log(string content) {
            LogUtils.UserLog(content);
        }
    }


}
