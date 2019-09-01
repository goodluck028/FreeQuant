using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;
using FreeQuant.Components;
using MySql.Data.MySqlClient;

namespace FreeQuant.DataReceiver {
    public class MySqlHelper {

        MySqlConnection mysqlConnection;
        DataSet dataSet;
        string IP = MySqlConfig.Config.IP;       
        string UserName = MySqlConfig.Config.UserName; 
        string Password = MySqlConfig.Config.Password; 
        string Database = MySqlConfig.Config.Database;
        /// <summary>
        /// 建立mysql连接
        /// </summary>
        public MySqlHelper() {
            try {
                mysqlConnection = new MySqlConnection("datasource=" + IP + ";username=" + UserName + ";password=" + Password + ";database=" + Database + ";charset=utf8");
            } catch (MySqlException ex) {
                LogUtil.Error(ex);
            }
        }

        public MySqlHelper(string IP, string UserName, string Password, string Database) {
            try {
                string connectionString = "datasource=" + IP + ";username=" + UserName + ";password=" + Password + ";database=" + Database + ";charset=gb2312";
                mysqlConnection = new MySqlConnection(connectionString);
            } catch (MySqlException ex) {
                LogUtil.Error(ex);
            }
        }

        public string MysqlInfo() {
            string mysqlInfo = null;
            try {
                mysqlConnection.Open();
                mysqlInfo += "Connection Opened." + Environment.NewLine;
                mysqlInfo += "Connection String:" + mysqlConnection.ConnectionString.ToString() + Environment.NewLine;
                mysqlInfo += "Database:" + mysqlConnection.Database.ToString() + Environment.NewLine;
                mysqlInfo += "Connection ServerVersion:" + mysqlConnection.ServerVersion.ToString() + Environment.NewLine;
                mysqlInfo += "Connection State:" + mysqlConnection.State.ToString() + Environment.NewLine;
            } catch (MySqlException ex) {
                LogUtil.Error(ex);
            } finally {
                mysqlConnection.Close();
            }
            return mysqlInfo;
        }
        /// <summary>
        /// 执行sql语句无返回结果
        /// </summary>
        public long MysqlCommand(string MysqlCommand) {
            try {
                mysqlConnection.Open();
                MySqlCommand mysqlCommand = new MySqlCommand(MysqlCommand, mysqlConnection);
                mysqlCommand.ExecuteNonQuery();
                return mysqlCommand.LastInsertedId;
            } catch (MySqlException ex) {
                LogUtil.Error(ex);
            } finally {
                mysqlConnection.Close();
            }
            return -1;
        }

        /// <summary>
        /// 执行sql语句无返回结果
        /// </summary>
        public long MysqlCommand(string MysqlCommand,params MySqlParameter[] parameters) {
            try {
                mysqlConnection.Open();
                MySqlCommand mysqlCommand = new MySqlCommand(MysqlCommand, mysqlConnection);
                mysqlCommand.ExecuteNonQuery();
                mysqlCommand.CommandType = CommandType.Text;
                mysqlCommand.Parameters.Add(parameters);
                return mysqlCommand.LastInsertedId;
            } catch (MySqlException ex) {
                LogUtil.Error(ex);
            } finally {
                mysqlConnection.Close();
            }
            return -1;
        }

        /// <summary>
        /// 执行select 语句返回执行结果
        /// </summary>
        public DataView MysqlDataAdapter(string table) {
            DataView dataView = new DataView();
            try {
                mysqlConnection.Open();
                MySqlDataAdapter mysqlDataAdapter = new MySqlDataAdapter("Select * from " + table, mysqlConnection);
                dataSet = new DataSet();
                mysqlDataAdapter.Fill(dataSet, table);
                dataView = dataSet.Tables[table].DefaultView;
            } catch (MySqlException ex) {
                LogUtil.Error(ex);
            } finally {
                mysqlConnection.Close();
            }
            return dataView;
        }
        /// <summary>
        /// 统计记录个数 参数：select count(*) from isns_users
        /// </summary>
        public long MysqlCountRow(string sql) {
            DataView dataView = new DataView();
            try {
                mysqlConnection.Open();

                MySqlCommand mycm = new MySqlCommand(sql, mysqlConnection);
                long recordCount = (long)mycm.ExecuteScalar();
                return recordCount;
            } catch (MySqlException ex) {
                LogUtil.Error(ex);
                return -1;
            } finally {
                mysqlConnection.Close();
            }
        }
    }
}
