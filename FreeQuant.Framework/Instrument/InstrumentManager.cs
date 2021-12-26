using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework
{
    /// <summary>
    /// 合约管理器
    /// </summary>
    public class InstrumentManager
    {

        //只能内部初始化
        internal InstrumentManager()
        {
        }

        //存储合约的字典
        private ConcurrentDictionary<string, Instrument> mInstrumentDic = new ConcurrentDictionary<string, Instrument>();

        //添加合约
        internal void addInstrument(Instrument inst)
        {
            mInstrumentDic.TryAdd(inst.InstrumentID, inst);
        }

        //获取合约
        public Instrument GetInstrument(string InstrumentID)
        {
            Instrument inst;
            if (mInstrumentDic.TryGetValue(InstrumentID, out inst))
            {
                return inst;
            }
            else
            {
                return null;
            }
        }
    }
}
