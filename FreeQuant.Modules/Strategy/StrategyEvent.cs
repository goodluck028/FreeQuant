using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    public class SendOrderRequest {
        private Order mOrder;

        public SendOrderRequest(Order order) {
            mOrder = order;
        }

        public Order Order => mOrder;
    }

    public class OrderEvent {
        private Order mOrder;

        public OrderEvent(Order order) {
            mOrder = order;
        }

        public Order Order => mOrder;
    }
}
