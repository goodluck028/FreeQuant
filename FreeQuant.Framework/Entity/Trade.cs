using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public class BrokerTrade {
        public BrokerTrade(Instrument instrument, Exchange exchange, DirectionType direction, long qty, double price, OpenCloseType openClose, DateTime tradeTime) {
            Instrument = instrument;
            Exchange = exchange;
            Direction = direction;
            Qty = qty;
            Price = price;
            OpenClose = openClose;
            TradeTime = tradeTime;
        }

        public Instrument Instrument { get; private set; }
        public Exchange Exchange { get; private set; }

        public DirectionType Direction { get; private set; }
        public long Qty { get; private set; }
        public double Price { get; private set; }
        public OpenCloseType OpenClose { get; private set; }
        public DateTime TradeTime { get; private set; }
    }
}
