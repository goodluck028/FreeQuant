using System;
using System.Collections;
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

        private void initGrid() {
        }

        //输出日志
        private int line = 0;
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
            if (line++ > 256) {
                textBox1.Clear();
                line = 0;
            }
            //
            string c = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + log + "\n";
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
