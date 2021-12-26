using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {

    public enum DirectionType
    {
        /// <summary>
        /// 买
        /// </summary>
        Buy,

        /// <summary>
        /// 卖
        /// </summary>
        Sell
    }

    public enum OpenCloseType
    {
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

    public enum OrderStatus
    {
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

    //订单委托
    public class Order {
        //ID
        public string OrderId { get; set; }
        //策略
        public BaseStrategy Strategy { get; }
        //合约
        public Instrument Instrument { get; }
        //买卖
        public DirectionType Direction { get; }
        //开平
        public OpenCloseType OpenClose { get; internal set; }
        //报价
        public double Price { get; internal set; }
        //报单时间(本地时间)
        public DateTime OrderTime { get; }
        //报单数量
        public int Qty { get; }
        //成交数量
        public int QtyTraded { get; set; }
        //未成交
        public int QtyLeft { get; set; }
        //状态
        public OrderStatus Status { get; set; }

        //事件
        private Action<Order> mOnChanged;
        public event Action<Order> OnChanged {
            add {
                mOnChanged -= value;
                mOnChanged += value;
            }
            remove {
                mOnChanged -= value;
            }
        }

        public Order(BaseStrategy strategy, Instrument inst, DirectionType direction, OpenCloseType openClose, double price, int volume) {
            Strategy = strategy;
            Instrument = inst;
            Direction = direction;
            OpenClose = openClose;
            Price = price;
            OrderTime = DateTime.Now;
            Qty = volume;
            QtyLeft = volume;
            Status = OrderStatus.Normal;
        }

        //
        internal void EmmitChange() {
            mOnChanged?.Invoke(this);
        }
    }
}
