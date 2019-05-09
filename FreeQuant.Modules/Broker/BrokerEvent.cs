using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    #region 交易
    public class TdBrokerLoginRequest { }

    public class TdBrokerLoginEvent {
        private bool mIsLoginSuccess;
        private string mErrorMsg;

        public TdBrokerLoginEvent(bool isLoginSuccess, string errorMsg) {
            mIsLoginSuccess = isLoginSuccess;
            mErrorMsg = errorMsg;
        }

        public bool IsLoginSuccess => mIsLoginSuccess;

        public string ErrorMsg => mErrorMsg;
    }

    public class TdBrokerLogoutRequest { }

    public class TdBrokerLogoutEvent {
        private bool mIsLogoutSuccess;
        private string mErrorMsg;

        public TdBrokerLogoutEvent(bool isLogoutSuccess, string errorMsg) {
            mIsLogoutSuccess = isLogoutSuccess;
            mErrorMsg = errorMsg;
        }

        public bool IsLogoutSuccess => mIsLogoutSuccess;

        public string ErrorMsg => mErrorMsg;
    }

    public class QueryInstrumentRequest { }

    public class InstrumentEvent {
        private Instrument mInstrument;
        public InstrumentEvent(Instrument instrument) {
            mInstrument = instrument;
        }
        public Instrument Instrument => mInstrument;
    }

    public class QueryPositionRequest { }

    public class BrokerPositionEvent
    {
        private string instrumentId;
        private DirectionType direction;
        private long ydPosition;
        private long ydFrozen;
        private long tdPosition;
        private long tdFrozen;

        public BrokerPositionEvent(string instrumentId, DirectionType direction, long ydPosition, long ydFrozen, long tdPosition, long tdFrozen)
        {
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

    public class BrokerOrderRequest {
        private BrokerOrder mOrder;

        public BrokerOrderRequest(BrokerOrder order) {
            mOrder = order;
        }

        public BrokerOrder Order => mOrder;
    }

    public class CancelOrderRequest {
        private BrokerOrder mOrder;

        public CancelOrderRequest(BrokerOrder order) {
            mOrder = order;
        }

        public BrokerOrder Order => mOrder;
    }

    public class BrokerOrderEvent
    {
        private string localId;
        private string orderId;
        private long tradeVol;
        private long leftVol;
        private OrderStatus status;

        public BrokerOrderEvent(string localId, string orderId, long tradeVol, long leftVol, OrderStatus status)
        {
            this.localId = localId;
            this.orderId = orderId;
            this.tradeVol = tradeVol;
            this.leftVol = leftVol;
            this.status = status;
        }

        public string LocalId => localId;

        public string OrderId => orderId;

        public long TradeVol => tradeVol;

        public long LeftVol => leftVol;

        public OrderStatus Status => status;
    }

    public class BrokerTradeEvent
    {
        private string localId;
        private long tradeVol;

        public BrokerTradeEvent(string localId, long tradeVol)
        {
            this.localId = localId;
            this.tradeVol = tradeVol;
        }

        public string LocalId => localId;

        public long TradeVol => tradeVol;
    }
    #endregion

    #region 行情
    public class MdBrokerLoginRequest { }

    public class MdBrokerLoginEvent {
        private bool mIsLoginSuccess;
        private string mErrorMsg;

        public MdBrokerLoginEvent(bool isLoginSuccess, string errorMsg) {
            mIsLoginSuccess = isLoginSuccess;
            mErrorMsg = errorMsg;
        }

        public bool IsLoginSuccess => mIsLoginSuccess;

        public string ErrorMsg => mErrorMsg;
    }

    public class MdBrokerLogoutRequest { }

    public class MdBrokerLogoutEvent {
        private bool mIsLogoutSuccess;
        private string mErrorMsg;

        public MdBrokerLogoutEvent(bool isLogoutSuccess, string errorMsg) {
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

    public class TickEvent
    {
        private Tick mTick;

        public TickEvent(Tick tick)
        {
            mTick = tick;
        }

        public Tick Tick => mTick;
    }
    #endregion
}
