using System;
using System.Collections.Concurrent;
using FreeQuant.Framework;
using System.IO;
using XAPI.Callback;
using XAPI;
using OrderStatus = XAPI.OrderStatus;

namespace Broker.Xapi2 {
    public class XapiTdBroker : BaseTdBroker {
        string mdPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CTP_SE_Trade_x64.dll");
        XApi mTdApi;
        //
        ConcurrentDictionary<string, OrderField> mBrokerOrderMap = new ConcurrentDictionary<string, OrderField>();
        ConcurrentDictionary<string, Order> mOrderMap = new ConcurrentDictionary<string, Order>();
        //
        public XapiTdBroker() {
            //TimerUtil.On1Min += _onCheck;
        }
        //
        public override void Login() {
            if (mTdApi == null) {
                mTdApi = new XApi(mdPath);
                mTdApi.Server.AppID = ConfigUtil.AppId;
                mTdApi.Server.AuthCode = ConfigUtil.AuthCode;
                mTdApi.Server.Address = ConfigUtil.TdServer;
                mTdApi.Server.BrokerID = ConfigUtil.TdBroker;
                mTdApi.User.UserID = ConfigUtil.TdInvestor;
                mTdApi.User.Password = ConfigUtil.TdPassword;
                //
                mTdApi.Server.PrivateTopicResumeType = ResumeType.Quick;
                mTdApi.OnRspQryInstrument = _onRspQryInstrument;
                mTdApi.OnRspQryInvestorPosition = _OnRspQryInvestorPosition;
                mTdApi.OnRtnOrder = _onRtnOrder;
                mTdApi.OnRtnTrade = _onRtnTrade;
                mTdApi.OnRspQrySettlementInfo =
                    (object sender, ref SettlementInfoClass settlementInfo, int size1, bool bIsLast) => {
                        LogUtil.SysLog("结算确认:" + settlementInfo.Content);
                    };
                mTdApi.OnConnectionStatus = _onConnectionStatus;
            } else if (mTdApi.IsConnected) {
                return;
            }
            mTdApi.ReconnectInterval = 60;
            mTdApi.Connect();
        }

        public override void Logout() {
        }

        public override void QueryInstrument() {
            ReqQueryField query = new ReqQueryField();
            mTdApi.ReqQuery(QueryType.ReqQryInstrument, query);
        }

        public override void QueryPosition() {
            ReqQueryField query = new ReqQueryField();
            mTdApi.ReqQuery(QueryType.ReqQryInvestorPosition, query);
        }

        public override void SendOrder(Order order) {
            OrderField field = new OrderField();
            field.Type = OrderType.Limit;
            field.HedgeFlag = HedgeFlagType.Speculation;
            field.InstrumentID = order.Instrument.InstrumentID;
            field.Side = order.Direction == DirectionType.Buy ? OrderSide.Buy : OrderSide.Sell;
            switch (order.OpenClose) {
                case FreeQuant.Framework.OpenCloseType.Close:
                    field.OpenClose = XAPI.OpenCloseType.Close;
                    break;
                case FreeQuant.Framework.OpenCloseType.CloseToday:
                    field.OpenClose = XAPI.OpenCloseType.CloseToday;
                    break;
                default:
                    field.OpenClose = XAPI.OpenCloseType.Open;
                    break;
            }
            field.Price = order.Price;
            field.Qty = order.Qty;

            string localId = mTdApi.SendOrder(field);

            //这里只记录策略订单，接口订单要等前置返回信息再记录
            order.OrderId = localId;
            mOrderMap.TryAdd(localId, order);
        }

        public override void CancelOrder(Order order) {
            OrderField field;
            if (mBrokerOrderMap.TryGetValue(order.OrderId, out field)) {
                mTdApi.CancelOrder(field.ID);
            }
        }

        private void _onRspQryInstrument(object sender, ref InstrumentField field, int size1, bool bIsLast) {
            //只订阅期货，并且不订阅套利等其他合约
            if (field.Type == InstrumentType.Future && RegexUtils.MatchInstrument(field.InstrumentID)) {
                Instrument inst = new Instrument(field.InstrumentID
                        , field.ProductID
                        , ConvertUtil.ConvertExchange(field.ExchangeID)
                        , field.VolumeMultiple
                        , field.PriceTick
                        , 1000);
                mOnInstrument?.Invoke(inst);
            }
        }

        private void _OnRspQryInvestorPosition(object sender, ref PositionField field, int size1, bool bIsLast) {
            Instrument inst = InstrumentManager.GetInstrument(field.InstrumentID);
            if (inst == null)
                return;
            //
            BrokerPosition brokerPos = new BrokerPosition(inst
                , ConvertUtil.ConvertExchange(field.ExchangeID)
                , field.Side == PositionSide.Long ? DirectionType.Buy : DirectionType.Sell
                , (long)field.Position
                , (long)field.TodayPosition
                , (long)field.HistoryPosition
                , (long)field.HistoryFrozen
                , (long)field.TodayBSPosition
                , (long)field.TodayBSFrozen);
            mOnBroderPosition?.Invoke(brokerPos);
        }

        private void _onRtnOrder(object sender, ref OrderField field) {
            Order order = convertOrder(field);
            mOnOrder?.Invoke(order);
        }

        private void _onRtnTrade(object sender, ref TradeField field) {
            Instrument inst = InstrumentManager.GetInstrument(field.InstrumentID);
            if (inst == null)
                return;
            //
            BrokerTrade trade = new BrokerTrade(inst
                , ConvertUtil.ConvertExchange(field.ExchangeID)
                , ConvertUtil.ConvertDirectionType(field.Side)
                , (long)field.Qty
                , field.Price
                , ConvertUtil.ConvertOpenCloseType(field.OpenClose)
                , DateTime.Now);

            mOnTrade?.Invoke(trade);
        }

        private void _onCheck() {
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

        //转换订单
        private Order convertOrder(OrderField field) {
            Order order;
            if (!mOrderMap.TryGetValue(field.LocalID, out order)) {
                return null;
            }
            //删除已成交订单
            switch (field.Status) {
                case OrderStatus.Rejected:
                case OrderStatus.Expired:
                case OrderStatus.Filled:
                    deleteOrder(field);
                    break;
            }
            //
            FreeQuant.Framework.OrderStatus status = ConvertUtil.ConvertOrderStatus(field.Status);
            order.Status = status;
            order.QtyLeft = (int)field.LeavesQty;
            order.QtyTraded = (int)field.CumQty;
            //
            return order;
        }

        private void deleteOrder(OrderField field) {
            Order o;
            OrderField f;
            mOrderMap.TryRemove(field.LocalID, out o);
            mBrokerOrderMap.TryRemove(field.LocalID, out f);
        }

        //状态变化
        private void _onConnectionStatus(object sender, XAPI.ConnectionStatus brokerStatus, ref RspUserLoginField userLogin, int size1) {
            FreeQuant.Framework.ConnectionStatus status = ConvertUtil.ConvertConnectionStatus(brokerStatus);
            mOnStatusChanged?.Invoke(status);
            //
            LogUtil.SysLog("交易状态:" + brokerStatus.ToString());
        }
    }
}
