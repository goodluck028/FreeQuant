using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Components;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    [Component]
    public class DataReceiver {
        //单例
        public DataReceiver() {
            EventBus.Register(this);
        }

        //bar生成器
        Dictionary<string, BarGenerator> GeneratorMap = new Dictionary<string, BarGenerator>();

        //数据
        [OnEvent]
        private void _onTick(BrokerEvent.TickEvent evt) {
            Tick tick = evt.Tick;
            BarGenerator generator;
            if (!GeneratorMap.TryGetValue(tick.Instrument.InstrumentID, out generator)) {
                generator = new BarGenerator(tick.Instrument.InstrumentID, BarSizeType.Min1);
                generator.OnBarTick += _onBarTick;
                GeneratorMap.Add(tick.Instrument.InstrumentID, generator);
            }
            //
            generator.addTick(tick);

        }

        private void _onBarTick(Bar bar, List<Tick> ticks) {
            //todo 存储数据
        }
    }
}
