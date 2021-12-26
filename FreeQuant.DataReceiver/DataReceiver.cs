using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using FreeQuant.Framework;
using IComponent = FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    /// <summary>
    /// 数据接收器
    /// </summary>
    internal class DataReceiver {
        //合约事件
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

        //tick事件
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

        //borker
        private BaseMdBroker MdBroker => Quanter.BrokerManager.DefaultMdBroker;
        private BaseTdBroker TdBroker => Quanter.BrokerManager.DefaultTdBroker;

        //bar生成器
        Dictionary<Instrument, BarGenerator> GeneratorMap = new Dictionary<Instrument, BarGenerator>();

        public void run() {
            //订阅行情
            MdBroker.OnTick += _onTick;
            //监听状态变化
            MdBroker.OnStatusChanged += (ConnectionStatus mdStatus) => {
                if (mdStatus == ConnectionStatus.Connected) {
                    //当行情服务器连接成功时，连接交易服务器
                    TdBroker.OnInstrument += _onInstrument;
                    TdBroker.OnStatusChanged += (ConnectionStatus tdStatus) => {
                        if (tdStatus == ConnectionStatus.Connected) {
                            //查询合约
                            TdBroker.QueryInstrument();
                        }
                    };
                    //交易登录
                    TdBroker.Login();
                }
            };
            //行情登录
            MdBroker.Login();
        }

        //处理tick
        private void _onTick(Tick tick) {
            if (!mInstruments.Contains(tick.Instrument))
                return;
            //新tick需要创建bar生成器
            BarGenerator generator;
            if (!GeneratorMap.TryGetValue(tick.Instrument, out generator)) {
                generator = new BarGenerator(tick.Instrument, BarSizeType.Min1);
                generator.OnBarTick += _onBarTick;
                GeneratorMap.Add(tick.Instrument, generator);
            }
            //触发tick事件
            mOnTick?.Invoke(tick);
            //生成bar
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
                    //触发合约事件
                    mInstruments.Add(inst);
                    mOnInstrument?.Invoke(inst);
                    //订阅行情
                    MdBroker.SubscribeMarketData(inst);
                }
            }
        }

        //写数据到数据库
        IDataWriter mWriter = new SqlServerWriter();
        private void _onBarTick(Bar bar, List<Tick> ticks) {
            mWriter.InsertBar(bar);
            mWriter.InsertTicks(ticks);
        }
    }
}
