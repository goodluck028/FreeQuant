using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Components;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    internal class DataReceiver {
        //单例
        private static DataReceiver mInstance = new DataReceiver();
        private DataReceiver() {
            EventBus.Register(this);
        }
        public static DataReceiver Instance => mInstance;
        //日志
        public Action<string> OnLog;

        [OnLog]
        private void _onLog(LogEvent log) {
            OnLog?.Invoke(log.Content);
        }

        //数据
        [OnEvent]
        private void _onTick(TickEvent tick) {

        }
    }
}
