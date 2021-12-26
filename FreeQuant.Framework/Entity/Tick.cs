using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public class Tick {
        public Tick(Instrument instrument, double lastPrice, double bidPrice, long bidVolume, double askPrice, long askVolume, double volume, double openInterest, DateTime updateTime, double upperLimitPrice, double lowerLimitPrice) {
            Instrument = instrument;
            LastPrice = lastPrice;
            BidPrice = bidPrice;
            BidVolume = bidVolume;
            AskPrice = askPrice;
            AskVolume = askVolume;
            Volume = volume;
            OpenInterest = openInterest;
            UpdateTime = updateTime;
            UpperLimitPrice = upperLimitPrice;
            LowerLimitPrice = lowerLimitPrice;
        }

        // 合约;
        public Instrument Instrument { get; private set; }
        // 最新价
        public double LastPrice { get; private set; }
        //申买价一
        public double BidPrice { get; private set; }
        // 申买量一
        public long BidVolume { get; private set; }
        //申卖价一
        public double AskPrice { get; private set; }
        //申卖量一
        public long AskVolume { get; private set; }
        //数量
        public double Volume { get; private set; }
        //持仓量
        public double OpenInterest { get; private set; }
        //最后修改时间
        public DateTime UpdateTime { get; private set; }
        //涨停板价
        public double UpperLimitPrice { get; private set; }
        //跌停板价
        public double LowerLimitPrice { get; private set; }
    }
}
