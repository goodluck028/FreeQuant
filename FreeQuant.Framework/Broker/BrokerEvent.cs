using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class BrokerEvent {

        #region 交易

        public class TdLoginRequest {
        }

        public class TdLoginEvent {
            private bool mIsLoginSuccess;
            private string mErrorMsg;

            public TdLoginEvent(bool isLoginSuccess, string errorMsg) {
                mIsLoginSuccess = isLoginSuccess;
                mErrorMsg = errorMsg;
            }

            public bool IsLoginSuccess => mIsLoginSuccess;

            public string ErrorMsg => mErrorMsg;
        }

        public class TdLogoutRequest {
        }

        public class TdLogoutEvent {
            private bool mIsLogoutSuccess;
            private string mErrorMsg;

            public TdLogoutEvent(bool isLogoutSuccess, string errorMsg) {
                mIsLogoutSuccess = isLogoutSuccess;
                mErrorMsg = errorMsg;
            }

            public bool IsLogoutSuccess => mIsLogoutSuccess;

            public string ErrorMsg => mErrorMsg;
        }

        public class QueryInstrumentRequest {
        }

        internal class BrokerInstrumentEvent {
            private Instrument mInstrument;

            public BrokerInstrumentEvent(Instrument instrument) {
                mInstrument = instrument;
            }

            public Instrument Instrument => mInstrument;
        }

        public class InstrumentEvent {
            private Instrument mInstrument;

            public InstrumentEvent(Instrument instrument) {
                mInstrument = instrument;
            }

            public Instrument Instrument => mInstrument;
        }

        public class QueryPositionRequest {
        }

        public class PositionEvent {
            private string instrumentId;
            private DirectionType direction;
            private long ydPosition;
            private long ydFrozen;
            private long tdPosition;
            private long tdFrozen;

            public PositionEvent(string instrumentId, DirectionType direction, long ydPosition, long ydFrozen,
                long tdPosition, long tdFrozen) {
                this.instrumentId = instrumentId;
                this.direction = direction;
                this.ydPosition = ydPosition;
                this.ydFrozen = ydFrozen;
                this.tdPosition = tdPosition;
                this.tdFrozen = tdFrozen;
            }

            public string InstrumentId => instrumentId;

            public DirectionType Direction => direction;

            public long YdPosition => ydPosition;

            public long YdFrozen => ydFrozen;

            public long TdPosition => tdPosition;

            public long TdFrozen => tdFrozen;
        }

        public class TradeEvent {
            private Order mOrder;
            private long mTradeVol;

            public TradeEvent(Order order, long tradeVol) {
                mOrder = order;
                mTradeVol = tradeVol;
            }

            public Order Order => mOrder;

            public long TradeVol => mTradeVol;
        }

        #endregion

        #region 行情

        public class MdLoginRequest {
        }

        public class MdLoginEvent {
            private bool mIsLoginSuccess;
            private string mErrorMsg;

            public MdLoginEvent(bool isLoginSuccess, string errorMsg) {
                mIsLoginSuccess = isLoginSuccess;
                mErrorMsg = errorMsg;
            }

            public bool IsLoginSuccess => mIsLoginSuccess;

            public string ErrorMsg => mErrorMsg;
        }

        public class MdLogoutRequest {
        }

        public class MdLogoutEvent {
            private bool mIsLogoutSuccess;
            private string mErrorMsg;

            public MdLogoutEvent(bool isLogoutSuccess, string errorMsg) {
                mIsLogoutSuccess = isLogoutSuccess;
                mErrorMsg = errorMsg;
            }

            public bool IsLogoutSuccess => mIsLogoutSuccess;

            public string ErrorMsg => mErrorMsg;
        }

        public class SubscribeInstrumentRequest {
            private Instrument mInstrument;

            public SubscribeInstrumentRequest(Instrument instrument) {
                mInstrument = instrument;
            }

            public Instrument Instrument => mInstrument;
        }

        public class UnsubscribeInstrumentRequest {
            private Instrument mInstrument;

            public UnsubscribeInstrumentRequest(Instrument instrument) {
                mInstrument = instrument;
            }

            public Instrument Instrument => mInstrument;
        }

        public class TickEvent {
            private Tick mTick;

            public TickEvent(Tick tick) {
                mTick = tick;
            }

            public Tick Tick => mTick;
        }

        #endregion

        #region 其他
        public class MonitorEvent{}
        #endregion

    }
}
