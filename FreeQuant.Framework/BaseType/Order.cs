using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    //订单委托
    public class Order {
        //ID
        private string mLocalId;
        //策略
        private BaseStrategy mStrategy;
        //合约
        private Instrument mInstrument;
        //买卖
        private DirectionType mDirection = DirectionType.Buy;
        //开平
        private OffsetType mOffset = OffsetType.Auto;
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

        //事件
        public event Action<Order> OnChanged;

        public Order(BaseStrategy strategy, Instrument instrument, DirectionType direction, OffsetType offset, double price, int volume) {
            mStrategy = strategy;
            mInstrument = instrument;
            mDirection = direction;
            mOffset = offset;
            mPrice = price;
            mOrderTime = DateTime.Now;
            mVolume = volume;
            mVolumeLeft = volume;
            mStatus = OrderStatus.Normal;
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

        public OffsetType Offset => mOffset;

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

        public int VolumeTraded {
            get { return mVolumeTraded; }
            set { mVolumeTraded = value; }
        }

        public int VolumeLeft {
            get {
                return mVolumeLeft;
            }
            set { mVolumeLeft = value; }
        }

        public OrderStatus Status {
            get {
                return mStatus;
            }
            set { mStatus = value; }
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

        public string LocalId {
            get { return mLocalId; }
            set { mLocalId = value; }
        }

        //
        internal void EmmitChange() {
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
        /// 自动
        /// </summary>
        Auto,
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
