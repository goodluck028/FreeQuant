using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Modules {
    [Component]
    internal class LogModule{
        public LogModule()
        {
            Console.WriteLine("日志组件启动");
        }

        [OnLog]
        private void OnEngineError(Error error) {
            LogUtil.EnginLog(error.Exception.Message);
        }
    }
}
