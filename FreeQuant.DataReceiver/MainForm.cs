using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e) {
            initView();
        }

        //
        private DataTable mTable = new DataTable();
        Dictionary<string, DataRow> mRowMap = new Dictionary<string, DataRow>();
        private void initView() {
            LogUtil.OnLog += printLog;
            //
            mTable.Columns.Add("instrumentId", typeof(string));
            mTable.Columns.Add("tickSum", typeof(long));
            mTable.Columns.Add("lastPrice", typeof(double));
            mTable.Columns.Add("updateTime", typeof(string));
        }

        private void printLog(string log) {
            string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + log + "r\n\r\n";
            Invoke(new Action(() => {
                textBox1.AppendText(c);
                if(textBox1.TextLength > 1000 * 10) {
                    textBox1.Clear();
                }
            }));
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            ComponentsSchelduler.Instance.Stop();
        }

        //instrument
        private void _onInstrument(Instrument inst) {
            if(mRowMap.ContainsKey(inst.InstrumentID))
                return;
            //
            DataRow row;
            row = mTable.NewRow();
            row["instrumentId"] = inst.InstrumentID;
            row["tickSum"] = 0;
            mTable.Rows.Add(row);
            mRowMap.Add(inst.InstrumentID, row);
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
                //重排序
                mTable.DefaultView.Sort = "instrumentId ASC";
                mTable = mTable.DefaultView.ToTable();
                //从新映射row
                mRowMap.Clear();
                foreach (DataRow row in mTable.Rows) {
                    mRowMap.Add((string)row["instrumentId"], row);
                }
                //更新界面
                Invoke(new Action((() => {
                    dataGridView1.DataSource = mTable;
                    //
                    dataGridView1.Columns["instrumentId"].Width = 96;
                    dataGridView1.Columns["instrumentId"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["tickSum"].Width = 72;
                    dataGridView1.Columns["tickSum"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["lastPrice"].Width = 80;
                    dataGridView1.Columns["lastPrice"].SortMode = DataGridViewColumnSortMode.NotSortable;
                    dataGridView1.Columns["updateTime"].Width = 160;
                    dataGridView1.Columns["updateTime"].SortMode = DataGridViewColumnSortMode.NotSortable;
                })));
            }));
            mDelay.Start();

        }

        //tick
        private void _onTick(Tick tick) {
            DataRow row;
            if (mRowMap.TryGetValue(tick.Instrument.InstrumentID, out row)) {
                row["instrumentId"] = tick.Instrument.InstrumentID;
                row["tickSum"] = (long)row["tickSum"] + 1;
                row["lastPrice"] = tick.LastPrice;
                row["updateTime"] = tick.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
        }

        //行号
        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e) {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }

        //
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            DialogResult result = MessageBox.Show("确认退出吗?", "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK) {
                Dispose();
                Application.Exit();
            } else {
                e.Cancel = true;
            }
        }
    }
}
