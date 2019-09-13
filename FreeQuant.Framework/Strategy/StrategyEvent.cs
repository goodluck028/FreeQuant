using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class StrategyEvent
    {

        public class SendOrderRequest
        {
            private Order mOrder;

            public SendOrderRequest(Order order)
            {
                mOrder = order;
            }

            public Order Order => mOrder;
        }

        public class CancelOrderRequest
        {
            private Order mOrder;

            public CancelOrderRequest(Order order)
            {
                mOrder = order;
            }

            public Order Order => mOrder;
        }

        public class ChangePositionEvent
        {
            private Position mPosition;

            public ChangePositionEvent(Position position)
            {
                mPosition = position;
            }

            public Position Position => mPosition;
        }

        public class OrderEvent
        {
            private Order mOrder;

            public OrderEvent(Order order)
            {
                mOrder = order;
            }

            public Order Order => mOrder;
        }

        public class StrategyStartRequest
        {
            private string mTypeName;

            public StrategyStartRequest(string typeName)
            {
                mTypeName = typeName;
            }

            public string TypeName => mTypeName;
        }

        public class StrategStopRequest {
            private string mTypeName;

            public StrategStopRequest(string typeName) {
                mTypeName = typeName;
            }

            public string TypeName => mTypeName;
        }
    }
}
