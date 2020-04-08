using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
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
