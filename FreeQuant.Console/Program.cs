using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreeQuant.Framework;
using FreeQuant.Components;

namespace FreeQuant.Console {
    class Program {
        static void Main(string[] args)
        {
            while (true)
            {
                ConsoleKeyInfo info = System.Console.ReadKey();
                if (info.Modifiers == ConsoleModifiers.Control && info.Key == ConsoleKey.Q)
                {
                    System.Console.WriteLine("aaaaa");
                    break;
                }
            }

            System.Console.ReadKey();

            ComponentLoader.LoadAllComponents();
            FrameworkDispatcher.Run();
            System.Console.ReadKey();
        }
    }
}
