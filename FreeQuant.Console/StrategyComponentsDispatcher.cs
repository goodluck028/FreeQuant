using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;
using FreeQuant.Components;

namespace FreeQuant.Console {
    public class StrategyComponentsDispatcher {
        public static StrategyComponentsDispatcher mInstance;
        private StrategyComponentsDispatcher() {
            EventBus.Register(this);
            //登录行情
            MdBrokerLoginRequest request = new MdBrokerLoginRequest();
            EventBus.PostEvent(request);
        }
        public static void Run() {
            if (mInstance == null) {
                mInstance = new StrategyComponentsDispatcher();
            }
        }

        //todo 响应行情登录成功
        [OnEvent]
        public void OnMdLogin(MdBrokerLoginEvent evt)
        {

        }
    }
}
