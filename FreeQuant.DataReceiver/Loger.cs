using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    internal class Loger {
        public static void Info(string content) {
            LogUtil.Log("info", content);
        }
        public static void Error(string content) {
            LogUtil.Log("error", content);
        }
    }
}
