using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using FreeQuant.Framework;
using FreeQuant.EventEngin;
using XAPI.Callback;
using XAPI;

namespace Broker.Xapi2 {
    public class XapiMdBroker : BaseMdBroker, IComponent {
        string mdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CTP_SE_Quote_x64.dll");
        XApi mMdApi;
        //
        public void OnLoad() {
            EventBus.Register(this);
            LogUtil.EnginLog("行情组件启动");
        }
        public void OnReady() { }
        //
        public override void Login() {
            if (mMdApi == null) {
                mMdApi = new XApi(mdPath);
                mMdApi.Server.AppID = ConfigUtil.Config.AppId;
                mMdApi.Server.AuthCode = ConfigUtil.Config.AuthCode;
                mMdApi.Server.Address = ConfigUtil.Config.MdServer;
                mMdApi.Server.BrokerID = ConfigUtil.Config.MdBroker;
                mMdApi.User.UserID = ConfigUtil.Config.MdInvestor;
                mMdApi.User.Password = ConfigUtil.Config.MdPassword;
                //
                mMdApi.OnConnectionStatus = _onConnectionStatus;
                mMdApi.OnRtnError = _onRtnError;
                mMdApi.OnRtnDepthMarketData = _onRtnDepthMarketData;
            } else if (mMdApi.IsConnected) {
                return;
            }
            mMdApi.Connect();
        }

        public override void Logout() {
        }

        private HashSet<string> mInstIds = new HashSet<string>();
        public override void SubscribeMarketData(Instrument inst) {
            if (mInstIds.Add(inst.InstrumentID)) {
                mMdApi.Subscribe(inst.InstrumentID, "");
            }
        }

        public override void UnSubscribeMarketData(Instrument inst) {
            mMdApi.Unsubscribe(inst.InstrumentID, "");
            mInstIds.Remove(inst.InstrumentID);
            LogUtil.EnginLog("退订合约：" + inst.InstrumentID);
        }

        private Dictionary<string, double> volumeMap = new Dictionary<string, double>();
        private void _onRtnDepthMarketData(object sender, ref DepthMarketDataNClass marketData) {
            Instrument inst = InstrumentManager.GetInstrument(marketData.InstrumentID);
            if (inst == null)
                return;
            //计算量的差值
            double vol = 0;
            double difVol = 0;
            if (volumeMap.TryGetValue(marketData.InstrumentID, out vol)) {
                if (marketData.Volume < vol)
                    return;
                difVol = marketData.Volume - vol;
                vol = marketData.Volume;
            } else {
                volumeMap.Add(marketData.InstrumentID, marketData.Volume);
                return;
            }
            //
            Tick tick = new Tick(inst
                , marketData.LastPrice
                , marketData.Bids.Length > 0 ? marketData.Bids[0].Price : 0
                , marketData.Bids.Length > 0 ? marketData.Bids[0].Size : 0
                , marketData.Asks.Length > 0 ? marketData.Asks[0].Price : 0
                , marketData.Asks.Length > 0 ? marketData.Asks[0].Size : 0
                , difVol
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
                    Resub();
                    break;
            }
            LogUtil.EnginLog("行情状态:" + status.ToString());
        }

        private void _onRtnError(object sender, ref ErrorField error) {
            LogUtil.EnginLog($"行情错误({error.RawErrorID}):{error.Text}");
        }

        [OnEvent]
        private void _onCheck(BrokerEvent.MonitorEvent evt) {
            long now = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
            if (now > 231 && now < 845) {
                return;
            }

            if (now > 1516 && now < 2045) {
                return;
            }
            //
            Login();
        }

        private void Resub() {
            foreach (string instId in mInstIds) {
                mMdApi.Subscribe(instId, "");
            }
        }
    }
}
