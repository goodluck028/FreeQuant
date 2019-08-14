using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;
using FreeQuant.Components;

namespace FreeQuant.DataReceiver {
    public class ReceiverComponentsScheduler {
        public static ReceiverComponentsScheduler mInstance;
        private ReceiverComponentsScheduler() {
            EventBus.Register(this);
            start();
        }
        public static void Run() {
            if (mInstance == null) {
                mInstance = new ReceiverComponentsScheduler();
            }
        }

        private void start()
        {
            //登录交易
            BrokerEvent.TdBrokerLoginRequest request = new BrokerEvent.TdBrokerLoginRequest();
            EventBus.PostEvent(request);
        }

        [OnEvent]
        private void OnTdLogin(BrokerEvent.TdBrokerLoginEvent evt)
        {
            //todo 查询合约

            //登录行情
            BrokerEvent.MdBrokerLoginRequest request = new BrokerEvent.MdBrokerLoginRequest();
            EventBus.PostEvent(request);
        }

        //响应行情登录成功
        [OnEvent]
        private void OnMdLogin(BrokerEvent.MdBrokerLoginEvent evt)
        {
        }
    }
}
