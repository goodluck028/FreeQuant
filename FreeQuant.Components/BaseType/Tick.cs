using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Components {
    public class Tick {
        // 合约;
        private Instrument mInstrument;
        // 最新价
        private double mLastPrice;
        //申买价一
        private double mBidPrice;
        // 申买量一
        private long mBidVolume;
        //申卖价一
        private double mAskPrice;
        //申卖量一
        private long mAskVolume;
        //数量
        private double mVolume;
        //持仓量
        private double mOpenInterest;
        //最后修改时间
        private DateTime mUpdateTime;
        //涨停板价
        private double mUpperLimitPrice;
        //跌停板价
        private double mLowerLimitPrice;

        public Tick(Instrument instrument, double lastPrice, double bidPrice, long bidVolume, double askPrice, long askVolume, double volume, double openInterest, DateTime updateTime, double upperLimitPrice, double lowerLimitPrice) {
            mInstrument = instrument;
            mLastPrice = lastPrice;
            mBidPrice = bidPrice;
            mBidVolume = bidVolume;
            mAskPrice = askPrice;
            mAskVolume = askVolume;
            mVolume = volume;
            mOpenInterest = openInterest;
            mUpdateTime = updateTime;
            mUpperLimitPrice = upperLimitPrice;
            mLowerLimitPrice = lowerLimitPrice;
        }

        public Instrument Instrument => mInstrument;

        public double LastPrice => mLastPrice;

        public double BidPrice => mBidPrice;

        public long BidVolume => mBidVolume;

        public double AskPrice => mAskPrice;

        public long AskVolume => mAskVolume;

        public double Volume => mVolume;

        public double OpenInterest => mOpenInterest;

        public DateTime UpdateTime => mUpdateTime;

        public double UpperLimitPrice => mUpperLimitPrice;

        public double LowerLimitPrice => mLowerLimitPrice;
    }
}
