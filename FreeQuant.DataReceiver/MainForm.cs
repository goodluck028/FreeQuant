using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using FreeQuant.Components;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            EventBus.Register(this);
            //
            LogUtil.Logger.Record();
            //
            ComponentLoader.LoadAllComponents();
            ComponentsSchelduler.Begin();
            //
            initGrid();
        }

        //
        private DataTable mTable = new DataTable();
        Dictionary<string, DataRow> mRowMap = new Dictionary<string, DataRow>();
        private void initGrid() {
            mTable.Columns.Add("instrumentId", typeof(string));
            mTable.Columns.Add("tickSum", typeof(long));
            mTable.Columns.Add("lastPrice", typeof(double));
            mTable.Columns.Add("updateTime", typeof(DateTime));
            dataGridView1.DataSource = mTable;
        }

        //输出日志
        [OnLog]
        private void OnEnginLog(LogEvent.EnginLog evt) {
            printLog(evt.Content);
        }
        [OnLog]
        private void OnUserLog(LogEvent.UserLog evt) {
            printLog(evt.Content);
        }
        [OnLog]
        private void OnException(Exception ex) {
            printLog(ex.Message);
        }
        private int line = 0;
        private void printLog(string log) {
            if (line++ > 256) {
                Invoke(new Action(() => {
                    textBox1.Clear();
                }));
                line = 0;
            }
            //
            string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + log + "\n";

            Invoke(new Action(() => {
                textBox1.AppendText(c);
                textBox1.ScrollToCaret();
            }));
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            ComponentsSchelduler.Stop();
        }

        //instrument
        [OnEvent]
        private void OnInstrument(BrokerEvent.InstrumentEvent evt) {
            Invoke(new Action((() => {
                DataRow row;
                row = mTable.NewRow();
                row["instrumentId"] = evt.Instrument.InstrumentID;
                mTable.Rows.Add(row);
                mTable.DefaultView.Sort = "instrumentId ASC";
                mTable = mTable.DefaultView.ToTable();
                dataGridView1.DataSource = mTable;
            })));
        }

        //tick
        [OnEvent]
        private void OnTick(BrokerEvent.TickEvent evt) {
            Invoke(new Action((() => {
                Tick tick = evt.Tick;
                DataRow row;
                if (mRowMap.TryGetValue(tick.Instrument.InstrumentID, out row)) {
                    row["instrumentId"] = tick.Instrument.InstrumentID;
                    row["tickSum"] = (int)row["tickSum"] + 1;
                    row["lastPrice"] = tick.LastPrice;
                    row["updateTime"] = tick.UpdateTime;
                }
            })));

        }

        //行号
        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e) {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }
    }
}
