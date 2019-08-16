using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Components {
    public class BarGenerator {
        //
        private string mInstrumentID;
        private List<Tick> mTickList;
        private Bar mBar;
        private BarSizeType mSizeType = BarSizeType.Min1;
        //
        public Action<Bar, List<Tick>> OnBarTick;
        //
        public BarGenerator(string instrumentID, BarSizeType sizeType) {
            mInstrumentID = instrumentID;
            mSizeType = sizeType;
            EventBus.Register(this);
        }

        //每过1分钟判断一次，是否要生成新bar
        public class TimerEvent { }
        public static Timer timer = new Timer(_ => OnCallBack(), null, 0, 1000 * 60);
        private static void OnCallBack() {
            TimerEvent evt = new TimerEvent();
            EventBus.PostEvent(evt);
        }

        [OnEvent]
        private void onTimerEvent(TimerEvent evt) {
            refreshBar(DateTime.Now);
        }

        //
        public void addTick(Tick tick) {
            if (!tick.Instrument.Equals(mInstrumentID))
                return;

            //判断是否需要生成新bar
            refreshBar(tick.UpdateTime);

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
        //判断是否需要生成新bar
        private void refreshBar(DateTime current) {
            if (mBar == null)
                return;
            //
            DateTime barTime = mTickList[0].UpdateTime;
            int barQuotient = (barTime.Day * 1440 + barTime.Hour * 60 + barTime.Minute) / ((int)mSizeType);
            int tickQuotient = (current.Day * 1440 + current.Hour * 60 + barTime.Minute) / ((int)mSizeType);
            if (!barQuotient.Equals(tickQuotient)) {
                OnBarTick?.Invoke(mBar, mTickList);
                mBar = null;
                mTickList = null;
            }
        }
    }


}
