using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FreeQuant.Framework;
using System.Threading;

namespace FreeQuant.DataReceiver {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main() {
            //全局异常
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        //非主线程异常
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
            Exception ex = e.ExceptionObject as Exception;
            LogUtil.ErrLog(ex.ToString());
            restart();
        }

        //主线程异常
        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e) {
            Exception ex = e.Exception;
            LogUtil.ErrLog(ex.ToString());
        }

        static void restart() {
            Thread.Sleep(10 * 1000);
            System.Diagnostics.ProcessStartInfo cp = new System.Diagnostics.ProcessStartInfo();
            cp.FileName = Application.ExecutablePath;
            cp.Arguments = "cmd params";
            cp.UseShellExecute = true;
            System.Diagnostics.Process.Start(cp);
        }
    }
}
