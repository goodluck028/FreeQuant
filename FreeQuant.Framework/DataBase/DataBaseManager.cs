using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using FreeQuant.EventEngin;

namespace FreeQuant.Framework {
    [AutoCreate]
    public class DataBaseManager{
        public DataBaseManager()
        {
            EventBus.Register(this);
            LogUtil.EnginLog("数据库组件启动");
        }

        [OnLog]
        private void OnPostion(StrategyEvent.ChangePositionEvent evt) {
            savePosition(evt.Position);
        }

        [OnLog]
        private void OnOrder(Order order) {
            saveOrder(order);
        }

        //持仓
        public static long GetPosition(string strategyName, string instrumentID) {
            string sql = $@" select position from t_position where strategy_name = '{strategyName}' and instrument_id = '{instrumentID}'";
            object vol = SQLiteHelper.ExecuteScalar(sql);
            return vol == null ? 0 : (long)vol;
        }

        private void savePosition(Position p) {
            string sql = @"replace into [t_position]
                            ([strategy_name], [instrument_id], [position], [last_time]) 
                            values
                            (@strategyName,@instrumentID,@vol,@lastTime)";
            SQLiteHelper.ExecuteNonQuery(sql
                , new System.Data.SQLite.SQLiteParameter("strategyName", p.StrategyName)
                , new System.Data.SQLite.SQLiteParameter("instrumentID", p.Instrument.InstrumentID)
                , new System.Data.SQLite.SQLiteParameter("vol", p.Vol)
                , new System.Data.SQLite.SQLiteParameter("lastTime", p.LastTime));
        }

        //订单
        private void saveOrder(Order o) {
            string direction = o.Direction == DirectionType.Buy ? "多" : "空";
            string sql = $@"insert into [t_order]
                            ([strategy_name], [instrument_id], [direction], [price], [volume], [volume_traded], [order_time])
                            values
                            (@strategyName,@instrumentID,@direction,@price,@volume,@olumeTraded,@orderTime)";
            SQLiteHelper.ExecuteNonQuery(sql
                , new System.Data.SQLite.SQLiteParameter("strategyName", o.Strategy.GetType().Name)
                , new System.Data.SQLite.SQLiteParameter("instrumentID", o.Instrument.InstrumentID)
                , new System.Data.SQLite.SQLiteParameter("direction", direction)
                , new System.Data.SQLite.SQLiteParameter("price", o.Price)
                , new System.Data.SQLite.SQLiteParameter("volume", o.Volume)
                , new System.Data.SQLite.SQLiteParameter("olumeTraded", o.Volume - o.VolumeLeft)
                , new System.Data.SQLite.SQLiteParameter("orderTime", o.OrderTime));
        }


    }
}
