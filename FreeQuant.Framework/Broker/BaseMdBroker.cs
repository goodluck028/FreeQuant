using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {

    public abstract class BaseMdBroker {

        //日志事件
        public Action<string> OnLog;
        //行情事件
        public Action<Tick> OnTick;

        //登陆
        public abstract void Login();
        //登出
        public abstract void Logout();
        //订阅行情
        public abstract void SubscribeMarketData(string instId);
        //退订行情
        public abstract void UnSubscribeMarketData(string instId);
        //行情事件
    }
}
