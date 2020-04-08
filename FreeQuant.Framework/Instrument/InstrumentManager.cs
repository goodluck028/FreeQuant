using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class InstrumentManager {
        private static ConcurrentDictionary<string, Instrument> mInstrumentDic = new ConcurrentDictionary<string, Instrument>();

        static InstrumentManager() {
            BrokerManager.DefaultTdBroker.OnInstrument += addInstrument;
        }

        private static void addInstrument(Instrument inst) {
            mInstrumentDic.TryAdd(inst.InstrumentID, inst);
        }

        public static Instrument GetInstrument(string InstrumentID) {
            Instrument inst;
            if (mInstrumentDic.TryGetValue(InstrumentID, out inst)) {
                return inst;
            } else {
                return null;
            }
        }
    }
}
