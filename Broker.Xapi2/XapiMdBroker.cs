using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using FreeQuant.Framework;
using XAPI.Callback;
using XAPI;

namespace Broker.Xapi2 {
    public class XapiMdBroker : BaseMdBroker {
        string mdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CTP_SE_Quote_x64.dll");
        XApi mMdApi;
        //
        public XapiMdBroker() {
            //TimerUtil.On1Min += _onTimer;
        }
        //
        public override void Login() {
            if (mMdApi == null) {
                mMdApi = new XApi(mdPath);
                mMdApi.Server.AppID = ConfigUtil.AppId;
                mMdApi.Server.AuthCode = ConfigUtil.AuthCode;
                mMdApi.Server.Address = ConfigUtil.MdServer;
                mMdApi.Server.BrokerID = ConfigUtil.MdBroker;
                mMdApi.User.UserID = ConfigUtil.MdInvestor;
                mMdApi.User.Password = ConfigUtil.MdPassword;
                //
                mMdApi.OnConnectionStatus = _onConnectionStatus;
                mMdApi.OnRtnError = _onRtnError;
                mMdApi.OnRtnDepthMarketData = _onRtnDepthMarketData;
            } else if (mMdApi.IsConnected) {
                return;
            }
            mMdApi.ReconnectInterval = 60;
            mMdApi.Connect();
        }

        public override void Logout() {
        }

        private HashSet<Instrument> mInstruments = new HashSet<Instrument>();
        public override void SubscribeMarketData(Instrument inst) {
            if (mInstruments.Add(inst)) {
                mMdApi.Subscribe(inst.InstrumentID, "");
                LogUtil.SysLog("订阅合约：" + inst.InstrumentID);
            }
        }

        public override void UnSubscribeMarketData(Instrument inst) {
            mMdApi.Unsubscribe(inst.InstrumentID, "");
            mInstruments.Remove(inst);
            LogUtil.SysLog("退订合约：" + inst.InstrumentID);
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
                if (marketData.Volume < vol) {
                    volumeMap[marketData.InstrumentID] = marketData.Volume;
                    return;
                }
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
            mOnTick?.Invoke(tick);
        }

        private void _onConnectionStatus(object sender, XAPI.ConnectionStatus brokerStatus, ref RspUserLoginField userLogin, int size1) {
            FreeQuant.Framework.ConnectionStatus status = ConvertUtil.ConvertConnectionStatus(brokerStatus);
            mOnStatusChanged?.Invoke(status);
            //
            //if (brokerStatus == XAPI.ConnectionStatus.Done) {
            //    Resub();
            //}
            //
            LogUtil.SysLog("行情状态:" + brokerStatus.ToString());
        }

        private void _onRtnError(object sender, ref ErrorField error) {
            LogUtil.SysLog($"行情错误({error.RawErrorID}):{error.Text}");
        }

        private void _onTimer() {
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

        //private void Resub() {
        //    foreach (Instrument inst in mInstruments) {
        //        mMdApi.Subscribe(inst.InstrumentID, "");
        //        LogUtil.SysLog("订阅合约：" + inst.InstrumentID);
        //    }
        //}
    }
}
