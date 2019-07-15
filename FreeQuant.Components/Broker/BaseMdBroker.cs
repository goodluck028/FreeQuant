using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Components {

    [Component]
    public abstract class BaseMdBroker{
        public BaseMdBroker()
        {
            EventBus.Register(this);
            LogUtil.EnginLog("行情组件启动");
        }

        #region EventBus事件
        [OnEvent]
        private void OnMdBrokerLoginRequest(TdBrokerLoginRequest request) {
            Login();
        }

        [OnEvent]
        private void OnMdBrokerLogoutRequest(TdBrokerLogoutRequest request) {
            Logout();
        }

        [OnEvent]
        private void OnSubscribInstrumentRequest(SubscribeInstrumentRequest request) {
            SubscribeMarketData(request.Instrument);
        }

        [OnEvent]
        private void OnUnsubscribInstrumentRequest(UnsubscribeInstrumentRequest request) {
            UnSubscribeMarketData(request.Instrument);
        }
        #endregion

        #region 登录
        //登陆
        public abstract void Login();
        //登录结果事件
        protected void PostLoginEvent(bool isSuccess = true, string errorMsg = "") {
            MdBrokerLoginEvent evt = new MdBrokerLoginEvent(isSuccess, errorMsg);
            EventBus.PostEvent(evt);
        }
        //登出
        public abstract void Logout();
        //登出结果事件
        protected void PostLogoutEvent(bool isSuccess = true, string errorMsg = "") {
            MdBrokerLogoutEvent evt = new MdBrokerLogoutEvent(isSuccess, errorMsg);
            EventBus.PostEvent(evt);
        }
        #endregion

        #region 行情
        //订阅行情
        public abstract void SubscribeMarketData(Instrument Instrument);
        //退订行情
        public abstract void UnSubscribeMarketData(Instrument Instrument);
        //行情事件
        public void PostTickEvent(Tick tick) {
            TickEvent evt = new TickEvent(tick);
            EventBus.PostEvent(evt);
        }
        #endregion
    }
}
