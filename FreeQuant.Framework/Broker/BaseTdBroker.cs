using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {

    public enum ConnectionStatus
    {
        /// <summary>
        /// 连接已经断开
        /// </summary>
        Disconnected,
        /// <summary>
        /// 连接中...
        /// </summary>
        Connecting,
        /// <summary>
        /// 连接成功
        /// </summary>
        Connected
    }

    public abstract class BaseTdBroker {
        //订单事件
        protected Action<Order> mOnOrder;
        public event Action<Order> OnOrder {
            add {
                mOnOrder -= value;
                mOnOrder += value;
            }
            remove {
                mOnOrder -= value;
            }
        }
        //交易事件
        protected Action<BrokerTrade> mOnTrade;
        public event Action<BrokerTrade> OnTrade {
            add {
                mOnTrade -= value;
                mOnTrade += value;
            }
            remove {
                mOnTrade -= value;
            }
        }
        //合约事件
        protected Action<Instrument> mOnInstrument;
        public event Action<Instrument> OnInstrument {
            add {
                mOnInstrument -= value;
                mOnInstrument += value;
            }
            remove {
                mOnInstrument -= value;
            }
        }
        //持仓事件
        protected Action<BrokerPosition> mOnBroderPosition;
        public event Action<BrokerPosition> OnBroderPosition {
            add {
                mOnBroderPosition -= value;
                mOnBroderPosition += value;
            }
            remove {
                mOnBroderPosition -= value;
            }
        }
        //连接状态
        protected Action<ConnectionStatus> mOnStatusChanged;
        public event Action<ConnectionStatus> OnStatusChanged {
            add {
                mOnStatusChanged -= value;
                mOnStatusChanged += value;
            }
            remove {
                mOnStatusChanged -= value;
            }
        }

        //登陆
        public abstract void Login();

        //登出
        public abstract void Logout();

        //发送订单
        public abstract void SendOrder(Order order);

        //撤销订单
        public abstract void CancelOrder(Order order);

        //请求合约
        public abstract void QueryInstrument();

        //请求持仓
        public abstract void QueryPosition();

    }
}
