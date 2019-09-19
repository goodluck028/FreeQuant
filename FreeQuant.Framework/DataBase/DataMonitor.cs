﻿using System;
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

namespace FreeQuant.Framework {
    [AutoCreate]
    public class DataMonitor {

        public DataMonitor() {
            EventBus.Register(this);
            LogUtil.EnginLog("数据库组件启动");
        }

        [OnLog]
        private void OnPostion(StrategyEvent.ChangePositionEvent evt) {
            Position p = evt.Position;
            DataManager.Instance.SetPosition(p.StrategyName.GetType().FullName, p.Instrument.InstrumentID, p.Vol);
        }

        [OnLog]
        private void OnOrder(Order order) {
            DataManager.Instance.SaveOrder(order);
        }
    }

    public class DataManager {

        private static DataManager mInstance = new DataManager();

        private DataManager()
        {
            //todo
            //策略表
            strategyTable = new DataTable("strategys");
            strategyTable.Columns.Add("strategy_name", Type.GetType("System.String"));
            strategyTable.Columns.Add("useable", Type.GetType("System.Boolean"));
        }

        public static DataManager Instance => mInstance;

        //
        private DataTable strategyTable;
        private Dictionary<string, DataTable> positionDic = new Dictionary<string, DataTable>();
        private Dictionary<string, DataTable> orderDic = new Dictionary<string, DataTable>();

        //
        private readonly string QUERY_POSITION = @"select [id],
                                                            [strategy_name], 
                                                            [instrument_id], 
                                                            [position], 
                                                            [last_time]
                                                            from [t_position]
                                                    where ";

        private readonly string QUERY_ORDER = @"select [id], 
                                                        [strategy_name], 
                                                        [instrument_id], 
                                                        [direction], 
                                                        [price], 
                                                        [volume], 
                                                        [volume_traded], 
                                                        [order_time]
                                                        from [t_order]";

        //
        internal DataTable StrategyTable {
            get {
                return strategyTable;
            }
        }


        //持仓
        public long GetPosition(string strategyName, string instrumentID) {
            long pos = 0;
            DataTable dt;
            if (positionDic.TryGetValue(strategyName, out dt)) {
                try {
                    pos = (long)dt.Select($"instrument_id='{instrumentID}'")[0]["position"];
                } catch (Exception e) {
                    pos = 0;
                }
            }
            return pos;
        }
        public DataTable GetPositionTable(string strategyName) {
            DataTable t;
            return positionDic.TryGetValue(strategyName, out t) ? t : new DataTable();
        }
        public void SetPosition(string strategyName, string instrumentID, long position) {
            try {
                //修改数据库
                string sql = @"replace into [t_position]
                            ([strategy_name], [instrument_id], [position], [last_time]) 
                            values
                            (@strategyName,@instrumentID,@position,@lastTime)";
                SQLiteHelper.ExecuteNonQuery(sql
                    , new System.Data.SQLite.SQLiteParameter("strategyName", strategyName)
                    , new System.Data.SQLite.SQLiteParameter("instrumentID", instrumentID)
                    , new System.Data.SQLite.SQLiteParameter("position", position)
                    , new System.Data.SQLite.SQLiteParameter("lastTime", DateTime.Now));

                //修改缓存
                DataTable t;
                if (positionDic.TryGetValue(strategyName, out t)) {
                    DataRow[] rows = t.Select($"strategy_name = '{strategyName}' and instrument_id = '{instrumentID}'");
                    if (rows.Length > 0) {
                        rows[0]["position"] = position;
                    }
                }
            } catch (Exception ex) {
                LogUtil.Error(ex);
            }
        }

        //
        public DataTable GetOrderTable(string strategyName) {
            DataTable t;
            return orderDic.TryGetValue(strategyName, out t) ? t : new DataTable();
        }
        public void SaveOrder(Order o) {
            try {
                string direction = o.Direction == DirectionType.Buy ? "多" : "空";
                string sql = $@"replace into [t_order]
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
            } catch (Exception ex) {
                LogUtil.Error(ex);
            }
        }

        //
        public void DeleteStrategy(string strategyName) {
            //删持仓
            string sql = @"delete
                            from [t_position]
                            where [strategy_name] = @strategyName";
            SQLiteHelper.ExecuteNonQuery(sql
                , new System.Data.SQLite.SQLiteParameter("strategyName", strategyName));

            //删订单
            sql = @"delete
                        from [t_order]
                        where [strategy_name] = @strategyName";
            SQLiteHelper.ExecuteNonQuery(sql
                , new System.Data.SQLite.SQLiteParameter("strategyName", strategyName));

            //删缓存
            DataRow[] rows = strategyTable.Select($"strategy_name='{strategyName}'");
            foreach (var r in rows) {
                strategyTable.Rows.Remove(r);
            }
        }
    }
}