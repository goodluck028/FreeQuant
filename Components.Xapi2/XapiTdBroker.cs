using System;
using System.Text.RegularExpressions;
using FreeQuant.Components;
using System.IO;
using XAPI.Callback;
using XAPI;
using OrderStatus = XAPI.OrderStatus;

namespace Components.Xapi2 {
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
            mTdApi.OnRspQryInvestorPosition = _OnRspQryInvestorPosition;
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

        protected override void QueryPosition() {
            ReqQueryField query = new ReqQueryField();
            mTdApi.ReqQuery(QueryType.ReqQryInvestorPosition, query);
        }

        protected override void SendOrder(BrokerOrder brokerOrder) {
            OrderField field = new OrderField();
            field.Type = OrderType.Limit;
            field.HedgeFlag = HedgeFlagType.Speculation;
            field.InstrumentID = brokerOrder.InstrumentId;
            field.Side = brokerOrder.Direction == DirectionType.Buy ? OrderSide.Buy : OrderSide.Sell;
            switch (brokerOrder.Offset) {
                case OffsetType.Close:
                    field.OpenClose = OpenCloseType.Close;
                    break;
                case OffsetType.CloseToday:
                    field.OpenClose = OpenCloseType.CloseToday;
                    break;
                default:
                    field.OpenClose = OpenCloseType.Open;
                    break;
            }
            field.Price = brokerOrder.LimitPrice;
            field.Qty = brokerOrder.Volume;
            string localId = mTdApi.SendOrder(field);
            brokerOrder.LocalId = localId;
        }

        protected override void CancelOrder(BrokerOrder brokerOrder) {
            mTdApi.CancelOrder(brokerOrder.LocalId);
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
                PostInstrumentEvent(inst);
            }
        }

        private void _OnRspQryInvestorPosition(object sender, ref PositionField position, int size1, bool bIsLast) {
            Instrument inst = InstrumentManager.GetInstrument(position.InstrumentID);
            if (inst == null)
                return;
            //
            DirectionType type = DirectionType.Buy;
            if (position.Side == PositionSide.Short) {
                type = DirectionType.Sell;
            }
            BrokerPositionEvent evt = new BrokerPositionEvent(position.InstrumentID
                , type
                , Convert.ToInt64(position.HistoryPosition)
                , Convert.ToInt64(position.HistoryFrozen)
                , Convert.ToInt64(position.TodayPosition)
                , Convert.ToInt64(position.TodayBSFrozen));
            PostPositionEvent(evt);
        }

        private void _onRtnOrder(object sender, ref OrderField order) {
            FreeQuant.Components.OrderStatus status = FreeQuant.Components.OrderStatus.Normal;
            switch (order.Status) {
                case OrderStatus.NotSent:
                case OrderStatus.PendingNew:
                case OrderStatus.New:
                    status = FreeQuant.Components.OrderStatus.Normal;
                    break;
                case OrderStatus.Rejected:
                case OrderStatus.Expired:
                    status = FreeQuant.Components.OrderStatus.Error;
                    break;
                case OrderStatus.PartiallyFilled:
                    status = FreeQuant.Components.OrderStatus.Partial;
                    break;
                case OrderStatus.Filled:
                    status = FreeQuant.Components.OrderStatus.Filled;
                    break;
                case OrderStatus.Cancelled:
                    status = FreeQuant.Components.OrderStatus.Canceled;
                    break;
                case OrderStatus.PendingCancel:
                case OrderStatus.PendingReplace:
                case OrderStatus.Replaced:
                    return;
            }

            BrokerOrderEvent evt = new BrokerOrderEvent(order.LocalID
                , order.OrderID
                , Convert.ToInt64(order.CumQty)
                , Convert.ToInt64(order.LeavesQty)
                , status);
            PostOrderEvent(evt);
        }

        private void _onRtnTrade(object sender, ref TradeField trade) {
            BrokerTradeEvent evt = new BrokerTradeEvent(trade.ID, Convert.ToInt64(trade.Qty));
            PostTradeEvent(evt);
        }
    }
}
