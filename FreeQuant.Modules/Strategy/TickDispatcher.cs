using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Modules {
    internal class TickDispatcher {
        private Dictionary<Instrument, HashSet<BaseStrategy>> mTickSendMap = new Dictionary<Instrument, HashSet<BaseStrategy>>();

        public void SubscribTick(Instrument inst, BaseStrategy stg) {
            HashSet<BaseStrategy> set;
            if (!mTickSendMap.TryGetValue(inst, out set)) {
                set = new HashSet<BaseStrategy>();
                mTickSendMap.Add(inst, set);
            }
            set.Add(stg);
        }

        public void SendTick(Tick tick) {
            HashSet<BaseStrategy> set;
            if (mTickSendMap.TryGetValue(tick.Instrument, out set)) {
                foreach (BaseStrategy stg in set) {
                    stg.SendTick(tick);
                }
            }
        }

        public List<Instrument> InstrumentList => mTickSendMap.Keys.ToList();
    }
}
