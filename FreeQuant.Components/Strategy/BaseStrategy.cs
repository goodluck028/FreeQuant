using System;
using System.Collections.Generic;

namespace FreeQuant.Components {
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
        public void SubscribInstrument(params string[] instIds) {
            foreach (string instId in instIds)
            {
                Instrument inst = InstrumentManager.GetInstrument(instId);
                OnSubscribeInstrument?.Invoke(this, inst);
                mMainInstrument = mMainInstrument ?? inst;
                mInstrumentSet.Add(inst);
            }
        }

        //启停信号
        internal void SendStart() {
            OnStart();
        }
        internal void SendStop() {
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

        //订单管理
        OrderManager mOrderManager = new OrderManager();

        //发送订单
        internal void SendOrder(Order order) {
            Tick t;
            double uper = 0;
            double lower = 0;
            if (lastTickDic.TryGetValue(order.Instrument, out t)) {
                uper = t.UpperLimitPrice;
                lower = t.LowerLimitPrice;
            }
            //
            double priceTick = order.Instrument.PriceTick;
            if (priceTick != 0) {
                order.Price = ((int)(order.Price / priceTick)) * priceTick;
            }
            if (order.Price > uper && uper != 0) {
                order.Price = uper;
            } else if (order.Price < lower) {
                order.Price = lower;
            }
            //
            OnSendOrder?.Invoke(order);
            //
            mOrderManager.AddOrder(order);
        }

        //撤单
        internal void CancleOrder(Order order) {
            OnCancelOrder?.Invoke(order);
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
        public Order BuyOrder(int vol, double price, Instrument instrument) {
            if (vol < 0) {
                return BuyOrder(0, price, instrument);
            }
            Order order = new Order(this, instrument, DirectionType.Buy, price, vol);
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
        public Order SellOrder(int vol, double price, Instrument instrument) {
            if (vol < 0) {
                return SellOrder(0, price, instrument);
            }
            Order order = new Order(this, instrument, DirectionType.Sell, price, vol);
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
        public Order ToPositionOrder(int position, double price, Instrument instrument) {
            int myPos = GetPosition(mMainInstrument);
            if (position > myPos) {
                return BuyOrder(position - myPos, price, instrument);
            } else if (position < myPos) {
                return SellOrder(myPos - position, price, instrument);
            } else {
                return BuyOrder(0);
            }
        }
    }

    //其他
    public partial class BaseStrategy {
        //写日志
        public void Log(string content) {
            LogUtil.UserLog(content);
        }
    }


}
