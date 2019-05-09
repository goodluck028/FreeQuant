using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Modules.Broker {
    [Component]
    class BrokerPositionManager {

        private static Dictionary<Instrument, BrokerPosition> mPositionMap = new Dictionary<Instrument, BrokerPosition>();

        public BrokerPositionManager() {
            EventBus.Register(this);
            LogUtil.EnginLog("Broker持仓管理组件启动");
        }

        [OnEvent]
        private void OnBrokerPosition(BrokerPositionEvent evt) {
            Instrument inst = InstrumentManager.GetInstrument(evt.InstrumentId);
            if (inst == null)
                return;
            //
            BrokerPosition position;
            if (!mPositionMap.TryGetValue(inst, out position)) {
                position = new BrokerPosition(inst);
                mPositionMap.Add(position.Instrument, position);
            }

            if (evt.Direction == DirectionType.Buy) {
                position.YdLong = evt.YdPosition;
                position.FrozenYdLong = evt.YdFrozen;
                position.TdLong = evt.TdPosition;
                position.FrozenTdLong = evt.TdFrozen;
            } else {
                position.YdShort = evt.YdPosition;
                position.FrozenYdShort = evt.YdFrozen;
                position.TdShort = evt.TdPosition;
                position.FrozenTdShort = evt.TdFrozen;
            }
        }

        public static void QuerryPosition() {
            EventBus.PostEvent(new QueryPositionRequest());
        }
    }
}
