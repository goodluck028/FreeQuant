using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;
using FreeQuant.Modules.Broker;

namespace FreeQuant.Modules {
    public abstract class BaseTdBroker : BaseModule {

        public override void OnLoad() {
            EventBus.Register(this);
            LogUtil.EnginLog("交易模块启动");
        }

        #region EventBus事件
        [OnEvent]
        private void OnTdBrokerLoginRequest(TdBrokerLoginRequest request) {
            Login();
        }

        [OnEvent]
        private void OnTdBrokerLogoutRequest(TdBrokerLogoutRequest request) {
            Logout();
        }

        [OnEvent]
        private void OnQueryInstrumentRequest(QueryInstrumentRequest request) {
            QueryInstrument();
        }

        [OnEvent]
        private void OnSendOrderRequest(SendOrderRequest request) {
            SendOrder(request.Order);
        }

        [OnEvent]
        private void OnCancelOrderRequest(CancelOrderRequest request) {
            CancelOrder(request.Order);
        }
        #endregion

        #region 登录
        //登陆
        protected abstract void Login();
        //登录结果事件
        protected void PostLoginEvent(bool isSuccess = true, string errorMsg = "") {
            TdBrokerLoginEvent evt = new TdBrokerLoginEvent(isSuccess, errorMsg);
            EventBus.PostEvent(evt);
        }
        //登出
        protected abstract void Logout();
        //登出结果事件
        protected void PostLogoutEvent(bool isSuccess = true, string errorMsg = "") {
            TdBrokerLogoutEvent evt = new TdBrokerLogoutEvent(isSuccess, errorMsg);
            EventBus.PostEvent(evt);
        }
        #endregion

        #region 订单
        //发送订单
        protected abstract void SendOrder(BrokerOrder order);
        //撤销订单
        protected abstract void CancelOrder(BrokerOrder order);
        //订单事件
        protected void PostOrderEvent(BrokerOrder order) {
            BrokerOrderEvent evt = new BrokerOrderEvent(order);
            EventBus.PostEvent(evt);
        }
        #endregion

        #region 合约
        //请求合约
        protected abstract void QueryInstrument();
        protected void PostInstrumentEvent(Instrument inst) {
            InstrumentReturnEvent evt = new InstrumentReturnEvent(inst);
            EventBus.PostEvent(evt);
        }
        #endregion



    }
}
