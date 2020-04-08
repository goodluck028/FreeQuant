using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public class Bar {
        public Bar(Instrument instrument, double openPrice, DateTime beginTime, BarSizeType mSizeType) {
            Instrument = instrument;
            OpenPrice = openPrice;
            BeginTime = beginTime;
            this.mSizeType = mSizeType;
        }


        // 合约代码;
        public Instrument Instrument { get; private set; }
        // 开始价
        public double OpenPrice { get; private set; }
        // 最高价
        public double HighPrice { get; internal set; }
        // 最低价
        public double LowPrice { get; internal set; }
        // 结束价
        public double ClosePrice { get; internal set; }
        // 成交量
        public double Volume { get; internal set; }
        // 持仓量
        public double OpenInterest { get; internal set; }
        // 开始时间
        public DateTime BeginTime { get; private set; }
        // 长度
        public BarSizeType mSizeType { get; private set; }
    }

    public enum BarSizeType {
        Min1 = 1,
        Min5 = 5,
        Min10 = 10,
        Min20 = 20,
        Min30 = 30,
        Hour1 = 60,
        Hour2 = 120,
        Hour4 = 240,
        Day1 = 1440
    }
}
