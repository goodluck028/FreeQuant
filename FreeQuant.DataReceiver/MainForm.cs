using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using FreeQuant.Framework;
using System.Threading.Tasks;
using System.Collections;

namespace FreeQuant.DataReceiver {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e) {
            initView();
        }

        //
        Dictionary<string, ListViewItem> mRowMap = new Dictionary<string, ListViewItem>();
        private void initView() {
            LogUtil.OnLog += printLog;
        }

        private void printLog(string log) {
            string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + log + "r\n\r\n";
            Invoke(new Action(() => {
                textBox1.AppendText(c);
                if (textBox1.TextLength > 1000 * 10) {
                    textBox1.Clear();
                }
            }));
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            ComponentsSchelduler.Instance.Stop();
        }

        //instrument
        private void _onInstrument(Instrument inst) {
            if (mRowMap.ContainsKey(inst.InstrumentID))
                return;
            //
            ListViewItem li = new ListViewItem();
            li.SubItems.Add(inst.InstrumentID);
            li.SubItems.Add("0");
            listView1.Items.Add(li);
            mRowMap.Add(inst.InstrumentID, li);
            delayOrderBy();
        }
        //延时排序
        private class Sorter : IComparer {
            public int Compare(object x, object y) {
                int returnVal = -1;
                returnVal = string.Compare(((ListViewItem)x).SubItems[0].Text,
                ((ListViewItem)y).SubItems[0].Text);
                return returnVal;
            }
        }
        private DateTime mLastTime;
        private void delayOrderBy() {
            //小于10秒就返回
            if (mLastTime == null) {
                mLastTime = DateTime.Now.AddSeconds(10);
                Task.Run(() => {
                    while (true) {
                        Thread.Sleep(1000);
                        lock (this) {
                            if (DateTime.Now.CompareTo(mLastTime) < 0) {
                                continue;
                            }
                        }
                        //排序
                        listView1.ListViewItemSorter = new Sorter();
                        listView1.Sort();
                        break;
                    }
                });
            } else {
                lock (this) {
                    mLastTime = DateTime.Now.AddSeconds(10);
                }
            }
        }

        //tick
        private void _onTick(Tick tick) {
            ListViewItem li;
            if (mRowMap.TryGetValue(tick.Instrument.InstrumentID, out li)) {
                li.SubItems[0].Text = tick.Instrument.InstrumentID;
                li.SubItems[1].Tag = (long)(li.SubItems[1].Tag) + 1;
                li.SubItems[1].Text = li.SubItems[1].Tag.ToString();
                li.SubItems[2].Text = tick.LastPrice.ToString();
                li.SubItems[3].Text = tick.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
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
