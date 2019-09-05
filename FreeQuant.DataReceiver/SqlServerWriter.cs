using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Components;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace FreeQuant.DataReceiver {
    internal class SqlServerWriter : IDataWriter {

        public void CreateTable(string dbName) {
            SqlConnection conn = new SqlConnection(DataBaseConfig.Config.Server);
            try {
                conn.Open();
                //
                string sql = $@"exec [dbo].[p_create_table] {dbName}";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            } catch (Exception e) {
                LogUtil.Error(e);
            } finally {
                conn.Close();
            }
        }

        public void InsertBar(Bar bar) {
            SqlConnection conn = new SqlConnection(DataBaseConfig.Config.Server);
            string tbName = RegexUtils.TakeProductName(bar.Instrument.InstrumentID);
            try {
                conn.Open();
                //
                string sql = $@"INSERT INTO [hisdata_future].[dbo].[t_bar1min_{tbName}]
                           ([exchange_id]
                           ,[instrument_id]
                           ,[multiplier]
                           ,[begin_time]
                           ,[open_price]
                           ,[high_price]
                           ,[low_price]
                           ,[close_price]
                           ,[volume]
                           ,[open_interest])
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
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@exchange_id", bar.Instrument.ExchangeID);
                cmd.Parameters.AddWithValue("@instrument_id", bar.Instrument.InstrumentID);
                cmd.Parameters.AddWithValue("@multiplier", bar.Instrument.VolumeMultiple);
                cmd.Parameters.AddWithValue("@begin_time", bar.BeginTime);
                cmd.Parameters.AddWithValue("@open_price", bar.OpenPrice);
                cmd.Parameters.AddWithValue("@high_price", bar.HighPrice);
                cmd.Parameters.AddWithValue("@low_price", bar.LowPrice);
                cmd.Parameters.AddWithValue("@close_price", bar.ClosePrice);
                cmd.Parameters.AddWithValue("@volume", bar.Volume);
                cmd.Parameters.AddWithValue("@open_interest", bar.OpenInterest);
                cmd.ExecuteNonQuery();
            } catch (Exception e) {
                LogUtil.Error(e);
            } finally {
                conn.Close();
            }
        }

        public void InsertTicks(List<Tick> ticks) {
            if (ticks.Count == 0)
                return;
            //
            DataTable dt = new DataTable();
            dt.Columns.Add("exchange_id", typeof(string));
            dt.Columns.Add("instrument_id", typeof(string));
            dt.Columns.Add("multiplier", typeof(double));
            dt.Columns.Add("ask_price", typeof(double));
            dt.Columns.Add("ask_vol", typeof(long));
            dt.Columns.Add("bid_price", typeof(double));
            dt.Columns.Add("bid_vol", typeof(long));
            dt.Columns.Add("last_price", typeof(double));
            dt.Columns.Add("volume", typeof(double));
            dt.Columns.Add("open_interest", typeof(double));
            dt.Columns.Add("uper_limit", typeof(double));
            dt.Columns.Add("lower_limit", typeof(double));
            dt.Columns.Add("update_time", typeof(DateTime));
            foreach (Tick tcik in ticks) {
                DataRow dr = dt.NewRow();
                dr["exchange_id"] = tcik.Instrument.ExchangeID;
                dr["instrument_id"] = tcik.Instrument.InstrumentID;
                dr["multiplier"] = tcik.Instrument.VolumeMultiple;
                dr["ask_price"] = tcik.AskPrice;
                dr["ask_vol"] = tcik.AskVolume;
                dr["bid_price"] = tcik.BidPrice;
                dr["bid_vol"] = tcik.BidVolume;
                dr["last_price"] = tcik.LastPrice;
                dr["volume"] = tcik.Volume;
                dr["open_interest"] = tcik.OpenInterest;
                dr["uper_limit"] = tcik.UpperLimitPrice;
                dr["lower_limit"] = tcik.LowerLimitPrice;
                dr["update_time"] = tcik.UpdateTime;
                dt.Rows.Add(dr);
            }
            //
            SqlConnection conn = new SqlConnection(DataBaseConfig.Config.Server);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
            string tbName = RegexUtils.TakeProductName(ticks[0].Instrument.InstrumentID);
            bulkCopy.DestinationTableName = $"[hisdata_future].[dbo].[t_tick_{tbName}]";
            bulkCopy.BatchSize = ticks.Count;
            try {
                conn.Open();
                bulkCopy.WriteToServer(dt);
            } catch (Exception e) {
                LogUtil.Error(e);
            } finally {
                conn.Close();
            }
        }
    }
}
