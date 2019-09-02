using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using FreeQuant.Components;
using FreeQuant.Framework;
using MySql.Data.MySqlClient;

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
                generator = new BarGenerator(tick.Instrument, BarSizeType.Min1);
                generator.OnBarTick += _onBarTick;
                GeneratorMap.Add(tick.Instrument.InstrumentID, generator);
            }
            //
            generator.addTick(tick);

        }

        private MySqlHelper mWriter = new MySqlHelper();
        private void _onBarTick(Bar bar, List<Tick> ticks) {
            return;
            string tbName = RegexUtils.TakeShortInstrumentID(bar.Instrument.InstrumentID);
            StringBuilder sb = new StringBuilder();
            //bar
            sb.Append($@"INSERT INTO `hisdata_future`.`bar1min_{tbName}`
                        (`exchange_id`,
                        `instrument_id`,
                        `begin_time`,
                        `open_price`,
                        `high_price`,
                        `low_price`,
                        `close_price`,
                        `volume`,
                        `multiplier`,
                        `open_interest`)
                    VALUES
                        ('{ bar.Instrument.ExchangeID }',
                        '{ bar.BeginTime }',
                        '{ bar.BeginTime }',
                        { bar.OpenPrice },
                        { bar.HighPrice },
                        { bar.LowPrice },
                        { bar.ClosePrice },
                        { bar.Volume },
                        { bar.Instrument.VolumeMultiple },
                        { bar.OpenInterest }); ");
            mWriter.MysqlCommand(sb.ToString());
            //tick
            sb.Clear();
            if (ticks.Count == 0)
                return;
            sb.Append($@"INSERT INTO `hisdata_future`.`tick_{tbName}`
                        (`instrument_id`,
                        `last_price`,
                        `bid_price`,
                        `bid_vol`,
                        `ask_price`,
                        `ask_vol`,
                        `vol`,
                        `open_interest`,
                        `uper_limit`,
                        `lower_limit`,
                        `update_time`)
                        VALUES  ");
            for (int i = 0; i < ticks.Count; i++) {
                Tick t = ticks[i];
                string time = t.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                sb.Append($@" ('{t.Instrument.InstrumentID }',
                                {t.LastPrice },
                                {t.BidPrice },
                                {t.BidVolume },
                                {t.AskPrice },
                                {t.AskVolume },
                                {t.Volume },
                                {t.OpenInterest},
                                {t.UpperLimitPrice },
                                {t.LowerLimitPrice },
                                '{time}')");
                if (i < ticks.Count - 1) {
                    sb.Append(",");
                }
            }
            mWriter.MysqlCommand(sb.ToString());
        }
    }
}
