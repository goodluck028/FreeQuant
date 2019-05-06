﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    //订单委托
    public class Order {
        //策略
        private BaseStrategy mStrategy;
        //合约
        private Instrument mInstrument;
        //买卖
        private DirectionType mDirection = DirectionType.Buy;
        //报价
        private double mPrice = 0d;
        //报单时间(本地时间)
        private DateTime mOrderTime = DateTime.Now;
        //报单数量
        private int mVolume = 0;
        //成交数量
        private int mVolumeTraded = 0;
        //未成交
        private int mVolumeLeft = 0;
        //状态
        private OrderStatus mStatus = OrderStatus.Normal;
        //子订单
        private List<BrokerOrder> mSubOrders = new List<BrokerOrder>();

        //事件
        public event Action<Order> OnChanged;

        public Order(BaseStrategy strategy, Instrument instrument, DirectionType direction, double price, int volume) {
            this.mStrategy = strategy;
            this.mInstrument = instrument;
            this.mDirection = direction;
            this.mPrice = price;
            this.mOrderTime = DateTime.Now;
            this.mVolume = volume;
            this.mVolumeLeft = volume;
            this.mStatus = OrderStatus.Normal;
        }

        //
        public Instrument Instrument {
            get {
                return mInstrument;
            }
        }

        public DirectionType Direction {
            get {
                return mDirection;
            }
        }

        public double Price {
            internal set {
                mPrice = value;
            }
            get {
                return mPrice;
            }
        }

        public int Volume {
            get {
                return mVolume;
            }
        }

        public int VolumeLeft {
            get {
                return mVolumeLeft;
            }
        }

        public OrderStatus Status {
            get {
                return mStatus;
            }
        }

        public DateTime OrderTime {
            get {
                return mOrderTime;
            }
        }

        public BaseStrategy Strategy {
            get {
                return mStrategy;
            }
        }

        internal List<BrokerOrder> SubOrders {
            get {
                return mSubOrders;
            }
        }

        //
        internal void AddSubOrder(BrokerOrder o) {
            mSubOrders.Add(o);
        }
        internal void RefreshSubOrders() {
            if (mSubOrders == null)
                return;
            if (mStatus == OrderStatus.Canceled)
                return;
            if (mStatus == OrderStatus.Error)
                return;
            if (mStatus == OrderStatus.Filled)
                return;

            int left = 0; //未成
            int traded = 0; //已成
            int currentTrade = 0; //当前成交

            int normalCount = 0;
            int partialCount = 0;
            int filledCount = 0;
            int errorCount = 0;
            int cancledCount = 0;

            /*统计，这个地方有点复杂，会出现子订单一个正常，一个错误，一个撤单这种情况，
             * partial>normal>canceled
             */
            foreach (BrokerOrder brokerOrder in mSubOrders) {
                //
                left += brokerOrder.VolumeLeft;
                traded += brokerOrder.VolumeTraded;
                //
                switch (brokerOrder.Status) {
                    case OrderStatus.Normal:
                        normalCount++;
                        break;
                    case OrderStatus.Partial:
                        partialCount++;
                        break;
                    case OrderStatus.Filled:
                        filledCount++;
                        break;
                    case OrderStatus.Error:
                        errorCount++;
                        break;
                    case OrderStatus.Canceled:
                        cancledCount++;
                        break;
                }
            }

            mVolumeLeft = left;
            currentTrade = traded - mVolumeTraded;
            mVolumeTraded = traded;

            //策略持仓调整
            if (currentTrade > 0) {
                if (mDirection == DirectionType.Buy) {
                    mStrategy.AddPosition(mInstrument, currentTrade);
                } else {
                    mStrategy.AddPosition(mInstrument, -currentTrade);
                }
            }

            //状态
            if (partialCount > 0) {
                mStatus = OrderStatus.Partial;
            } else if (normalCount > 0) {
                mStatus = OrderStatus.Normal;
            } else if (filledCount == mSubOrders.Count) {
                mStatus = OrderStatus.Filled;
            } else if (errorCount == mSubOrders.Count) {
                mStatus = OrderStatus.Error;
            } else {
                mStatus = OrderStatus.Canceled;
            }

            //订单状态变化
            mStrategy.UpdateOrder(this);
            OnChanged?.Invoke(this);
        }

        //发送订单
        public void Send() {
            mStrategy.SendOrder(this);
        }

        public void Cancle() {
            mStrategy.CancleOrder(this);
        }
    }

    public class BrokerOrder {
        //合并订单
        private Order mPOrder;

        //发单标识
        private string localID;

        // 报单标识
        private string orderID;

        // 合约
        private string instrumentID;

        // 买卖
        private DirectionType direction;

        // 开平
        private OffsetType offset;

        // 报价
        private double limitPrice;

        // 报单时间(交易所)
        private DateTime insertTime;

        // 报单数量
        private int volume;

        //成交数量
        private int volumeTraded;

        // 未成交,trade更新
        private int volumeLeft;

        // 状态
        private OrderStatus status;

        public BrokerOrder(Order pOrder, string instrumentID, DirectionType direction, OffsetType offset, double limitPrice, DateTime insertTime, int volume, int volumeLeft, OrderStatus status) {
            this.mPOrder = pOrder;
            this.instrumentID = instrumentID;
            this.direction = direction;
            this.offset = offset;
            this.limitPrice = limitPrice;
            this.insertTime = insertTime;
            this.volume = volume;
            this.volumeLeft = volumeLeft;
            this.status = status;
        }

        internal void Refresh() {
            mPOrder.RefreshSubOrders();
        }

        public Order POrder => mPOrder;

        public string InstrumentId => instrumentID;

        public DirectionType Direction => direction;

        public OffsetType Offset => offset;

        public double LimitPrice => limitPrice;

        public int Volume => volume;

        public string LocalId
        {
            get { return localID; }
            set { localID = value; }
        }

        public string OrderId
        {
            get { return orderID; }
            set { orderID = value; }
        }

        public DateTime InsertTime
        {
            get { return insertTime; }
            set { insertTime = value; }
        }

        public int VolumeTraded
        {
            get { return volumeTraded; }
            set { volumeTraded = value; }
        }

        public int VolumeLeft
        {
            get { return volumeLeft; }
            set { volumeLeft = value; }
        }

        public OrderStatus Status
        {
            get { return status; }
            set { status = value; }
        }
    }

    public enum DirectionType {
        /// <summary>
        /// 买
        /// </summary>
        Buy,

        /// <summary>
        /// 卖
        /// </summary>
        Sell
    }

    public enum OrderStatus {
        /// <summary>
        /// 委托
        /// </summary>
        Normal,
        /// <summary>
        /// 部成
        /// </summary>
        Partial,
        /// <summary>
        /// 全成
        /// </summary>
        Filled,
        /// <summary>
        /// 撤单[某些"被拒绝"的委托也会触发此状态]
        /// </summary>
        Canceled,
        /// <summary>
        /// 错误
        /// </summary>
        Error
    }

    public enum OffsetType {
        /// <summary>
        /// 开仓
        /// </summary>
        Open,
        /// <summary>
        /// 平仓
        /// </summary>
        Close,
        /// <summary>
        /// 平今
        /// </summary>
        CloseToday
    }
}