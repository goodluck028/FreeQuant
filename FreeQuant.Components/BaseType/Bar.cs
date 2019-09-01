using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Components {
    public class Bar {
        // 合约代码;
        private Instrument mInstrument;
        // 开始价
        private double mOpenPrice;
        // 最高价
        private double mHighPrice;
        // 最低价
        private double mLowPrice;
        // 结束价
        private double mClosePrice;
        // 成交量
        private long mVolume;
        // 持仓量
        private double mOpenInterest;
        // 开始时间
        private DateTime mBeginTime;
        // 长度
        private BarSizeType mSizeType;

        public Instrument Instrument {
            get { return mInstrument; }
            internal set { mInstrument = value; }
        }

        public double OpenPrice {
            get { return mOpenPrice; }
            internal set { mOpenPrice = value; }
        }

        public double HighPrice {
            get { return mHighPrice; }
            internal set { mHighPrice = value; }
        }

        public double LowPrice {
            get { return mLowPrice; }
            internal set { mLowPrice = value; }
        }

        public double ClosePrice {
            get { return mClosePrice; }
            internal set { mClosePrice = value; }
        }

        public long Volume {
            get { return mVolume; }
            internal set { mVolume = value; }
        }

        public double OpenInterest {
            get { return mOpenInterest; }
            internal set { mOpenInterest = value; }
        }

        public DateTime BeginTime {
            get { return mBeginTime; }
            internal set { mBeginTime = value; }
        }

        public BarSizeType SizeType
        {
            get { return mSizeType; }
            internal set { mSizeType = value; }
        }
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
