using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules.Broker {
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

    public class InstrumentReturnEvent {
        private Instrument mInstrument;
        public InstrumentReturnEvent(Instrument instrument) {
            mInstrument = instrument;
        }
        public Instrument Instrument => mInstrument;
    }

    public class SendOrderRequest {
        private BrokerOrder mOrder;

        public SendOrderRequest(BrokerOrder order) {
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

    public class OrderReturnEvent {
        private BrokerOrder mOrder;

        public OrderReturnEvent(BrokerOrder order) {
            mOrder = order;
        }

        public BrokerOrder Order => mOrder;
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

    public class TickReturnEvent
    {
        private Tick mTick;

        public TickReturnEvent(Tick tick)
        {
            mTick = tick;
        }

        public Tick Tick => mTick;
    }
    #endregion
}
