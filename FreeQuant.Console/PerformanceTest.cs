using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Console {
    public class PerformanceTest {

        [OnEvent]
        protected void onTime(DateTime time) {
        }

        [OnEvent]
        protected void onInt(int i) {
            System.Console.WriteLine(i);
        }
    }

    public class PerformanceTest2 : PerformanceTest { }
}
