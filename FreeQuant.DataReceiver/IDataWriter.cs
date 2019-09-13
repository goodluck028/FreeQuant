using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    internal interface IDataWriter
    {
        void CreateTable(string dbName);
        void InsertBar(Bar bar);
        void InsertTicks(List<Tick> ticks);
    }
}
