using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;
using System.Collections;
using MySql.Data.MySqlClient;
using System.Data;

namespace FreeQuant.DataReceiver
{

    public class ConnectionPool
    {
        private static ConnectionPool cpool = null;//池管理对象
        private static object objlock = typeof(ConnectionPool);//池管理对象实例
        private int size = 1;//池中连接数
        private int useCount = 0;//已经使用的连接数
        private ArrayList pool = null;//连接保存的集合
        private string ConnectionStr = Config.Server;//连接字符串

        public ConnectionPool()
        {
            //创建可用连接的集合
            pool = new ArrayList();
        }

        #region 创建获取连接池对象
        public static ConnectionPool getPool()
        {
            lock (objlock)
            {
                if (cpool == null)
                {
                    cpool = new ConnectionPool();
                }
                return cpool;
            }
        }
        #endregion

        #region 获取池中的连接
        public MySqlConnection getConnection()
        {
            lock (pool)
            {
                MySqlConnection tmp = null;
                //可用连接数量大于0
                if (pool.Count > 0)
                {
                    //取第一个可用连接
                    tmp = (MySqlConnection)pool[0];
                    //在可用连接中移除此链接
                    pool.RemoveAt(0);
                    //不成功
                    if (!isUserful(tmp))
                    {
                        //可用的连接数据已去掉一个
                        useCount--;
                        tmp = getConnection();
                    }
                }
                else
                {
                    //可使用的连接小于连接数量
                    if (useCount <= size)
                    {
                        try
                        {
                            //创建连接
                            tmp = CreateConnection(tmp);
                        }
                        catch (Exception e)
                        {
                        }
                    }
                }
                //连接为null
                if (tmp == null)
                {
                    //达到最大连接数递归调用获取连接否则创建新连接
                    if (useCount <= size)
                    {
                        tmp = getConnection();
                    }
                    else
                    {
                        tmp = CreateConnection(tmp);
                    }
                }
                return tmp;
            }
        }
        #endregion

        #region 创建连接
        private MySqlConnection CreateConnection(MySqlConnection tmp)
        {
            //创建连接
            MySqlConnection conn = new MySqlConnection(ConnectionStr);
            conn.Open();
            //可用的连接数加上一个
            useCount++;
            tmp = conn;
            return tmp;
        }
        #endregion

        #region 关闭连接,加连接回到池中
        public void closeConnection(MySqlConnection con)
        {
            lock (pool)
            {
                if (con != null)
                {
                    //将连接添加在连接池中
                    pool.Add(con);
                }
            }
        }
        #endregion

        #region 目的保证所创连接成功,测试池中连接
        private bool isUserful(MySqlConnection con)
        {
            //主要用于不同用户
            bool result = true;
            if (con != null)
            {
                string sql = "select 1";//随便执行对数据库操作
                MySqlCommand cmd = new MySqlCommand(sql, con);
                try
                {
                    cmd.ExecuteScalar().ToString();
                }
                catch
                {
                    result = false;
                }

            }
            return result;
        }
        #endregion
    }

    /// <summary>
    /// Mysql数据库
    /// </summary>
    internal class MySqlWriter : IDataWriter
    {

        //创建数据库
        public void CreateDb()
        {
            MySqlConnection conn = ConnectionPool.getPool().getConnection();
            try
            {
                string sql = "CREATE SCHEMA If Not Exists `hisdata_future` ;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                LogUtil.ErrLog(e.ToString());
            }
        }

        //创建表
        public void CreateTable(string product)
        {
            MySqlConnection conn = ConnectionPool.getPool().getConnection();
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                //创建一分钟bar表
                string sql = $@"CREATE TABLE If Not Exists `hisdata_future`.`t_bar1min_{product}` (
                                  `exchange_id` varchar(16) NOT NULL,
                                  `instrument_id` varchar(16) NOT NULL,
                                  `multiplier` float NOT NULL,
                                  `begin_time` datetime NOT NULL,
                                  `open_price` float NOT NULL,
                                  `high_price` float NOT NULL,
                                  `low_price` float NOT NULL,
                                  `close_price` float NOT NULL,
                                  `volume` float NOT NULL,
                                  `open_interest` float NOT NULL,
                                  INDEX `ix_time` (`begin_time` ASC)
                                ) ENGINE=MyISAM";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
                //创建tick表
                sql = $@"CREATE  TABLE If Not Exists `hisdata_future`.`t_tick_{product}` (
                          `exchange_id` VARCHAR(16) NOT NULL ,
                          `instrument_id` VARCHAR(16) NOT NULL ,
                          `multiplier` FLOAT NOT NULL ,
                          `ask_price` FLOAT NOT NULL ,
                          `ask_vol` FLOAT NOT NULL ,
                          `bid_price` FLOAT NOT NULL ,
                          `bid_vol` FLOAT NOT NULL ,
                          `last_price` FLOAT NOT NULL ,
                          `volume` FLOAT NOT NULL ,
                          `open_interest` FLOAT NOT NULL ,
                          `uper_limit` FLOAT NOT NULL ,
                          `lower_limit` FLOAT NOT NULL ,
                          `update_time` DATETIME NOT NULL ,
                          INDEX `ix_time` (`update_time` ASC) )
                        ENGINE = MyISAM;";
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                LogUtil.ErrLog(e.ToString());
            }
        }

        //写入bar
        public void InsertBar(Bar bar)
        {
            MySqlConnection conn = ConnectionPool.getPool().getConnection();
            string product = bar.Instrument.ProductID;
            try
            {
                string sql = $@"INSERT INTO `hisdata_future`.`t_bar1min_{product}`
                                   (`exchange_id`
                                   ,`instrument_id`
                                   ,`multiplier`
                                   ,`begin_time`
                                   ,`open_price`
                                   ,`high_price`
                                   ,`low_price`
                                   ,`close_price`
                                   ,`volume`
                                   ,`open_interest`)
                             VALUES
                                   (@exchange_id
                                   ,@instrument_id
                                   ,@multiplier
                                   ,@begin_time
                                   ,@open_price
                                   ,@high_price
                                   ,@low_price
                                   ,@close_price
                                   ,@volume
                                   ,@open_interest)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                //
                cmd.Parameters.AddWithValue("@exchange_id", bar.Instrument.Exchange.ToString());
                cmd.Parameters.AddWithValue("@instrument_id", bar.Instrument.InstrumentID);
                cmd.Parameters.AddWithValue("@multiplier", bar.Instrument.VolumeMultiple);
                cmd.Parameters.AddWithValue("@begin_time", bar.BeginTime);
                cmd.Parameters.AddWithValue("@open_price", bar.OpenPrice);
                cmd.Parameters.AddWithValue("@high_price", bar.HighPrice);
                cmd.Parameters.AddWithValue("@low_price", bar.LowPrice);
                cmd.Parameters.AddWithValue("@close_price", bar.ClosePrice);
                cmd.Parameters.AddWithValue("@volume", bar.Volume);
                cmd.Parameters.AddWithValue("@open_interest", bar.OpenInterest);
                //
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                LogUtil.ErrLog(e.ToString());
            }
        }

        //写入tick
        public void InsertTick(Tick tick)
        {
            MySqlConnection conn = ConnectionPool.getPool().getConnection();
            string product = tick.Instrument.ProductID;
            try
            {
                string sql = $@"INSERT INTO `hisdata_future`.`t_tick_{product}`
                                    (`exchange_id`,
                                    `instrument_id`,
                                    `multiplier`,
                                    `ask_price`,
                                    `ask_vol`,
                                    `bid_price`,
                                    `bid_vol`,
                                    `last_price`,
                                    `volume`,
                                    `open_interest`,
                                    `uper_limit`,
                                    `lower_limit`,
                                    `update_time`)
                                VALUES
                                    (@exchange_id,
                                    @instrument_id,
                                    @multiplier,
                                    @ask_price,
                                    @ask_vol,
                                    @bid_price,
                                    @bid_vol,
                                    @last_price,
                                    @volume,
                                    @open_interest,
                                    @uper_limit,
                                    @lower_limit,
                                    @update_time);";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                //
                cmd.Parameters.AddWithValue("@exchange_id", tick.Instrument.Exchange.ToString());
                cmd.Parameters.AddWithValue("@instrument_id", tick.Instrument.InstrumentID);
                cmd.Parameters.AddWithValue("@multiplier", tick.Instrument.VolumeMultiple);
                cmd.Parameters.AddWithValue("@ask_price", tick.AskPrice);
                cmd.Parameters.AddWithValue("@ask_vol", tick.AskVolume);
                cmd.Parameters.AddWithValue("@bid_price", tick.BidPrice);
                cmd.Parameters.AddWithValue("@bid_vol", tick.BidVolume);
                cmd.Parameters.AddWithValue("@last_price", tick.LastPrice);
                cmd.Parameters.AddWithValue("@volume", tick.Volume);
                cmd.Parameters.AddWithValue("@open_interest", tick.OpenInterest);
                cmd.Parameters.AddWithValue("@uper_limit", tick.UpperLimitPrice);
                cmd.Parameters.AddWithValue("@lower_limit", tick.LowerLimitPrice);
                cmd.Parameters.AddWithValue("@update_time", tick.UpdateTime);
                //
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                LogUtil.ErrLog(e.ToString());
            }
        }

        //写入ticks
        public void InsertTicks(List<Tick> ticks)
        {
            if (ticks.Count == 0)
                return;
            //
            MySqlConnection conn = ConnectionPool.getPool().getConnection();
            string product = ticks[0].Instrument.ProductID;
            try
            {
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn;
                //通过事务加速插入操作
                MySqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                foreach (Tick tick in ticks)
                {
                    cmd.CommandText = $@"INSERT INTO `hisdata_future`.`t_tick_{product}`
                                    (`exchange_id`,
                                    `instrument_id`,
                                    `multiplier`,
                                    `ask_price`,
                                    `ask_vol`,
                                    `bid_price`,
                                    `bid_vol`,
                                    `last_price`,
                                    `volume`,
                                    `open_interest`,
                                    `uper_limit`,
                                    `lower_limit`,
                                    `update_time`)
                                VALUES
                                    (@exchange_id,
                                    @instrument_id,
                                    @multiplier,
                                    @ask_price,
                                    @ask_vol,
                                    @bid_price,
                                    @bid_vol,
                                    @last_price,
                                    @volume,
                                    @open_interest,
                                    @uper_limit,
                                    @lower_limit,
                                    @update_time);";
                    //
                    cmd.Parameters.Clear();
                    //
                    cmd.Parameters.AddWithValue("@exchange_id", tick.Instrument.Exchange.ToString());
                    cmd.Parameters.AddWithValue("@instrument_id", tick.Instrument.InstrumentID);
                    cmd.Parameters.AddWithValue("@multiplier", tick.Instrument.VolumeMultiple);
                    cmd.Parameters.AddWithValue("@ask_price", tick.AskPrice);
                    cmd.Parameters.AddWithValue("@ask_vol", tick.AskVolume);
                    cmd.Parameters.AddWithValue("@bid_price", tick.BidPrice);
                    cmd.Parameters.AddWithValue("@bid_vol", tick.BidVolume);
                    cmd.Parameters.AddWithValue("@last_price", tick.LastPrice);
                    cmd.Parameters.AddWithValue("@volume", tick.Volume);
                    cmd.Parameters.AddWithValue("@open_interest", tick.OpenInterest);
                    cmd.Parameters.AddWithValue("@uper_limit", tick.UpperLimitPrice);
                    cmd.Parameters.AddWithValue("@lower_limit", tick.LowerLimitPrice);
                    cmd.Parameters.AddWithValue("@update_time", tick.UpdateTime);
                    //
                    cmd.ExecuteNonQuery();
                }
                //提交事务
                tx.Commit();
            }
            catch (Exception e)
            {
                LogUtil.ErrLog(e.ToString());
            }
        }
    }
}
