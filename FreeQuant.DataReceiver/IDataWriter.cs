using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    /// <summary>
    /// 数据存储接口
    /// </summary>
    internal interface IDataWriter
    {
        //创建数据库
        void CreateDb();
        //创建表
        void CreateTable(string product);
        //插入bar
        void InsertBar(Bar bar);
        //插入tick
        void InsertTick(Tick tick);
        //插入ticks
        void InsertTicks(List<Tick> ticks);
    }
}
