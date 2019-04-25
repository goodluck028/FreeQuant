using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Console {
    public class PerformanceTest {

        [OnEvent]
        public void onTime(DateTime time)
        {
            System.Console.WriteLine((DateTime.Now-time).TotalMilliseconds);
        }


        private DateTime BeginTime;

        [OnEvent]
        public void onInt(int i)
        {
        }
    }
}
