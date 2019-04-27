using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FreeQuant.Modules;
using FreeQuant.Modules.Broker;
using XAPI.Callback;
using XAPI;

namespace Modules.Xapi2 {
    public class XapiMdBroker : BaseMdBroker {
        string mdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CTP_Quote_x86.dll");
        private Account mAccount = ConfigUtil.Config.MyMdAccount;
        XApi mdApi;

        public override void Login() {
            mdApi = new XApi(mdPath);
            mdApi.Server.Address = mAccount.Server;
            mdApi.Server.BrokerID = mAccount.Broker;
            mdApi.User.UserID = mAccount.Investor;
            mdApi.User.Password = mAccount.Password;
            mdApi.OnConnectionStatus = (object sender, XAPI.ConnectionStatus status, ref RspUserLoginField userLogin, int size1) => {
                LogUtil.EnginLog("行情状态:" + status.ToString());
            };

            mdApi.OnRtnDepthMarketData = _onRtnDepthMarketData;

            mdApi.Connect();
        }

        public override void Logout() {
            mdApi.Dispose();
        }

        public override void SubscribeMarketData(Instrument inst) {
            mFilterMap.Add(inst.InstrumentID,new DefaultTickFilter());
        }

        public override void UnSubscribeMarketData(Instrument Instrument) {
            mdApi.Unsubscribe(Instrument.InstrumentID,"");
        }

        private Dictionary<string,ITickFilter> mFilterMap = new Dictionary<string, ITickFilter>(); 
        private void _onRtnDepthMarketData(object sender, ref DepthMarketDataNClass marketData) {
            Instrument inst = InstrumentManager.GetInstrument(marketData.InstrumentID);
            if(inst == null)
                return;
            Tick tick = new Tick(inst
                ,marketData.LastPrice
                ,marketData.Bids[0].Price
                ,marketData.Bids[0].Size
                ,marketData.Asks[0].Price
                ,marketData.Asks[0].Size
                ,marketData.AveragePrice
                ,Convert.ToInt64(marketData.Volume)
                ,marketData.OpenInterest
                , new DateTime(
                    marketData.ActionDay / 10000
                    , marketData.ActionDay / 100 % 100
                    , marketData.ActionDay % 100
                    , marketData.UpdateTime / 10000
                    , marketData.UpdateTime / 100 % 100
                    , marketData.UpdateTime % 100
                    , marketData.UpdateMillisec)
                , marketData.UpperLimitPrice
                ,marketData.LowerLimitPrice);

            ITickFilter filter;
            if (mFilterMap.TryGetValue(tick.Instrument.InstrumentID, out filter))
            {
                if (filter.Check(tick))
                {
                    PostTickEvent(tick);
                }
            }
        }
    }
}
