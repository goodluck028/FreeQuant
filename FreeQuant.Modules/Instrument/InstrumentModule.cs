using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;
using FreeQuant.Modules.Broker;

namespace FreeQuant.Modules {
    public class InstrumentManager : BaseModule {
        private static ConcurrentDictionary<string, Instrument> mInstrumentMap = new ConcurrentDictionary<string, Instrument>();

        public override void OnLoad() {
            LogUtil.EnginLog("合约管理模块启动");
        }

        [OnEvent]
        private void OnInstrument(InstrumentReturnEvent evt) {
            Instrument inst = evt.Instrument;
            mInstrumentMap.TryAdd(inst.InstrumentID, inst);
        }

        public static Instrument GetInstrument(string instrumentId) {
            Instrument inst;
            if (mInstrumentMap.TryGetValue(instrumentId, out inst)) {
                return inst;
            } else {
                return null;
            }
        }


    }
}
