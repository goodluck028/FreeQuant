using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
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
}
