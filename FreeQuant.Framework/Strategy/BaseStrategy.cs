﻿using FreeQuant.EventEngin;
using System;
using System.Collections.Generic;

namespace FreeQuant.Framework {
    public enum StrategyStatus { Starting, Ruing, Stoping, Stoped }

    //基本
    public partial class BaseStrategy {
        private string mName;
        private StrategyStatus mStatus = StrategyStatus.Stoped;

        public string Name {
            get { return mName ?? GetType().FullName; }
            protected set { }
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

        //合约列表
        private Instrument mMainInstrument;
        private HashSet<Instrument> mInstrumentSet = new HashSet<Instrument>();

        //最新tick
        private Dictionary<Instrument, Tick> lastTickDic = new Dictionary<Instrument, Tick>();

        //订阅行情
        internal void AddInstruments(params string[] instIds) {
            foreach (string instId in instIds) {
                Instrument inst = InstrumentManager.GetInstrument(instId);
                if (inst == null)
                    continue;
                //
                mMainInstrument = mMainInstrument ?? inst;
                mInstrumentSet.Add(inst);
            }
        }

        //启停信号
        [OnEvent]
        private void OnStart(StrategyEvent.StrategyStartRequest request) {
            if (GetType().FullName.Equals(request.TypeName) && mStatus == StrategyStatus.Stoped) {
                Start();
            }
        }
        [OnEvent]
        private void OnStop(StrategyEvent.StrategStopRequest request) {
            if (GetType().FullName.Equals(request.TypeName) && mStatus == StrategyStatus.Ruing) {
                Stop();
            }
        }

        internal void Start() {
            mStatus = StrategyStatus.Starting;
            OnStart();
            mStatus = StrategyStatus.Ruing;
            //
            foreach (Instrument inst in mInstrumentSet) {
                BrokerEvent.SubscribeInstrumentRequest request = new BrokerEvent.SubscribeInstrumentRequest(inst);
                EventBus.PostEvent(request);
            }
        }
        internal void Stop() {
            mStatus = StrategyStatus.Stoping;
            OnStop();
            mStatus = StrategyStatus.Stoped;
        }

        //tick
        [OnEvent]
        private void OnTick(BrokerEvent.TickEvent evt) {
            Tick tick = evt.Tick;
            if (mInstrumentSet.Contains(evt.Tick.Instrument)) {
                lastTickDic[tick.Instrument] = tick;
                OnTick(tick);
            }
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
            StrategyEvent.ChangePositionEvent evt = new StrategyEvent.ChangePositionEvent(p);
            EventBus.PostEvent(evt);
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
            mOrderManager.AddOrder(order);
            //
            StrategyEvent.SendOrderRequest request = new StrategyEvent.SendOrderRequest(order);
            EventBus.PostEvent(request);
        }

        //撤单
        internal void CancleOrder(Order order) {
            StrategyEvent.CancelOrderRequest request = new StrategyEvent.CancelOrderRequest(order);
            EventBus.PostEvent(request);
        }

        //订单生成函数
        public Order BuyOrder(int vol, OffsetType offset = OffsetType.Auto) {
            if (lastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = lastTickDic[mMainInstrument].UpperLimitPrice;
                return BuyOrder(vol, price, offset);
            } else {
                return BuyOrder(0, 0);
            }

        }
        public Order BuyOrder(int vol, double price, OffsetType offset = OffsetType.Auto) {
            return BuyOrder(vol, price, mMainInstrument, offset);
        }
        public Order BuyOrder(int vol, double price, Instrument instrument, OffsetType offset = OffsetType.Auto) {
            if (vol < 0) {
                return BuyOrder(0, price, instrument);
            }
            Order order = new Order(this, instrument, DirectionType.Buy, offset, price, vol);
            return order;
        }
        public Order SellOrder(int vol, OffsetType offset = OffsetType.Auto) {
            if (lastTickDic.ContainsKey(mMainInstrument)) {
                double price = 0;
                price = lastTickDic[mMainInstrument].LowerLimitPrice;
                return SellOrder(vol, price, offset);
            } else {
                return SellOrder(0, 0);
            }

        }
        public Order SellOrder(int vol, double price, OffsetType offset = OffsetType.Auto) {
            return SellOrder(vol, price, mMainInstrument, offset);
        }
        public Order SellOrder(int vol, double price, Instrument instrument, OffsetType offset = OffsetType.Auto) {
            if (vol < 0) {
                return SellOrder(0, price, instrument);
            }
            Order order = new Order(this, instrument, DirectionType.Sell, offset, price, vol);
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

}