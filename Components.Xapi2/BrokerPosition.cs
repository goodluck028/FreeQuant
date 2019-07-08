using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.Xapi2 {
    internal class BrokerPosition {
        private string mInstrumentId;
        private PositionPart mTdLongPart = new PositionPart();
        private PositionPart mTdShortPart = new PositionPart();
        private PositionPart mYdLongPart = new PositionPart();
        private PositionPart mYdShortPart = new PositionPart();
        
        //todo:添加持仓管理逻辑
    }

    internal class PositionPart {
        private long mPosition;
        private long mFrozenPosition;

        public long Position => mPosition;

        public long FrozenPosition => mFrozenPosition;

        public long FreePosition => mPosition - mFrozenPosition;

        public void add(long vol) {
            mPosition += vol;
        }

        public void addFrozen(int vol) {
            mFrozenPosition += vol;
            mFrozenPosition = mFrozenPosition > mPosition ? mPosition : mFrozenPosition;
        }

    }
}
