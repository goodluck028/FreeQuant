using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using FreeQuant.EventEngin;
using System.Data;
using System.Reflection;
using SQLite;

namespace FreeQuant.Framework {
    public class DataManager :IComponent{

        public void OnLoad() {
            EventBus.Register(this);
            LogUtil.EnginLog("数据库组件启动");
        }

        public void OnReady() {}

        //
        [OnLog]
        private void OnPostion(StrategyEvent.ChangePositionEvent evt) {
            Position p = evt.Position;
        }

        [OnLog]
        private void OnOrder(Order order) {
        }
        //
        private string dbFile = @"./data/QuantData.db";
        private void createTable() {
            var db = new SQLiteConnection(dbFile);
            db.CreateTable<StrategyEntity>();
            db.CreateTable<PositionEntity>();
            db.CreateTable<PositionEntity>();
        }
        //

    }
}
