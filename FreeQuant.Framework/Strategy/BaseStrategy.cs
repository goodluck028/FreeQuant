using System;
using System.Collections.Generic;

namespace FreeQuant.Framework {
    public enum StrategyStatus { Starting, Ruing, Stoping, Stoped }

    //基本
    public partial class BaseStrategy {
        private string mName;
        private bool mEnable = false;
        private StrategyStatus mStatus = StrategyStatus.Stoped;

        public string Name {
            get { return mName ?? GetType().FullName; }
        }

        public bool Enable
        {
            get { return mEnable; }
            set { mEnable = value; }
        }

        //写日志
        public void Log(string content) {
            LogUtil.UserLog(content);
        }
    }

    //用户实现
    public partial class BaseStrategy {
        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected virtual void OnTick(Tick tick) { }
        protected virtual void OnPositionChanged(Position position) { }
    }

    //行情
    public partial class BaseStrategy {
        BaseMdBroker mMdBroker = BrokerManager.DefaultMdBroker;

        //合约列表
        private Instrument mMainInstrument;
        private HashSet<Instrument> mInstrumentSet = new HashSet<Instrument>();

        //最新tick
        private Dictionary<Instrument, Tick> lastTickDic = new Dictionary<Instrument, Tick>();

        //启停信号
        private void EmitStartEvent() {
            if (mStatus == StrategyStatus.Stoped) {
                Start();
            }
        }
        private void EmitStopEvent() {
            if (mStatus == StrategyStatus.Ruing) {
                Stop();
            }
        }

        internal void Start() {
            foreach (Attribute attr in GetType().GetCustomAttributes(true)) {
                //策略名
                if (attr is StrategyNameAttribute) {
                    mName = (attr as StrategyNameAttribute).Name;
                }
                //加载合约
                if (attr is InstrumentsAttribute) {
                    string[] instIds = (attr as InstrumentsAttribute).Instruments;
                    foreach (string instId in instIds) {
                        Instrument inst = InstrumentManager.GetInstrument(instId);
                        if (inst == null)
                            continue;
                        //主合约与去重
                        mMainInstrument = mMainInstrument ?? inst;
                        mInstrumentSet.Add(inst);
                    }
                }
            }
            //启动过程
            mStatus = StrategyStatus.Starting;
            OnStart();
            mStatus = StrategyStatus.Ruing;
            //订阅合约
            foreach (Instrument inst in mInstrumentSet) {
                mMdBroker.SubscribeMarketData(inst.InstrumentID);
            }
        }
        internal void Stop() {
            mStatus = StrategyStatus.Stoping;
            OnStop();
            mStatus = StrategyStatus.Stoped;
        }

        //tick
        private void EmitTickEvent(Tick tick) {
            if (mInstrumentSet.Contains(tick.Instrument)) {
                lastTickDic[tick.Instrument] = tick;
                OnTick(tick);
            }
        }
    }

    //交易
    public partial class BaseStrategy {
        BaseTdBroker mTdBroker = BrokerManager.DefaultTdBroker;

        //持仓
        private Dictionary<string, int> mPositionMap = new Dictionary<string, int>();
        public int GetPosition(string instId) {
            if (mPositionMap.ContainsKey(instId)) {
                return mPositionMap[instId];
            } else {
                return 0;
            }
        }
        internal void AddPosition(string instId, int vol) {
            mPositionMap[instId] = mPositionMap.ContainsKey(instId) ? mPositionMap[instId] + vol : vol;
            PositionManager.SetPosition(GetType().FullName, instId, vol);
        }

        //订单管理
        OrderHolder mOrderHolder = new OrderHolder();

        //发送订单
        protected void SendOrder(Order order) {
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
            mOrderHolder.AddOrder(order);
            //

            mTdBroker.SendOrder(order);
        }

        //撤单
        protected void CancleOrder(Order order) {
            mTdBroker.CancelOrder(order);
        }

        //订单生成函数
        protected Order BuyOrder(int vol, OffsetType offset = OffsetType.Auto) {
            if (lastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = lastTickDic[mMainInstrument].UpperLimitPrice;
                return BuyOrder(vol, price, offset);
            } else {
                return BuyOrder(0, 0);
            }

        }
        protected Order BuyOrder(int vol, double price, OffsetType offset = OffsetType.Auto) {
            return BuyOrder(vol, price, mMainInstrument, offset);
        }
        protected Order BuyOrder(int vol, double price, Instrument instrument, OffsetType offset = OffsetType.Auto) {
            if (vol < 0) {
                return BuyOrder(0, price, instrument);
            }
            Order order = new Order(this, instrument, DirectionType.Buy, offset, price, vol);
            return order;
        }
        protected Order SellOrder(int vol, OffsetType offset = OffsetType.Auto) {
            if (lastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = lastTickDic[mMainInstrument].LowerLimitPrice;
                return SellOrder(vol, price, offset);
            } else {
                return SellOrder(0, 0);
            }

        }
        protected Order SellOrder(int vol, double price, OffsetType offset = OffsetType.Auto) {
            return SellOrder(vol, price, mMainInstrument, offset);
        }
        protected Order SellOrder(int vol, double price, Instrument instrument, OffsetType offset = OffsetType.Auto) {
            if (vol < 0) {
                return SellOrder(0, price, instrument);
            }
            Order order = new Order(this, instrument, DirectionType.Sell, offset, price, vol);
            return order;
        }
        protected Order ToPositionOrder(int position) {
            int myPos = GetPosition(mMainInstrument.InstrumentID);
            if (position > myPos && lastTickDic.ContainsKey(mMainInstrument)) {
                return BuyOrder(position - myPos, lastTickDic[mMainInstrument].UpperLimitPrice, mMainInstrument);
            } else if (position < myPos && lastTickDic.ContainsKey(mMainInstrument)) {
                return SellOrder(myPos - position, lastTickDic[mMainInstrument].LowerLimitPrice, mMainInstrument);
            } else {
                return BuyOrder(0);
            }
        }
        protected Order ToPositionOrder(int position, double price) {
            return ToPositionOrder(position, price, mMainInstrument);
        }
        protected Order ToPositionOrder(int position, double price, Instrument instrument) {
            int myPos = GetPosition(mMainInstrument.InstrumentID);
            if (position > myPos) {
                return BuyOrder(position - myPos, price, instrument);
            } else if (position < myPos) {
                return SellOrder(myPos - position, price, instrument);
            } else {
                return BuyOrder(0);
            }
        }
    }

}
