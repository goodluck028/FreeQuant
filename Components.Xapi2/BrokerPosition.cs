using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XAPI;

namespace Components.Xapi2 {
    //持仓管理器
    internal class BrokerPositionManger
    {
        private BrokerPositionManger() { }
        private static BrokerPositionManger mInstance = new BrokerPositionManger();

        public static BrokerPositionManger Instance
        {
            get { return mInstance; }
            set { mInstance = value; }
        }

        private Dictionary<string, BrokerPosition> mPositionMap = new Dictionary<string, BrokerPosition>();

        private BrokerPosition getBrokerPosition(string instId) {
            BrokerPosition bp;
            mPositionMap.TryGetValue(instId, out bp);
            if (bp == null) {
                bp = new BrokerPosition();
                mPositionMap.Add(instId, bp);
            }
            return bp;
        }
        public void UpdatePosition(PositionField position) {
            getBrokerPosition(position.InstrumentID).UpdatePosition(position);
        }

        public void UpdatePosition(TradeField trade) {
            getBrokerPosition(trade.InstrumentID).UpdatePosition(trade);
        }

        public void AutoClose(OrderField order) {
            getBrokerPosition(order.InstrumentID).AutoClose(order);
        }

        public void AddOrder(OrderField order) {
            getBrokerPosition(order.InstrumentID).AddOrder(order);
        }
    }

    //单个合约持仓
    internal class BrokerPosition {
        //
        private PositionPart mTdLongPart = new PositionPart();
        private PositionPart mTdShortPart = new PositionPart();
        private PositionPart mYdLongPart = new PositionPart();
        private PositionPart mYdShortPart = new PositionPart();

        //更新持仓
        public void UpdatePosition(PositionField position) {
            if (position.Side == PositionSide.Long) {
                mTdLongPart.Position = (long)position.TodayPosition;
                mTdLongPart.Frozen = (long)position.TodayBSFrozen;
                mYdLongPart.Position = (long)(position.Position - position.TodayPosition);
                mYdLongPart.Frozen = (long)position.HistoryFrozen;
            } else {
                mTdShortPart.Position = (long)position.TodayPosition;
                mTdShortPart.Frozen = (long)position.TodayBSFrozen;
                mYdShortPart.Position = (long)(position.Position - position.TodayPosition);
                mYdShortPart.Frozen = (long)position.HistoryFrozen;
            }
        }

        public void UpdatePosition(TradeField trade) {
            if (trade.Side == OrderSide.Buy) { //买
                if (trade.OpenClose == OpenCloseType.CloseToday) { //买平今空
                    mTdShortPart.Frozen -= (long)trade.Qty;
                    mTdShortPart.Position -= (long)trade.Qty;
                } else if (trade.OpenClose == OpenCloseType.Close) { //买平昨空
                    mYdShortPart.Frozen -= (long)trade.Qty;
                    mYdShortPart.Position -= (long)trade.Qty;
                } else { //买开多
                    mTdLongPart.Position += (long)trade.Qty;
                }
            } else if (trade.Side == OrderSide.Sell) { //卖
                if (trade.OpenClose == OpenCloseType.CloseToday) { //卖平今多
                    mTdLongPart.Frozen -= (long)trade.Qty;
                    mTdLongPart.Position -= (long)trade.Qty;
                } else if (trade.OpenClose == OpenCloseType.Close) { //，卖平昨多
                    mYdLongPart.Frozen -= (long)trade.Qty;
                    mYdLongPart.Position -= (long)trade.Qty;
                } else { //卖开空
                    mTdShortPart.Position += (long)trade.Qty;
                }
            }
        }

        //自动开平
        public OrderField AutoClose(OrderField order) {
            if (order.Side == OrderSide.Buy) {
                if (mTdShortPart.Free > order.Qty) {
                    order.OpenClose = OpenCloseType.CloseToday;
                } else if (mYdShortPart.Free > order.Qty) {
                    order.OpenClose = OpenCloseType.Close;
                }
            } else if (order.Side == OrderSide.Sell) {
                if (mTdLongPart.Free > order.Qty) {
                    order.OpenClose = OpenCloseType.CloseToday;
                } else if (mYdLongPart.Free > order.Qty) {
                    order.OpenClose = OpenCloseType.Close;
                }
            }
            return order;
        }

        //冻结管理
        List<OrderField> mFrozenOrders = new List<OrderField>();
        public void AddOrder(OrderField order) {
            //只关注平仓单
            if (order.OpenClose == OpenCloseType.Open || order.OpenClose == OpenCloseType.Undefined)
                return;
            //没有就添加
            if (!mFrozenOrders.Contains(order)) {
                mFrozenOrders.Add(order);
                if (order.Side == OrderSide.Buy) {
                    if (order.OpenClose == OpenCloseType.CloseToday) {
                        mTdShortPart.Frozen += (short)order.Qty;
                    } else if (order.OpenClose == OpenCloseType.Close) {
                        mYdShortPart.Frozen += (short)order.Qty;
                    }
                } else if (order.Side == OrderSide.Sell) {
                    if (order.OpenClose == OpenCloseType.CloseToday) {
                        mTdLongPart.Frozen += (short)order.Qty;
                    } else if (order.OpenClose == OpenCloseType.Close) {
                        mYdLongPart.Frozen += (short)order.Qty;
                    }
                }
            } else if (order.Status == OrderStatus.Cancelled
                 || order.Status == OrderStatus.Expired
                 || order.Status == OrderStatus.Rejected) {
                //解除冻结
                mFrozenOrders.Remove(order);
                if (order.Side == OrderSide.Buy) {
                    if (order.OpenClose == OpenCloseType.CloseToday) {
                        mTdShortPart.Frozen -= (short)order.Qty;
                    } else if (order.OpenClose == OpenCloseType.Close) {
                        mYdShortPart.Frozen -= (short)order.Qty;
                    }
                } else if (order.Side == OrderSide.Sell) {
                    if (order.OpenClose == OpenCloseType.CloseToday) {
                        mTdLongPart.Frozen -= (short)order.Qty;
                    } else if (order.OpenClose == OpenCloseType.Close) {
                        mYdLongPart.Frozen -= (short)order.Qty;
                    }
                }
            }
        }
    }

    internal class PositionPart {
        private long mPosition;
        private long mFrozen;

        public long Position {
            get { return mPosition; }
            set { mPosition = value; }
        }

        public long Frozen {
            get { return mFrozen; }
            set { mFrozen = value; }
        }

        public long Free => mPosition - mFrozen;

    }
}
