using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreeQuant.Components;
using FreeQuant.Framework;

namespace FreeQuant.Console {
    class Program {
        static void Main(string[] args)
        {
            LogUtil.Logger.Record();
            //启动
            ComponentLoader.LoadAllComponents();
            ComponentsScheduler.Instance.start();
            //键盘退出
            while (true) {
                System.Console.WriteLine("输入Ctrl + Q退出");
                ConsoleKeyInfo info = System.Console.ReadKey();
                if (info.Modifiers == ConsoleModifiers.Control && info.Key == ConsoleKey.Q) {
                    ComponentsScheduler.Instance.Stop();
                    break;
                }
            }
        }
    }
}
