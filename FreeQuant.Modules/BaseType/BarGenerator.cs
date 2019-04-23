using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    internal delegate void OnBarDataDelegate(Bar bar, List<Tick> ticks);
    public class BarGenerator {
        //
        internal event OnBarDataDelegate OnBarData;
        //
        private string mInstrumentID;
        private List<Tick> mTickList;
        private Bar mBar;
        private BarSizeType mSizeType = BarSizeType.Min1;

        //
        public BarGenerator(string instrumentID, BarSizeType sizeType) {
            mInstrumentID = instrumentID;
            mSizeType = sizeType;
        }

        internal void addTick(Tick tick) {
            if (!tick.InstrumentId.Equals(mInstrumentID))
                return;
            //判断是否需要生成新bar
            if (mBar != null) {
                DateTime barTime = mTickList[0].UpdateTime;
                DateTime tickTime = tick.UpdateTime;
                int barQuotient = (barTime.Day * 1440 + barTime.Hour * 60 + barTime.Minute) / ((int)mSizeType);
                int tickQuotient = (tickTime.Day * 1440 + tickTime.Hour * 60 + barTime.Minute) / ((int)mSizeType);
                if (!barQuotient.Equals(tickQuotient)) {
                    OnBarData?.Invoke(mBar, mTickList);
                    mBar = null;
                    mTickList = null;
                }
            }

            //更新bar数据
            if (mBar == null) {
                mBar = new Bar();
                mTickList = new List<Tick>();
                mBar.InstrumentId = mInstrumentID;
                mBar.OpenPrice = tick.LastPrice;
                mBar.HighPrice = tick.LastPrice;
                mBar.LowPrice = tick.LastPrice;
                mBar.ClosePrice = tick.LastPrice;
                mBar.Volume = tick.Volume;
                mBar.OpenInterest = tick.OpenInterest;
                mBar.BeginTime = tick.UpdateTime;
                mBar.SizeType = mSizeType;
            } else {
                mBar.HighPrice = mBar.HighPrice < tick.LastPrice ? tick.LastPrice : mBar.HighPrice;
                mBar.LowPrice = mBar.LowPrice > tick.LastPrice ? tick.LastPrice : mBar.LowPrice;
                mBar.ClosePrice = tick.LastPrice;
            }
            mBar.Volume = tick.Volume;
            mBar.OpenInterest = tick.OpenInterest;
            mTickList.Add(tick);
        }
    }


}
