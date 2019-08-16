using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Components {

    public abstract class BaseTdBroker {
        public BaseTdBroker() {
            EventBus.Register(this);
            LogUtil.EnginLog("交易组件启动");
        }

        #region EventBus事件
        [OnEvent]
        protected void OnTdBrokerLoginRequest(BrokerEvent.TdLoginRequest request) {
            Login();
        }

        [OnEvent]
        protected void OnTdBrokerLogoutRequest(BrokerEvent.TdLogoutRequest request) {
            Logout();
        }

        [OnEvent]
        protected void OnQueryInstrumentRequest(BrokerEvent.QueryInstrumentRequest request) {
            QueryInstrument();
        }

        [OnEvent]
        protected void OnQueryPosition(BrokerEvent.QueryPositionRequest request) {
            QueryPosition();
        }

        [OnEvent]
        protected void OnSendOrderRequest(StrategyEvent.SendOrderRequest request) {
            SendOrder(request.Order);
        }

        [OnEvent]
        protected void OnCancelOrderRequest(StrategyEvent.CancelOrderRequest request) {
            CancelOrder(request.Order);
        }
        #endregion

        #region 登录
        //登陆
        protected abstract void Login();
        //登录结果事件
        protected void PostLoginEvent(bool isSuccess = true, string errorMsg = "") {
            BrokerEvent.TdLoginEvent evt = new BrokerEvent.TdLoginEvent(isSuccess, errorMsg);
            EventBus.PostEvent(evt);
        }
        //登出
        protected abstract void Logout();
        //登出结果事件
        protected void PostLogoutEvent(bool isSuccess = true, string errorMsg = "") {
            BrokerEvent.TdLogoutEvent evt = new BrokerEvent.TdLogoutEvent(isSuccess, errorMsg);
            EventBus.PostEvent(evt);
        }
        #endregion

        #region 订单
        //发送订单
        protected abstract void SendOrder(Order order);
        //撤销订单
        protected abstract void CancelOrder(Order order);
        //
        protected void PostOrderEvent(Order order) {
            StrategyEvent.OrderEvent evt = new StrategyEvent.OrderEvent(order);
            EventBus.PostEvent(evt);
            //
            order.EmmitChange();
        }

        protected void PostTradeEvent(Order order, long tradeVol) {
            BrokerEvent.TradeEvent evt = new BrokerEvent.TradeEvent(order, tradeVol);
            EventBus.PostEvent(evt);
        }
        #endregion

        #region 合约
        //请求合约
        protected abstract void QueryInstrument();
        protected void PostInstrumentEvent(Instrument inst) {
            BrokerEvent.InstrumentEvent evt = new BrokerEvent.InstrumentEvent(inst);
            EventBus.PostEvent(evt);
        }
        #endregion

        #region 持仓
        protected abstract void QueryPosition();

        protected void PostPositionEvent(BrokerEvent.PositionEvent evt) {
            EventBus.PostEvent(evt);
        }
        #endregion

    }
}
