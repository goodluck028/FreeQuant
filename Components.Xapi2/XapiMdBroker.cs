using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using FreeQuant.Components;
using FreeQuant.Framework;
using XAPI.Callback;
using XAPI;

namespace Components.Xapi2 {
    [Component]
    public class XapiMdBroker : BaseMdBroker {
        string mdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CTP_SE_Quote_x64.dll");
        private Account mAccount = ConfigUtil.Config.MyMdAccount;
        XApi mMdApi;

        public override void Login() {
            mMdApi = new XApi(mdPath);
            mMdApi.Server.UserProductInfo = "simnow_client_test";
            mMdApi.Server.AuthCode = "0000000000000000";
            mMdApi.Server.Address = mAccount.Server;
            mMdApi.Server.BrokerID = mAccount.Broker;
            mMdApi.User.UserID = mAccount.Investor;
            mMdApi.User.Password = mAccount.Password;
            mMdApi.OnConnectionStatus = _onConnectionStatus;

            mMdApi.OnRtnError = _onRtnError;

            mMdApi.OnRtnDepthMarketData = _onRtnDepthMarketData;

            mMdApi.Connect();
        }

        public override void Logout() {
            mMdApi.Dispose();
        }

        public override void SubscribeMarketData(Instrument inst) {
            mFilterMap.Add(inst.InstrumentID, new DefaultTickFilter());
        }

        public override void UnSubscribeMarketData(Instrument Instrument) {
            mMdApi.Unsubscribe(Instrument.InstrumentID, "");
        }

        private Dictionary<string, ITickFilter> mFilterMap = new Dictionary<string, ITickFilter>();
        private void _onRtnDepthMarketData(object sender, ref DepthMarketDataNClass marketData) {
            Instrument inst = InstrumentManager.GetInstrument(marketData.InstrumentID);
            if (inst == null)
                return;
            Tick tick = new Tick(inst
                , marketData.LastPrice
                , marketData.Bids[0].Price
                , marketData.Bids[0].Size
                , marketData.Asks[0].Price
                , marketData.Asks[0].Size
                , marketData.AveragePrice
                , Convert.ToInt64(marketData.Volume)
                , marketData.OpenInterest
                , new DateTime(
                    marketData.ActionDay / 10000
                    , marketData.ActionDay / 100 % 100
                    , marketData.ActionDay % 100
                    , marketData.UpdateTime / 10000
                    , marketData.UpdateTime / 100 % 100
                    , marketData.UpdateTime % 100
                    , marketData.UpdateMillisec)
                , marketData.UpperLimitPrice
                , marketData.LowerLimitPrice);

            ITickFilter filter;
            if (mFilterMap.TryGetValue(tick.Instrument.InstrumentID, out filter)) {
                if (filter.Check(tick)) {
                    PostTickEvent(tick);
                }
            }
        }

        private void _onConnectionStatus(object sender, ConnectionStatus status, ref RspUserLoginField userLogin, int size1) {
            switch (status) {
                case ConnectionStatus.Logined:
                    PostLoginEvent(true, "登录成功");
                    break;
            }
            LogUtil.EnginLog("行情状态:" + status.ToString());
        }

        private void _onRtnError(object sender, ref ErrorField error) {
            LogUtil.EnginLog($"行情错误({error.RawErrorID}):{error.Text}");
        }
    }
}
