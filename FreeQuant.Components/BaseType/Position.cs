using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Components {
    public class Position {
        private BaseStrategy mStrategy;
        private Instrument mInstrument;
        private long mVol = 0;
        private DateTime mLastTime;

        public Position(BaseStrategy strategy, Instrument instrument, long vol, DateTime lastTime) {
            mStrategy = strategy;
            mInstrument = instrument;
            mVol = vol;
            mLastTime = lastTime;
        }

        public BaseStrategy StrategyName {
            get { return mStrategy; }
            internal set { mStrategy = value; }
        }

        public Instrument Instrument {
            get { return mInstrument; }
            internal set { mInstrument = value; }
        }

        public long Vol {
            get { return mVol; }
            internal set { mVol = value; }
        }

        public DateTime LastTime {
            get { return mLastTime; }
            internal set { mLastTime = value; }
        }
    }

    public class BrokerPosition
    {
        private Instrument mInstrument;
        private long mYdLong;
        private long mFrozenYdLong;
        private long mYdShort;
        private long mFrozenYdShort;
        private long mTdLong;
        private long mFrozenTdLong;
        private long mTdShort;
        private long mFrozenTdShort;

        public BrokerPosition(Instrument instrument)
        {
            mInstrument = instrument;
        }

        public Instrument Instrument => mInstrument;

        public long YdLong
        {
            get { return mYdLong; }
            set { mYdLong = value; }
        }

        public long FrozenYdLong
        {
            get { return mFrozenYdLong; }
            set { mFrozenYdLong = value; }
        }

        public long YdShort
        {
            get { return mYdShort; }
            set { mYdShort = value; }
        }

        public long FrozenYdShort
        {
            get { return mFrozenYdShort; }
            set { mFrozenYdShort = value; }
        }

        public long TdLong
        {
            get { return mTdLong; }
            set { mTdLong = value; }
        }

        public long FrozenTdLong
        {
            get { return mFrozenTdLong; }
            set { mFrozenTdLong = value; }
        }

        public long TdShort
        {
            get { return mTdShort; }
            set { mTdShort = value; }
        }

        public long FrozenTdShort
        {
            get { return mFrozenTdShort; }
            set { mFrozenTdShort = value; }
        }
    }
}
