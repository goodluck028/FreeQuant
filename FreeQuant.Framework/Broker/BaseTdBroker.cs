using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {

    public abstract class BaseTdBroker {
        //日志事件
        public Action<string> OnLog;
        //订单事件
        public Action<Order> OnOrder;
        //交易事件
        public Action<Order, long> OnTrade;
        //合约事件
        public Action<Instrument> OnInstrument;
        //持仓事件
        public Action<BrokerPosition> OnBroderPosition;

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
