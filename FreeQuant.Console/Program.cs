using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Console {
    class Program {
        static void Main(string[] args) {
            ModuleLoader.LoadAllModules();
            PerformanceTest test = new PerformanceTest();
            EventBus.Register(test);
            for (int i = 0; i < 100; i++) {
                Thread.Sleep(500);
                EventBus.PostEvent(i);
                if (i == 50) {
                    EventBus.UnRegister(test);
                }
            }

            System.Console.ReadKey();
        }
    }
}
