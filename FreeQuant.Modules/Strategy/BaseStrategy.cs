using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    public partial class BaseStrategy {
        internal event Action<BaseStrategy, Instrument> OnSubscribeInstrument;
        internal event Action<StrategyOrder> OnSendOrder;
        internal event Action<StrategyOrder> OnCancelOrder;
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
        private List<StrategyOrder> orderList = new List<StrategyOrder>();
        private List<StrategyOrder> activeOrderList = new List<StrategyOrder>();
        private List<StrategyOrder> doneOrderList = new List<StrategyOrder>();

        //发送订单
        internal void SendOrder(StrategyOrder strategyOrder) {
            orderList.Add(strategyOrder);
            activeOrderList.Add(strategyOrder);
//            if (mTdProvider != null) {
//                Tick t;
//                double uper = 0;
//                double lower = 0;
//                if (lastTickDic.TryGetValue(strategyOrder.Instrument, out t)) {
//                    uper = t.UpperLimitPrice;
//                    lower = t.LowerLimitPrice;
//                }
//                //
//                Instrument inst;
//                double priceTick = 0;
//                if (mTdProvider.TryGetInstrument(strategyOrder.Instrument, out inst)) {
//                    priceTick = inst.PriceTick;
//                }
//                //
//                if (priceTick != 0) {
//                    strategyOrder.Price = ((int)(strategyOrder.Price / priceTick)) * priceTick;
//                }
//                if (strategyOrder.Price > uper && uper != 0) {
//                    strategyOrder.Price = uper;
//                } else if (strategyOrder.Price < lower) {
//                    strategyOrder.Price = lower;
//                }
//                //
//                mTdProvider.SendOrder(strategyOrder);
//            }
        }

        //撤单
        internal void CancleOrder(StrategyOrder strategyOrder) {
            OnCancelOrder?.Invoke(strategyOrder);
        }

        //更新订单
        internal void UpdateOrder(StrategyOrder strategyOrder) {
            if (strategyOrder.Status == OrderStatus.Canceled
                || strategyOrder.Status == OrderStatus.Error
                || strategyOrder.Status == OrderStatus.Filled) {
                activeOrderList.Remove(strategyOrder);
                doneOrderList.Add(strategyOrder);
            }
        }

        //订单生成函数
        public StrategyOrder BuyOrder(int vol) {
            if (lastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = lastTickDic[mMainInstrument].UpperLimitPrice;
                return BuyOrder(vol, price);
            } else {
                return BuyOrder(0, 0);
            }

        }
        public StrategyOrder BuyOrder(int vol, double price) {
            return BuyOrder(vol, price, mMainInstrument);
        }
        public StrategyOrder BuyOrder(int vol, double price, Instrument instrument) {
            if (vol < 0) {
                return BuyOrder(0, price, instrument);
            }
            StrategyOrder strategyOrder = new StrategyOrder(this, instrument, DirectionType.Buy, price, vol);
            return strategyOrder;
        }
        public StrategyOrder SellOrder(int vol) {
            if (lastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = lastTickDic[mMainInstrument].LowerLimitPrice;
                return SellOrder(vol, price);
            } else {
                return SellOrder(0, 0);
            }

        }
        public StrategyOrder SellOrder(int vol, double price) {
            return SellOrder(vol, price, mMainInstrument);
        }
        public StrategyOrder SellOrder(int vol, double price, Instrument instrument) {
            if (vol < 0) {
                return SellOrder(0, price, instrument);
            }
            StrategyOrder strategyOrder = new StrategyOrder(this, instrument, DirectionType.Sell, price, vol);
            return strategyOrder;
        }
        public StrategyOrder ToPositionOrder(int position) {
            int myPos = GetPosition(mMainInstrument);
            if (position > myPos && lastTickDic.ContainsKey(mMainInstrument)) {
                return BuyOrder(position - myPos, lastTickDic[mMainInstrument].UpperLimitPrice, mMainInstrument);
            } else if (position < myPos && lastTickDic.ContainsKey(mMainInstrument)) {
                return SellOrder(myPos - position, lastTickDic[mMainInstrument].LowerLimitPrice, mMainInstrument);
            } else {
                return BuyOrder(0);
            }
        }
        public StrategyOrder ToPositionOrder(int position, double price) {
            return ToPositionOrder(position, price, mMainInstrument);
        }
        public StrategyOrder ToPositionOrder(int position, double price, Instrument instrument) {
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
