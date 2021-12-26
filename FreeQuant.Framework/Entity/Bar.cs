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

    public class BarGenerator
    {
        //
        private Instrument mInstrument;
        private List<Tick> mTickList;
        private Bar mBar;
        private BarSizeType mSizeType = BarSizeType.Min1;
        //
        private Action<Bar, List<Tick>> mOnBarTick;
        public event Action<Bar, List<Tick>> OnBarTick
        {
            add
            {
                mOnBarTick -= value;
                mOnBarTick += value;
            }
            remove
            {
                mOnBarTick -= value;
            }
        }
        //
        public BarGenerator(Instrument instrument, BarSizeType sizeType)
        {
            mInstrument = instrument;
            mSizeType = sizeType;
        }

        //
        public void addTick(Tick tick)
        {
            if (!tick.Instrument.Equals(mInstrument))
                return;

            //判断是否需要生成新bar
            refreshBar(tick.UpdateTime);

            //更新bar数据
            if (mBar == null)
            {
                mBar = new Bar(mInstrument, tick.LastPrice, tick.UpdateTime, mSizeType);
                mTickList = new List<Tick>();
                mBar.HighPrice = tick.LastPrice;
                mBar.LowPrice = tick.LastPrice;
                mBar.ClosePrice = tick.LastPrice;
                mBar.Volume = tick.Volume;
                mBar.OpenInterest = tick.OpenInterest;
            }
            else
            {
                mBar.HighPrice = mBar.HighPrice < tick.LastPrice ? tick.LastPrice : mBar.HighPrice;
                mBar.LowPrice = mBar.LowPrice > tick.LastPrice ? tick.LastPrice : mBar.LowPrice;
                mBar.ClosePrice = tick.LastPrice;
                mBar.Volume += tick.Volume;
                mBar.OpenInterest = tick.OpenInterest;
            }
            mTickList.Add(tick);
        }
        //判断是否需要生成新bar
        private void refreshBar(DateTime current)
        {
            if (mBar == null)
                return;
            //
            DateTime barTime = mTickList[0].UpdateTime;
            int barQuotient = (barTime.Day * 1440 + barTime.Hour * 60 + barTime.Minute) / ((int)mSizeType);
            int tickQuotient = (current.Day * 1440 + current.Hour * 60 + current.Minute) / ((int)mSizeType);
            if (!barQuotient.Equals(tickQuotient))
            {
                mOnBarTick?.Invoke(mBar, mTickList);
                mBar = null;
                mTickList = null;
            }
        }
    }
}
