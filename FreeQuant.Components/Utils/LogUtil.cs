using System;
using System.IO;
using FreeQuant.Framework;

namespace FreeQuant.Components {
    public class LogUtil {
        private static LogUtil mInstance = new LogUtil();
        public static LogUtil Logger => mInstance;

        //发送异步日志
        public static void EnginLog(string content) {
            LogEvent.EnginLog evt = new LogEvent.EnginLog(content);
            EventBus.PostLog(evt);
        }
        public static void UserLog(string content) {
            LogEvent.UserLog evt = new LogEvent.UserLog(content);
            EventBus.PostLog(evt);
        }
        public static void Error(Exception ex) {
            EventBus.PostLog(ex);
        }

        //写日志
        public void Output()
        {
            EventBus.Register(this);
            EnginLog(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>日志仅输出<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
        }

        private bool mIsRecord = false;
        public void Record() {
            mIsRecord = true;
            EventBus.Register(this);
            EnginLog(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>日志记录<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
        }

        //系统日志
        [OnLog]
        private void OnEnginLog(LogEvent.EnginLog evt) {
            PrintLog("EnginLog", evt.Content);
        }

        //用户日志
        [OnLog]
        private void OnUserLog(LogEvent.UserLog evt) {
            PrintLog("UserLog", evt.Content);
        }

        //异常
        [OnLog]
        private void OnException(Exception ex) {
            EnginLog(ex.Message);
            PrintLog("error", ex.Message);
        }

        //
        private void PrintLog(string fileName, string content) {
            string log = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "-->" + content;
            Console.WriteLine(log);
            if (!mIsRecord)
                return;
            //
            string path = AppDomain.CurrentDomain.BaseDirectory;
            string folderName = fileName;
            if (!string.IsNullOrEmpty(path)) {
                path = AppDomain.CurrentDomain.BaseDirectory + "\\log\\" + folderName;
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
