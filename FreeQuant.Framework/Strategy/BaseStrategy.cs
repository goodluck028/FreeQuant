using System;
using System.Collections.Generic;

namespace FreeQuant.Framework {
    /// <summary>
    /// 策略状态
    /// </summary>
    public enum StrategyStatus { Starting, Ruing, Stoping, Stoped }

    /// <summary>
    /// 为策略配置合约的特性类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InstrumentsAttribute : Attribute
    {
        public InstrumentsAttribute(params string[] instruments)
        {
            Instruments = instruments;
        }

        public string[] Instruments { get; }
    }

    /// <summary>
    /// 为策略配置名称的特性类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class StrategyNameAttribute : Attribute
    {
        public StrategyNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    /// <summary>
    /// 基本字段
    /// </summary>
    public partial class BaseStrategy {
        private string mName;
        private StrategyStatus mStatus = StrategyStatus.Stoped;

        public string Name {
            get { return mName ?? GetType().FullName; }
        }

        public bool Enable { get; set; } = false;

        //写日志
        public void Log(string content) {
            LogUtil.UserLog(content);
        }
    }

    /// <summary>
    /// 用户实现
    /// </summary>
    public partial class BaseStrategy {
        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected virtual void OnTick(Tick tick) { }
        protected virtual void OnPositionChanged(StrategyPosition position) { }
    }

    /// <summary>
    /// 行情
    /// </summary>
    public partial class BaseStrategy {
        BaseMdBroker mMdBroker = Quanter.BrokerManager.DefaultMdBroker;

        //合约列表
        private Instrument mMainInstrument;
        private HashSet<Instrument> mInstrumentSet = new HashSet<Instrument>();

        //最新tick
        private Dictionary<Instrument, Tick> mLastTickDic = new Dictionary<Instrument, Tick>();

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

        internal bool Start() {
            foreach (Attribute attr in GetType().GetCustomAttributes(true)) {
                //策略名
                if (attr is StrategyNameAttribute) {
                    mName = (attr as StrategyNameAttribute).Name;
                }
                //加载合约
                if (attr is InstrumentsAttribute) {
                    string[] instIDs = (attr as InstrumentsAttribute).Instruments;
                    foreach (string instID in instIDs) {
                        Instrument inst = Quanter.InstrumentManager.GetInstrument(instID);
                        if (inst == null) {
                            LogUtil.SysLog($"合约{instID}缺失");
                            return false;
                        }
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
                mMdBroker.SubscribeMarketData(inst);
            }
            return true;
        }
        internal void Stop() {
            mStatus = StrategyStatus.Stoping;
            OnStop();
            mStatus = StrategyStatus.Stoped;
        }

        //tick
        private void EmitTickEvent(Tick tick) {
            if (mInstrumentSet.Contains(tick.Instrument)) {
                mLastTickDic[tick.Instrument] = tick;
                OnTick(tick);
            }
        }
    }

    /// <summary>
    /// 交易
    /// </summary>
    public partial class BaseStrategy {
        BaseTdBroker mTdBroker = Quanter.BrokerManager.DefaultTdBroker;

        //持仓
        private Dictionary<string, int> mPositionMap = new Dictionary<string, int>();
        public int GetPosition(string instID) {
            if (mPositionMap.ContainsKey(instID)) {
                return mPositionMap[instID];
            } else {
                return 0;
            }
        }
        internal void AddPosition(string instID, int vol) {
            mPositionMap[instID] = mPositionMap.ContainsKey(instID) ? mPositionMap[instID] + vol : vol;
            Quanter.PositionManager.SetPosition(GetType().FullName, instID, vol);
        }

        //订单管理
        OrderHolder mOrderHolder = new OrderHolder();

        //发送订单
        protected void SendOrder(Order order) {
            Tick t;
            double uper = 0;
            double lower = 0;
            if (mLastTickDic.TryGetValue(order.Instrument, out t)) {
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
        protected Order BuyOrder(int vol, OpenCloseType offset = OpenCloseType.Auto) {
            if (mLastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = mLastTickDic[mMainInstrument].UpperLimitPrice;
                return BuyOrder(vol, price, offset);
            } else {
                return BuyOrder(0, 0);
            }

        }
        protected Order BuyOrder(int vol, double price, OpenCloseType offset = OpenCloseType.Auto) {
            return BuyOrder(vol, price, mMainInstrument, offset);
        }
        protected Order BuyOrder(int vol, double price, Instrument instrument, OpenCloseType offset = OpenCloseType.Auto) {
            if (vol < 0) {
                return BuyOrder(0, price, instrument);
            }
            Order order = new Order(this, instrument, DirectionType.Buy, offset, price, vol);
            return order;
        }
        protected Order SellOrder(int vol, OpenCloseType offset = OpenCloseType.Auto) {
            if (mLastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = mLastTickDic[mMainInstrument].LowerLimitPrice;
                return SellOrder(vol, price, offset);
            } else {
                return SellOrder(0, 0);
            }

        }
        protected Order SellOrder(int vol, double price, OpenCloseType offset = OpenCloseType.Auto) {
            return SellOrder(vol, price, mMainInstrument, offset);
        }
        protected Order SellOrder(int vol, double price, Instrument instrument, OpenCloseType offset = OpenCloseType.Auto) {
            if (vol < 0) {
                return SellOrder(0, price, instrument);
            }
            Order order = new Order(this, instrument, DirectionType.Sell, offset, price, vol);
            return order;
        }
        protected Order ToPositionOrder(int position) {
            int myPos = GetPosition(mMainInstrument.InstrumentID);
            if (position > myPos && mLastTickDic.ContainsKey(mMainInstrument)) {
                return BuyOrder(position - myPos, mLastTickDic[mMainInstrument].UpperLimitPrice, mMainInstrument);
            } else if (position < myPos && mLastTickDic.ContainsKey(mMainInstrument)) {
                return SellOrder(myPos - position, mLastTickDic[mMainInstrument].LowerLimitPrice, mMainInstrument);
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
