using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Console {
    public class PerformanceTest {

        [OnEvent]
        private void onTime(DateTime time)
        {
            System.Console.WriteLine((DateTime.Now-time).TotalMilliseconds);
        }


        private DateTime BeginTime;

        [OnEvent]
        private void onInt(int i)
        {
        }
    }
}
