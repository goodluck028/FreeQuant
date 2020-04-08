using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {

    public abstract class BaseMdBroker {
        //行情事件
        protected Action<Tick> mOnTick;
        public event Action<Tick> OnTick {
            add {
                mOnTick -= value;
                mOnTick += value;
            }
            remove {
                mOnTick -= value;
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
        //订阅行情
        public abstract void SubscribeMarketData(Instrument inst);
        //退订行情
        public abstract void UnSubscribeMarketData(Instrument inst);
        //行情事件
    }
}
