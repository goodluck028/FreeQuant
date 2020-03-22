using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class InstrumentManager {
        private static ConcurrentDictionary<string, Instrument> SInstrumentMap = new ConcurrentDictionary<string, Instrument>();

        public static void SetInstrument(Instrument inst) {
            SInstrumentMap.TryAdd(inst.InstrumentID, inst);
        }

        public static Instrument GetInstrument(string instrumentId) {
            Instrument inst;
            if (SInstrumentMap.TryGetValue(instrumentId, out inst)) {
                return inst;
            } else {
                return null;
            }
        }
    }
}
