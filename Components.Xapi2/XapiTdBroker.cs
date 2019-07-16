using System;
using System.Collections.Concurrent;
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
        //
        ConcurrentDictionary<string, OrderField> mBrokerOrderMap = new ConcurrentDictionary<string, OrderField>();
        ConcurrentDictionary<string, Order> mOrderMap = new ConcurrentDictionary<string, Order>();
        //
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

        protected override void SendOrder(Order order) {
            OrderField field = new OrderField();
            field.Type = OrderType.Limit;
            field.HedgeFlag = HedgeFlagType.Speculation;
            field.InstrumentID = order.Instrument.InstrumentID;
            field.Side = order.Direction == DirectionType.Buy ? OrderSide.Buy : OrderSide.Sell;
            switch (order.Offset) {
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
            field.Price = order.Price;
            field.Qty = order.Volume;

            //自动开平
            if (order.Offset == OffsetType.Auto)
            {
                BrokerPositionManger.Instance.AutoClose(field);
            }

            string localId = mTdApi.SendOrder(field);

            //这里只记录策略订单，接口订单要等前置返回信息再记录
            order.LocalId = localId;
            mOrderMap.TryAdd(localId, order);
        }

        protected override void CancelOrder(Order order) {
            OrderField field;
            if (mBrokerOrderMap.TryGetValue(order.LocalId, out field)) {
                mTdApi.CancelOrder(field.ID);
            }
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
            BrokerPositionManger.Instance.UpdatePosition(position);
        }

        private void _onRtnOrder(object sender, ref OrderField field) {
            BrokerPositionManger.Instance.AddOrder(field);
            //转换订单
            Order order = convertOrder(field);
            PostOrderEvent(order);
        }

        private void _onRtnTrade(object sender, ref TradeField trade) {
            BrokerPositionManger.Instance.UpdatePosition(trade);
        }

        //转换订单
        private Order convertOrder(OrderField field) {
            Order order;
            if (!mOrderMap.TryGetValue(field.LocalID, out order)) {
                return null;
            }
            FreeQuant.Components.OrderStatus status = FreeQuant.Components.OrderStatus.Normal;
            switch (field.Status) {
                case OrderStatus.NotSent:
                case OrderStatus.PendingNew:
                case OrderStatus.New:
                    status = FreeQuant.Components.OrderStatus.Normal;
                    break;
                case OrderStatus.Rejected:
                case OrderStatus.Expired:
                    status = FreeQuant.Components.OrderStatus.Error;
                    deleteOrder(field);
                    break;
                case OrderStatus.PartiallyFilled:
                    status = FreeQuant.Components.OrderStatus.Partial;
                    break;
                case OrderStatus.Filled:
                    status = FreeQuant.Components.OrderStatus.Filled;
                    deleteOrder(field);
                    break;
                case OrderStatus.Cancelled:
                    status = FreeQuant.Components.OrderStatus.Canceled;
                    deleteOrder(field);
                    break;
                case OrderStatus.PendingCancel:
                case OrderStatus.PendingReplace:
                case OrderStatus.Replaced:
                    break;
            }

            order.Status = status;
            order.VolumeLeft = (int)field.LeavesQty;
            order.VolumeTraded = (int)field.CumQty;

            return order;
        }

        private void deleteOrder(OrderField field) {
            Order o;
            OrderField f;
            mOrderMap.TryRemove(field.LocalID, out o);
            mBrokerOrderMap.TryRemove(field.LocalID, out f);
        }
    }
}
