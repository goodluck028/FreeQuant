using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Modules.Utils {
    internal class LogModule : BaseModule {
        public override void OnLoad() {
            Console.WriteLine("日志模块启动");
        }

        [OnLog]
        private void OnEngineError(Error error) {
            LogUtil.EnginLog(error.Exception.Message);
        }
    }
}
