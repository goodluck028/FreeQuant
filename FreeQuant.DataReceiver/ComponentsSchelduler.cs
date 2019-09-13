using FreeQuant.EventEngin;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    internal class ComponentsSchelduler {
        private static ComponentsSchelduler mInstance = new ComponentsSchelduler();
        public static ComponentsSchelduler Instance => mInstance;
        private ComponentsSchelduler() {
            EventBus.Register(this);
        }

        public void Stop() {
            EventBus.Stop();
        }

        public void start() {
            //登录行情
            BrokerEvent.MdLoginRequest request = new BrokerEvent.MdLoginRequest();
            EventBus.PostEvent(request);
        }

        //行情登录成功
        [OnEvent]
        private void OnMdLogin(BrokerEvent.MdLoginEvent evt) {
            //登录交易
            BrokerEvent.TdLoginRequest request = new BrokerEvent.TdLoginRequest();
            EventBus.PostEvent(request);
        }

        //交易登录成功
        [OnEvent]
        private void OnTdLogin(BrokerEvent.TdLoginEvent evt) {
            //查询合约
            BrokerEvent.QueryInstrumentRequest request = new BrokerEvent.QueryInstrumentRequest();
            EventBus.PostEvent(request);
        }

        //合约返回
        [OnEvent]
        private void OnInstrument(BrokerEvent.InstrumentEvent evt) {
            //订阅合约
            string[] names = DataBaseConfig.Config.Instruments.Split(',');
            foreach (string name in names) {
                if (name.Equals(RegexUtils.TakeProductName(evt.Instrument.InstrumentID))) {
                    Instrument inst = evt.Instrument;
                    BrokerEvent.SubscribeInstrumentRequest request = new BrokerEvent.SubscribeInstrumentRequest(inst);
                    EventBus.PostEvent(request);
                }
            }
        }


    }
}
