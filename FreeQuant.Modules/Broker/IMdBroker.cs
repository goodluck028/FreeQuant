using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    public interface IMdProvider
    {
        //登陆
        void Login();
        //登出
        void Logout();
        //是否登陆
        bool IsLogin { get; }
        //订阅行情
        void SubscribeMarketData(String InstrumentID);
        //退订行情
        void UnSubscribeMarketData(String InstrumentID);
        //行情回调
        event Action<Tick> OnTick;
    }
}
