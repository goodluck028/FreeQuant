using System;
using System.IO;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    /// <summary>
    /// 日志工具
    /// </summary>
    public static class LogUtil {
        private static Action<string> mOnLog;
        public static event Action<string> OnLog {
            add {
                mOnLog -= value;
                mOnLog += value;
            }
            remove {
                mOnLog -= value;
            }
        }
        public static object locker = new object();
        //
        public static void Log(string folder, string content) {
            Task.Run(() => {
                string log = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + content;
                mOnLog?.Invoke(log);
                //
                lock (locker) {
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
            });
        }
        public static void ErrLog(string content) {
            Log("error", content);
        }
        public static void SysLog(string content) {
            Log("sys", content);
        }
        public static void UserLog(string content) {
            Log("user", content);
        }

    }
}
