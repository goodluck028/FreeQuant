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
        private DataReceiver() { }
        private static DataReceiver mInstance = new DataReceiver();
        public static DataReceiver Instance => mInstance;
        //
        private Action<Instrument> mOnInstrument;
        public event Action<Instrument> OnInstrument {
            add {
                mOnInstrument -= value;
                mOnInstrument += value;
            }
            remove {
                mOnInstrument -= value;
            }
        }
        private Action<Tick> mOnTick;
        public event Action<Tick> OnTick {
            add {
                mOnTick -= value;
                mOnTick += value;
            }
            remove {
                mOnTick -= value;
            }
        }
        //
        private BaseMdBroker MdBroker => BrokerManager.DefaultMdBroker;
        private BaseTdBroker TdBroker => BrokerManager.DefaultTdBroker;

        //bar生成器
        Dictionary<Instrument, BarGenerator> GeneratorMap = new Dictionary<Instrument, BarGenerator>();

        public void run() {
            MdBroker.OnTick += _onTick;
            MdBroker.OnStatusChanged += (ConnectionStatus mdStatus) => {
                if (mdStatus == ConnectionStatus.Connected) {
                    TdBroker.OnInstrument += _onInstrument;
                    TdBroker.OnStatusChanged += (ConnectionStatus tdStatus) => {
                        if (tdStatus == ConnectionStatus.Connected) {
                            TdBroker.QueryInstrument();
                        }
                    };
                    TdBroker.Login();
                }
            };
            //
            MdBroker.Login();
        }

        //数据
        private void _onTick(Tick tick) {
            if (!mInstruments.Contains(tick.Instrument))
                return;
            //
            BarGenerator generator;
            if (!GeneratorMap.TryGetValue(tick.Instrument, out generator)) {
                generator = new BarGenerator(tick.Instrument, BarSizeType.Min1);
                generator.OnBarTick += _onBarTick;
                GeneratorMap.Add(tick.Instrument, generator);
            }
            //
            mOnTick?.Invoke(tick);
            generator.addTick(tick);
        }

        //合约
        private string[] mProducts = Config.Instruments.Split(',');
        private HashSet<Instrument> mInstruments = new HashSet<Instrument>();
        private void _onInstrument(Instrument inst) {
            //建表
            foreach (string product in mProducts) {
                if (product.Equals(inst.ProductID)) {
                    mWriter.CreateTable(inst.ProductID);
                    //
                    mInstruments.Add(inst);
                    mOnInstrument?.Invoke(inst);
                    MdBroker.SubscribeMarketData(inst);
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
