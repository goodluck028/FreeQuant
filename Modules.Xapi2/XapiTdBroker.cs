﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using FreeQuant.Modules;
using System.IO;
using FreeQuant.Framework;
using FreeQuant.Modules.Broker;
using XAPI.Callback;
using XAPI;
using OrderStatus = XAPI.OrderStatus;

namespace Modules.Xapi2 {
    public class XapiTdBroker : BaseTdBroker {
        string mdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CTP_Trade_x86.dll");
        private Account mAccount = ConfigUtil.Config.MyMdAccount;
        XApi mTdApi;

        protected override void Login() {
            mTdApi = new XApi(mdPath);
            mTdApi.Server.Address = mAccount.Server;
            mTdApi.Server.BrokerID = mAccount.Broker;
            mTdApi.User.UserID = mAccount.Investor;
            mTdApi.User.Password = mAccount.Password;
            mTdApi.Server.PrivateTopicResumeType = ResumeType.Quick;
            mTdApi.OnRspQryInstrument = _onRspQryInstrument;
            mTdApi.OnRtnOrder = _onRtnOrder;
            mTdApi.OnRtnTrade = _onRtnTrade;
            mTdApi.OnRspQrySettlementInfo = (object sender, ref SettlementInfoClass settlementInfo, int size1, bool bIsLast) => {
                LogUtil.EnginLog("结算确认:" + settlementInfo.Content);
            };
            mTdApi.OnConnectionStatus = (object sender, ConnectionStatus status, ref RspUserLoginField userLogin, int size1) => {
                LogUtil.EnginLog("交易状态:" + status.ToString());
            };

            mTdApi.Connect();
        }

        protected override void Logout() {
            mTdApi.Dispose();
        }

        protected override void QueryInstrument() {
            ReqQueryField query = new ReqQueryField();
            mTdApi.ReqQuery(QueryType.ReqQryInstrument, query);
        }

        protected override void SendOrder(BrokerOrder brokerOrder) {
        }

        protected override void CancelOrder(BrokerOrder brokerOrder) {
        }

        private void _onRspQryInstrument(object sender, ref InstrumentField instrument, int size1, bool bIsLast) {
            //只订阅期货，并且不订阅套利等其他合约
            Regex re = new Regex(@"^[a-zA-Z]+\d+$", RegexOptions.None);
            if (instrument.Type == InstrumentType.Future
                && re.IsMatch(instrument.InstrumentID)) {
                Exchange exchange = Exchange.SHFE;
                switch (instrument.ExchangeID) {
                    case "SHFE":
                        exchange = Exchange.SHFE;
                        break;
                    case "DCE":
                        exchange = Exchange.DCE;
                        break;
                    case "CZCE":
                        exchange = Exchange.CZCE;
                        break;
                    case "CFFEX":
                        exchange = Exchange.CFFEX;
                        break;
                    default:
                        return;
                }

                Instrument inst = new Instrument(instrument.InstrumentID
                        , instrument.ProductID
                        , exchange
                        , instrument.VolumeMultiple
                        , instrument.PriceTick
                        , 1000);
                InstrumentReturnEvent evt = new InstrumentReturnEvent(inst);
                EventBus.PostEvent(evt);
            }
        }

        private void _onRtnOrder(object sender, ref OrderField order)
        {
            FreeQuant.Modules.OrderStatus status = FreeQuant.Modules.OrderStatus.Normal;
            switch (order.Status)
            {
                case OrderStatus.NotSent:
                case OrderStatus.PendingNew:
                case OrderStatus.New:
                    status = FreeQuant.Modules.OrderStatus.Normal;
                    break;
                case OrderStatus.Rejected:
                case OrderStatus.Expired:
                    status = FreeQuant.Modules.OrderStatus.Error;
                    break;
                case OrderStatus.PartiallyFilled:
                    status = FreeQuant.Modules.OrderStatus.Partial;
                    break;
                case OrderStatus.Filled:
                    status = FreeQuant.Modules.OrderStatus.Filled;
                    break;
                case OrderStatus.Cancelled:
                    status = FreeQuant.Modules.OrderStatus.Canceled;
                    break;
                case OrderStatus.PendingCancel:
                case OrderStatus.PendingReplace:
                case OrderStatus.Replaced:
                    break;
            }

            OrderReturnEvent evt = new OrderReturnEvent(order.LocalID
                , order.OrderID
                , Convert.ToInt64(order.CumQty)
                , Convert.ToInt64(order.LeavesQty)
                , status);

            EventBus.PostEvent(evt);
        }

        private void _onRtnTrade(object sender, ref TradeField trade) {
            TradeReturnEvent evt = new TradeReturnEvent(trade.ID, Convert.ToInt64(trade.Qty));
            EventBus.PostEvent(evt);
        }
    }
}
