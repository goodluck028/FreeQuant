using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    public partial class MainForm : Form {
        public MainForm() {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e) {
            DataReceiver.Instance.OnLog = printLog;
            //
            ComponentLoader.LoadAllComponents();
            ReceiverComponentsCommander.Begin();

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
    }
}
