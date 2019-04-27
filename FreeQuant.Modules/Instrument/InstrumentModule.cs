using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Modules {
    internal class InstrumentManager : BaseModule {
        public override void OnLoad() {
            FqLog.EnginLog("合约管理模块启动");
        }
    }
}
