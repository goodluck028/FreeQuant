using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.EventEngin;

namespace FreeQuant.Framework {

    internal class OrderManager {
        private List<Order> mOrders = new List<Order>();
        private List<Order> mActiveOrders = new List<Order>();
        private List<Order> mDoneOrders = new List<Order>();

        //添加订单
        public void AddOrder(Order order) {
            mOrders.Add(order);
            mActiveOrders.Add(order);
            //
            order.OnChanged += UpdateOrder;
        }

        //更新订单
        public void UpdateOrder(Order order) {
            if (order.Status == OrderStatus.Canceled
                || order.Status == OrderStatus.Error
                || order.Status == OrderStatus.Filled) {
                mActiveOrders.Remove(order);
                mDoneOrders.Add(order);
            }
        }
    }
}
