using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;
using FreeQuant.Components;

namespace FreeQuant.Console {
    public class StrategyComponentsCommander {
        public static StrategyComponentsCommander mInstance;
        private StrategyComponentsCommander() {
            EventBus.Register(this);
        }
        public static void Begin() {
            if (mInstance == null) {
                mInstance = new StrategyComponentsCommander();
            }
        }

        private void start()
        {
            //登录行情
            BrokerEvent.MdLoginRequest request = new BrokerEvent.MdLoginRequest();
            EventBus.PostEvent(request);
        }

        //响应行情登录成功
        [OnEvent]
        private void OnMdLogin(BrokerEvent.MdLoginEvent evt)
        {

        }
    }
}
