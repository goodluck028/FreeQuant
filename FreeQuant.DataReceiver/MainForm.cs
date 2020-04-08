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

        private void safeInvoke(Action act) {
            if (InvokeRequired) {
                BeginInvoke(act);
            }
        }

        private void MainForm_Shown(object sender, EventArgs e) {
            initView();
            start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            DialogResult result = MessageBox.Show("确认退出吗?", "操作提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (result == DialogResult.OK) {
                DataReceiver.Instance.OnInstrument -= _onInstrument;
                DataReceiver.Instance.OnTick -= _onTick;
                //
                Dispose();
                Application.Exit();
            } else {
                e.Cancel = true;
            }
        }

        //
        Dictionary<string, ListViewItem> mRowMap = new Dictionary<string, ListViewItem>();
        private void initView() {
            LogUtil.OnLog += printLog;
        }
        //
        private void start() {
            DataReceiver.Instance.OnInstrument += _onInstrument;
            DataReceiver.Instance.OnTick += _onTick;
            DataReceiver.Instance.run();
        }

        private void printLog(string log) {
            string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + log + "\r\n\r\n";
            safeInvoke(() => {
                textBox1.AppendText(c);
                if (textBox1.TextLength > 1000 * 1000) {
                    textBox1.Clear();
                }
            });
        }

        //instrument
        private void _onInstrument(Instrument inst) {
            if (mRowMap.ContainsKey(inst.InstrumentID))
                return;
            //
            safeInvoke(() => {
                ListViewItem li = new ListViewItem();
                li.Text = inst.InstrumentID;
                li.SubItems.Add("0");
                li.SubItems.Add("");
                li.SubItems.Add("");
                listView1.Items.Add(li);
                mRowMap.Add(inst.InstrumentID, li);
            });
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
        private Thread mOrderThread;
        private void delayOrderBy() {
            if (mOrderThread != null)
                return;
            mOrderThread = new Thread(() => {
                Thread.Sleep(5 * 1000);
                safeInvoke(() => {
                    listView1.ListViewItemSorter = new Sorter();
                    listView1.Sort();
                    mOrderThread = null;
                });
            });
            mOrderThread.Start();
        }

        //tick
        private void _onTick(Tick tick) {
            safeInvoke(() => {
                ListViewItem li;
                if (mRowMap.TryGetValue(tick.Instrument.InstrumentID, out li)) {
                    li.SubItems[1].Tag = li.SubItems[1].Tag == null ? 1 : (long)(li.SubItems[1].Tag) + 1;
                    li.SubItems[1].Text = li.SubItems[1].Tag.ToString();
                    li.SubItems[2].Text = tick.LastPrice.ToString();
                    li.SubItems[3].Text = tick.UpdateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
            });
        }

        //行号
        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e) {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }
    }
}
