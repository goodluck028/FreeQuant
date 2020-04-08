using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.Console {
    class Program {
        static Action<string> pt;
        static event Action<string> print {
            add {
                pt -= value;
                pt += value;
            }
            remove {
                pt -= value;
            }
        }
        static void Main(string[] args) {
            add();
            add();
            pt?.Invoke("aaa");
            System.Console.ReadKey();
        }

        static void add() {
            print += log=>{
                System.Console.WriteLine(log);
            };
        }
    }
}
