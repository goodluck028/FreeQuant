using System;
using System.IO;

namespace FreeQuant.Framework {
    /// <summary>
    /// 日志工具
    /// </summary>
    public static class LogUtil {
        public static Action<string> OnLog;
        public static void Log(string folder, string content) {
            string log = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + content;
            OnLog?.Invoke(log);
            //
            string path = AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrEmpty(path)) {
                path = AppDomain.CurrentDomain.BaseDirectory + "\\log\\" + folder;
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                path = path + "\\" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(path)) {
                    FileStream fs = File.Create(path);
                    fs.Close();
                }
                if (File.Exists(path)) {
                    StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default);
                    sw.WriteLine(log);
                    sw.Close();
                }
            }
        }

    }
}
