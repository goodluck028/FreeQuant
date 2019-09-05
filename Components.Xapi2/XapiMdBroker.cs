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
        XApi mMdApi;

        public override void Login() {
            mMdApi = new XApi(mdPath);
            mMdApi.Server.AppID = ConfigUtil.Config.AppId;
            mMdApi.Server.AuthCode = ConfigUtil.Config.AuthCode;
            mMdApi.Server.Address = ConfigUtil.Config.MdServer;
            mMdApi.Server.BrokerID = ConfigUtil.Config.MdBroker;
            mMdApi.User.UserID = ConfigUtil.Config.MdInvestor;
            mMdApi.User.Password = ConfigUtil.Config.MdPassword;
            mMdApi.OnConnectionStatus = _onConnectionStatus;

            mMdApi.OnRtnError = _onRtnError;

            mMdApi.OnRtnDepthMarketData = _onRtnDepthMarketData;

            mMdApi.Connect();
        }

        public override void Logout() {
            mMdApi.Dispose();
        }

        public override void SubscribeMarketData(Instrument inst) {
            mMdApi.Subscribe(inst.InstrumentID, "");
            LogUtil.EnginLog("订阅合约：" + inst.InstrumentID);
        }

        public override void UnSubscribeMarketData(Instrument inst) {
            mMdApi.Unsubscribe(inst.InstrumentID, "");
            LogUtil.EnginLog("退订合约：" + inst.InstrumentID);
        }


        private void _onRtnDepthMarketData(object sender, ref DepthMarketDataNClass marketData) {
            Instrument inst = InstrumentManager.GetInstrument(marketData.InstrumentID);
            if (inst == null)
                return;
            Tick tick = new Tick(inst
                , marketData.LastPrice
                , marketData.Bids.Length > 0 ? marketData.Bids[0].Price : 0
                , marketData.Bids.Length > 0 ? marketData.Bids[0].Size : 0
                , marketData.Asks.Length > 0 ? marketData.Asks[0].Price : 0
                , marketData.Asks.Length > 0 ? marketData.Asks[0].Size : 0
                , marketData.Volume
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
            //
            PostTickEvent(tick);
        }

        private void _onConnectionStatus(object sender, ConnectionStatus status, ref RspUserLoginField userLogin, int size1) {
            switch (status) {
                case ConnectionStatus.Done:
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
