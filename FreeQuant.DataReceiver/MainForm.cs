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
            initView();
        }

        //
        private DataTable mTable = new DataTable();
        Dictionary<string, DataRow> mRowMap = new Dictionary<string, DataRow>();
        private void initView() {
            mTable.Columns.Add("instrumentId", typeof(string));
            mTable.Columns.Add("tickSum", typeof(long));
            mTable.Columns.Add("lastPrice", typeof(double));
            mTable.Columns.Add("updateTime", typeof(string));
            //不允许排序
            for (int i = 0; i < dataGridView1.Columns.Count; i++) {
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
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

        private void printLog(string log) {
            string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + log + "\n";
            Invoke(new Action(() => {
                listBox1.Items.Add(c);
                if (listBox1.Items.Count > 256) {
                    listBox1.Items.RemoveAt(0);
                }
                listBox1.SelectedIndex = listBox1.Items.Count - 1;
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
                row["tickSum"] = 0;
                mTable.Rows.Add(row);
                mRowMap.Add(evt.Instrument.InstrumentID, row);
            })));
            delayOrderBy();
        }
        //延时排序
        private long mTimeMili;
        private Thread mDelay;
        private void delayOrderBy() {
            mTimeMili = DateTime.Now.Millisecond * DateTime.Now.Second;
            //
            if (mDelay != null && mDelay.IsAlive)
                return;
            //
            mDelay = new Thread((() => {
                long lastMili = 0;
                while (lastMili != mTimeMili) {
                    lastMili = mTimeMili;
                    Thread.Sleep(1000);
                }
                Invoke(new Action((() => {
                    mTable.DefaultView.Sort = "instrumentId ASC";
                    mTable = mTable.DefaultView.ToTable();
                    dataGridView1.DataSource = mTable;
                    //
                    mRowMap.Clear();
                    foreach (DataRow row in mTable.Rows) {
                        mRowMap.Add((string)row["instrumentId"], row);
                    }
                })));

            }));
            mDelay.Start();

        }

        //tick
        [OnEvent]
        private void OnTick(BrokerEvent.TickEvent evt) {
            Invoke(new Action((() => {
                Tick tick = evt.Tick;
                DataRow row;
                if (mRowMap.TryGetValue(tick.Instrument.InstrumentID, out row)) {
                    row["instrumentId"] = tick.Instrument.InstrumentID;
                    row["tickSum"] = (long)row["tickSum"] + 1;
                    row["lastPrice"] = tick.LastPrice;
                    row["updateTime"] = tick.UpdateTime.ToString("yyyy-MM-dd hh:mm:ss.fff");
                }
            })));
        }

        //行号
        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e) {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }
    }
}
