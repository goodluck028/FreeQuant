using System;
using System.Collections;
using System.Threading;
using System.Windows.Forms;
using FreeQuant.Components;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    public partial class MainForm : Form
    {
        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            //
            LogUtil.Open();
            //
            DataReceiver.Instance.OnLog = printLog;
            //
            ComponentLoader.LoadAllComponents();
            ComponentsSchelduler.Begin();
            //
            initGrid();

        }

        private void initGrid() {
        }

        //输出日志
        private int line = 0;
        private void printLog(string text) {
            if (line++ > 256) {
                textBox1.Clear();
                line = 0;
            }
            //
            string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + text + "\n";
            Action act = () => {
                textBox1.AppendText(c);
                textBox1.ScrollToCaret();
            };
            Invoke(act);
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e) {
            ComponentsSchelduler.Stop();
        }
    }
}
