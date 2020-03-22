using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using FreeQuant.Framework;
using IComponent = FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    internal class DataReceiver {

        //bar生成器
        Dictionary<string, BarGenerator> GeneratorMap = new Dictionary<string, BarGenerator>();

        //数据
        private void _onTick(Tick tick) {
            BarGenerator generator;
            if (!GeneratorMap.TryGetValue(tick.Instrument.InstrumentID, out generator)) {
                generator = new BarGenerator(tick.Instrument, BarSizeType.Min1);
                generator.OnBarTick += _onBarTick;
                GeneratorMap.Add(tick.Instrument.InstrumentID, generator);
            }
            //
            generator.addTick(tick);
        }

        //合约
        private void _onInstrument(Instrument inst) {
            //建表
            string[] products = Config.Instruments.Split(',');
            foreach (string product in products) {
                if (product.Equals(RegexUtils.TakeProductName(inst.InstrumentID))) {
                    string name = RegexUtils.TakeShortInstrumentID(inst.InstrumentID);
                    mWriter.CreateTable(name);
                }
            }
        }

        //
        IDataWriter mWriter = new SqlServerWriter();
        private void _onBarTick(Bar bar, List<Tick> ticks) {
            mWriter.InsertBar(bar);
            mWriter.InsertTicks(ticks);
        }
    }
}
