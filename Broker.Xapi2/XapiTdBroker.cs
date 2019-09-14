using System;
using System.Collections.Concurrent;
using FreeQuant.Framework;
using System.IO;
using FreeQuant.EventEngin;
using XAPI.Callback;
using XAPI;
using OrderStatus = XAPI.OrderStatus;

namespace Broker.Xapi2 {
    [AutoCreate]
    public class XapiTdBroker : BaseTdBroker {
        string mdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CTP_SE_Trade_x64.dll");
        XApi mTdApi;
        //
        ConcurrentDictionary<string, OrderField> mBrokerOrderMap = new ConcurrentDictionary<string, OrderField>();
        ConcurrentDictionary<string, Order> mOrderMap = new ConcurrentDictionary<string, Order>();
        //
        protected override void Login() {
            if (mTdApi != null) {
                if (!mTdApi.IsConnected) {
                    mTdApi.Dispose();
                    mTdApi = null;
                }
                return;
            }
            //
            mTdApi = new XApi(mdPath);
            mTdApi.Server.AppID = ConfigUtil.Config.AppId;
            mTdApi.Server.AuthCode = ConfigUtil.Config.AuthCode;
            mTdApi.Server.Address = ConfigUtil.Config.TdServer;
            mTdApi.Server.BrokerID = ConfigUtil.Config.TdBroker;
            mTdApi.User.UserID = ConfigUtil.Config.TdInvestor;
            mTdApi.User.Password = ConfigUtil.Config.TdPassword;
            //
            mTdApi.Server.PrivateTopicResumeType = ResumeType.Quick;
            mTdApi.OnRspQryInstrument = _onRspQryInstrument;
            mTdApi.OnRspQryInvestorPosition = _OnRspQryInvestorPosition;
            mTdApi.OnRtnOrder = _onRtnOrder;
            mTdApi.OnRtnTrade = _onRtnTrade;
            mTdApi.OnRspQrySettlementInfo = (object sender, ref SettlementInfoClass settlementInfo, int size1, bool bIsLast) => {
                LogUtil.EnginLog("结算确认:" + settlementInfo.Content);
            };
            mTdApi.OnConnectionStatus = _onConnectionStatus;

            mTdApi.Connect();
        }

        protected override void Logout() {
            if (mTdApi == null)
                return;
            if (mTdApi.IsConnected) {
                mTdApi.Disconnect();
            } else {
                mTdApi.Dispose();
                mTdApi = null;
            }
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
            if (order.Offset == OffsetType.Auto) {
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
            if (instrument.Type == InstrumentType.Future
                && RegexUtils.MatchInstrument(instrument.InstrumentID)) {
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
            EventBus.PostEvent(position);
        }

        //这样搞，是为了避免接口线程和事件线程同时操作对象需要加锁的麻烦，但是会增加延迟。
        [OnEvent]
        private void onBroderPosition(PositionField field) {
            BrokerPositionManger.Instance.UpdatePosition(field);
        }

        private void _onRtnOrder(object sender, ref OrderField field) {
            EventBus.PostEvent(field);
        }

        [OnEvent]
        private void onBrokerOrder(OrderField field) {
            BrokerPositionManger.Instance.AddOrder(field);
            //转换订单
            Order order = convertOrder(field);
            PostOrderEvent(order);
        }

        private void _onRtnTrade(object sender, ref TradeField trade) {
            EventBus.PostEvent(trade);
        }

        [OnEvent]
        private void onBroderTrade(TradeField field) {
            BrokerPositionManger.Instance.UpdatePosition(field);
        }

        [OnEvent]
        private void _onCheck(BrokerEvent.MonitorEvent evt) {
            if (mTdApi == null)
                return;
            long now = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
            if (now > 231 && now < 845) {
                Logout();
                return;
            }

            if (now > 1516 && now < 2045) {
                Logout();
                return;
            }
            //
            if (!mTdApi.IsConnected) {
                Login();
            }
        }

        //转换订单
        private Order convertOrder(OrderField field) {
            Order order;
            if (!mOrderMap.TryGetValue(field.LocalID, out order)) {
                return null;
            }
            FreeQuant.Framework.OrderStatus status = FreeQuant.Framework.OrderStatus.Normal;
            switch (field.Status) {
                case OrderStatus.NotSent:
                case OrderStatus.PendingNew:
                case OrderStatus.New:
                    status = FreeQuant.Framework.OrderStatus.Normal;
                    break;
                case OrderStatus.Rejected:
                case OrderStatus.Expired:
                    status = FreeQuant.Framework.OrderStatus.Error;
                    deleteOrder(field);
                    break;
                case OrderStatus.PartiallyFilled:
                    status = FreeQuant.Framework.OrderStatus.Partial;
                    break;
                case OrderStatus.Filled:
                    status = FreeQuant.Framework.OrderStatus.Filled;
                    deleteOrder(field);
                    break;
                case OrderStatus.Cancelled:
                    status = FreeQuant.Framework.OrderStatus.Canceled;
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

        //状态变化
        private void _onConnectionStatus(object sender, ConnectionStatus status, ref RspUserLoginField userLogin, int size1) {
            switch (status) {
                case ConnectionStatus.Done:
                    BrokerEvent.TdLoginEvent evt = new BrokerEvent.TdLoginEvent(true, "");
                    EventBus.PostEvent(evt);
                    break;
            }
            //
            LogUtil.EnginLog("交易状态:" + status.ToString());
        }

    }
}
