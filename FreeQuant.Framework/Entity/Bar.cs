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
        private Bar mBar;
        private BarSizeType mSizeType = BarSizeType.Min1;
        //
        private Action<Bar> mOnBar;
        public event Action<Bar> OnBar
        {
            add
            {
                mOnBar -= value;
                mOnBar += value;
            }
            remove
            {
                mOnBar-= value;
            }
        }
        //
        public BarGenerator(Instrument instrument, BarSizeType sizeType)
        {
            mInstrument = instrument;
            mSizeType = sizeType;
        }

        //
        public void PutTick(Tick tick)
        {
            if (!tick.Instrument.Equals(mInstrument))
                return;
            //判断是否到达了bar的生成点
            if (mBar != null)
            {
                DateTime barTime = mBar.BeginTime;
                DateTime tickTime = tick.UpdateTime;
                //计算bar开始时间除以周期得到的整倍数
                int barQuotient = (barTime.Day * 1440 + barTime.Hour * 60 + barTime.Minute) / ((int)mSizeType);
                //计算最新tick时间除以周期得到的整倍数
                int tickQuotient = (tickTime.Day * 1440 + tickTime.Hour * 60 + tickTime.Minute) / ((int)mSizeType);
                //如果两者不同，认为新周期开始，触发bar事件
                if (!barQuotient.Equals(tickQuotient))
                {
                    mOnBar?.Invoke(mBar);
                    mBar = null;
                }
            }

            //更新bar数据
            if (mBar == null)
            {
                mBar = new Bar(mInstrument, tick.LastPrice, tick.UpdateTime, mSizeType);
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
        }
    }
}
