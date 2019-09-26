using FreeQuant.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FreeQuant.UI {
    internal class MainModel {
        //单例
        private MainModel() {
            loadTables();
        }
        private static MainModel instance = new MainModel();
        internal static MainModel Instance {
            get {
                return instance;
            }
        }

        //
        private DataTable strategyTable;
        private Dictionary<string, DataTable> positionDic = new Dictionary<string, DataTable>();
        private Dictionary<string, DataTable> orderDic = new Dictionary<string, DataTable>();

        internal DataTable StrategyTable {
            get {
                return strategyTable;
            }
        }

        //
        private void loadTables() {
            //加载当前策略
            strategyTable = new DataTable("strategys");
            strategyTable.Columns.Add("name", typeof(string));
            strategyTable.Columns.Add("enable", typeof(bool));

            //加载持仓
            DataTable positionTable = SQLiteHelper.GetDataTable(QUERY_POSITION);
            foreach (DataRow row in positionTable.Rows) {
                string strategyName = row.Field<string>("strategy_name");
                if (strategyTable.Select($"strategy_name = '{strategyName}'").Length == 0)
                    strategyTable.Rows.Add(strategyName, false);

                DataTable t;
                if (!positionDic.TryGetValue(strategyName, out t)) {
                    t = positionTable.Clone();
                    positionDic.Add(strategyName, t);
                }
                t.Rows.Add(row.ItemArray);
            }

            //加载订单
            DataTable orderTable = SQLiteHelper.GetDataTable(QUERY_ORDER);
            foreach (DataRow row in orderTable.Rows) {
                string strategyName = row.Field<string>("strategy_name");
                if (strategyTable.Select($"strategy_name = '{strategyName}'").Length == 0)
                    strategyTable.Rows.Add(strategyName, false);

                DataTable t;
                if (!orderDic.TryGetValue(strategyName, out t)) {
                    t = orderTable.Clone();
                    orderDic.Add(strategyName, t);
                }
                t.Rows.Add(row.ItemArray);
            }
        }

        //
        internal DataTable getPositionTable(string strategyName) {
            DataTable t;
            return positionDic.TryGetValue(strategyName, out t) ? t : new DataTable();
        }
        internal void setPosition(string strategyName, string instrumentID, int position) {
            try {
                //修改数据库
                string sql = @"update [t_position]
                            set [position] = @position,[last_time] = @lastTime
                            where [strategy_name] = @strategyName and [instrument_id] = @instrumentID";
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
                MessageBox.Show(ex.StackTrace);
            }
        }

        //
        internal DataTable getOrderTable(string strategyName) {
            DataTable t;
            return orderDic.TryGetValue(strategyName, out t) ? t : new DataTable();
        }

        //
        internal void deleteStrategy(string strategyName) {
            try {
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
            } catch (Exception ex) {
                MessageBox.Show(ex.StackTrace);
            }

        }

    }
}
