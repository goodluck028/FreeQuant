using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public class StrategyPosition {
        private BaseStrategy mStrategy;
        private Instrument mInstrument;
        private long mVol = 0;
        private DateTime mLastTime;

        public StrategyPosition(BaseStrategy strategy, Instrument instrument, long vol, DateTime lastTime) {
            mStrategy = strategy;
            mInstrument = instrument;
            mVol = vol;
            mLastTime = lastTime;
        }

        public BaseStrategy Strategy {
            get { return mStrategy; }
            internal set { mStrategy = value; }
        }

        public Instrument Instrument {
            get { return mInstrument; }
            internal set { mInstrument = value; }
        }

        public long Vol {
            get { return mVol; }
            internal set { mVol = value; }
        }

        public DateTime LastTime {
            get { return mLastTime; }
            internal set { mLastTime = value; }
        }
    }

    //单个合约持仓
    public class Position {
        private Instrument mInstrument;
        private PositionPart mTdLongPart = new PositionPart();
        private PositionPart mTdShortPart = new PositionPart();
        private PositionPart mYdLongPart = new PositionPart();
        private PositionPart mYdShortPart = new PositionPart();

        public Position(Instrument inst) {
            mInstrument = inst;
        }

        //更新持仓
        public void UpdatePosition(BrokerPosition position) {
            if (position.Direction == DirectionType.Buy) {
                mTdLongPart.Position = position.TodayPosition;
                mTdLongPart.Frozen = position.TodayBSFrozen;
                mYdLongPart.Position = position.Position - position.TodayPosition;
                mYdLongPart.Frozen = position.HistoryFrozen;
            } else {
                mTdShortPart.Position = position.TodayPosition;
                mTdShortPart.Frozen = position.TodayBSFrozen;
                mYdShortPart.Position = (position.Position - position.TodayPosition);
                mYdShortPart.Frozen = position.HistoryFrozen;
            }
        }

        public void UpdatePosition(BrokerTrade trade) {
            if (trade.Direction == DirectionType.Buy) { //买
                if (trade.OpenClose == OpenCloseType.CloseToday) { //买平今空
                    mTdShortPart.Frozen -= trade.Qty;
                    mTdShortPart.Position -= trade.Qty;
                } else if (trade.OpenClose == OpenCloseType.Close) { //买平昨空
                    mYdShortPart.Frozen -= trade.Qty;
                    mYdShortPart.Position -= trade.Qty;
                } else { //买开多
                    mTdLongPart.Position += trade.Qty;
                }
            } else { //卖
                if (trade.OpenClose == OpenCloseType.CloseToday) { //卖平今多
                    mTdLongPart.Frozen -= trade.Qty;
                    mTdLongPart.Position -= trade.Qty;
                } else if (trade.OpenClose == OpenCloseType.Close) { //，卖平昨多
                    mYdLongPart.Frozen -= trade.Qty;
                    mYdLongPart.Position -= trade.Qty;
                } else { //卖开空
                    mTdShortPart.Position += trade.Qty;
                }
            }
        }

        //自动开平
        public Order AutoClose(Order order) {
            if (order.Direction == DirectionType.Buy) {
                if (mTdShortPart.Free > order.Qty) {
                    order.OpenClose = OpenCloseType.CloseToday;
                } else if (mYdShortPart.Free > order.Qty) {
                    order.OpenClose = OpenCloseType.Close;
                }
            } else {
                if (mTdLongPart.Free > order.Qty) {
                    order.OpenClose = OpenCloseType.CloseToday;
                } else if (mYdLongPart.Free > order.Qty) {
                    order.OpenClose = OpenCloseType.Close;
                }
            }
            return order;
        }

        //冻结管理
        List<Order> mFrozenOrders = new List<Order>();
        public void AddOrder(Order order) {
            //只关注平仓单
            if (order.OpenClose == OpenCloseType.Open)
                return;
            //没有就添加
            if (!mFrozenOrders.Contains(order)) {
                mFrozenOrders.Add(order);
                if (order.Direction == DirectionType.Buy) {
                    if (order.OpenClose == OpenCloseType.CloseToday) {
                        mTdShortPart.Frozen += order.Qty;
                    } else if (order.OpenClose == OpenCloseType.Close) {
                        mYdShortPart.Frozen += order.Qty;
                    }
                } else{
                    if (order.OpenClose == OpenCloseType.CloseToday) {
                        mTdLongPart.Frozen += order.Qty;
                    } else if (order.OpenClose == OpenCloseType.Close) {
                        mYdLongPart.Frozen += order.Qty;
                    }
                }
            } else if (order.Status == OrderStatus.Canceled
                 || order.Status == OrderStatus.Error) {
                //解除冻结
                mFrozenOrders.Remove(order);
                if (order.Direction == DirectionType.Buy) {
                    if (order.OpenClose == OpenCloseType.CloseToday) {
                        mTdShortPart.Frozen -= (short)order.Qty;
                    } else if (order.OpenClose == OpenCloseType.Close) {
                        mYdShortPart.Frozen -= (short)order.Qty;
                    }
                } else{
                    if (order.OpenClose == OpenCloseType.CloseToday) {
                        mTdLongPart.Frozen -= (short)order.Qty;
                    } else if (order.OpenClose == OpenCloseType.Close) {
                        mYdLongPart.Frozen -= (short)order.Qty;
                    }
                }
            }
        }
    }

    public class PositionPart {

        public long Position { get; set; }

        public long Frozen { get; set; }

        public long Free => Position - Frozen;

    }

    //接口发过来持仓
    public class BrokerPosition {
        public BrokerPosition(Instrument instrument, Exchange exchange, DirectionType direction, long position, long todayPosition, long historyPosition, long historyFrozen, long todayBSPosition, long todayBSFrozen) {
            Instrument = instrument;
            Exchange = exchange;
            Direction = direction;
            Position = position;
            TodayPosition = todayPosition;
            HistoryPosition = historyPosition;
            HistoryFrozen = historyFrozen;
            TodayBSPosition = todayBSPosition;
            TodayBSFrozen = todayBSFrozen;
        }

        public Instrument Instrument { get; private set; }
        public Exchange Exchange { get; private set; }

        public DirectionType Direction { get; private set; }

        public long Position { get; private set; }
        public long TodayPosition { get; private set; }
        public long HistoryPosition { get; private set; }
        public long HistoryFrozen { get; private set; }

        ///今日买卖持仓
        public long TodayBSPosition { get; private set; }
        ///今日买卖持仓冻结
        public long TodayBSFrozen { get; private set; }
    }
}
