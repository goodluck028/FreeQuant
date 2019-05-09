using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Modules {

    [Component]
    class OrderManager {
        public OrderManager() {
            EventBus.Register(this);
            LogUtil.EnginLog("订单管理组件启动");
        }

        [OnEvent]
        private void OnOrderRequest(SendOrderRequest evt) {

        }

        [OnEvent]
        private void OnBrokerOrderEvent(BrokerOrderEvent evt) {

        }
    }
}
