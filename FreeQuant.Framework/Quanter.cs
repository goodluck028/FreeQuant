using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework
{
    /// <summary>
    /// 整个架构中的核心类，创建和管理多个组件
    /// </summary>
    public static class Quanter
    {
        //各管理器里面的事件传递写在这里面，避免在某个管理器里面调用其他管理器时，被调用的管理器还未创建。
        static Quanter()
        {
            //将交易broker中获取到的合约，分发给合约管理器
            BrokerManager.DefaultTdBroker.OnInstrument += InstrumentManager.addInstrument;
        }

        //broker管理器
        public static BrokerManager BrokerManager = new BrokerManager();

        //持仓管理器
        public static PositionManager PositionManager = new PositionManager();

        //合约管理器
        public static InstrumentManager InstrumentManager = new InstrumentManager();

        //策略管理器
        public static StrategyManager StrategyManager = new StrategyManager();
    }
}
