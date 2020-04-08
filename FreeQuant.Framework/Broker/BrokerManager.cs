using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class BrokerManager {
        private static ConcurrentDictionary<string, BaseMdBroker> MdBrokerDic = new ConcurrentDictionary<string, BaseMdBroker>();
        private static ConcurrentDictionary<string, BaseTdBroker> TdBrokerDic = new ConcurrentDictionary<string, BaseTdBroker>();

        static BrokerManager() {
            LoadBroker();
        }

        private static void LoadBroker() {
            //获取文件列表 
            string[] files = new string[] { };
            try {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "\\brokers";
                if (Directory.Exists(dir)) {
                    files = Directory.GetFiles(dir);
                } else {
                    Directory.CreateDirectory(dir);
                }
            } catch (Exception ex) {
                LogUtil.SysLog(ex.StackTrace);
            }

            //加载策略
            foreach (string f in files) {
                Assembly assembly;
                try {
                    assembly = Assembly.LoadFrom(f);
                } catch (Exception e) {
                    continue;
                }
                Type[] types = assembly.GetTypes();
                foreach (Type t in types) {
                    //
                    if (t.IsSubclassOf(typeof(BaseMdBroker))) {
                        if (MdBrokerDic.ContainsKey(t.FullName))
                            throw new Exception($"行情borker重复， {t.FullName}");
                        MdBrokerDic[t.FullName] = Activator.CreateInstance(t) as BaseMdBroker;
                    } else if (t.IsSubclassOf(typeof(BaseTdBroker))) {
                        if (TdBrokerDic.ContainsKey(t.FullName))
                            throw new Exception($"交易borker重复， {t.FullName}");
                        TdBrokerDic[t.FullName] = Activator.CreateInstance(t) as BaseTdBroker;
                    }
                }


            }
        }

        public static BaseMdBroker GetMdBroker(string className) {
            BaseMdBroker broker;
            if (MdBrokerDic.TryGetValue(className, out broker)) {
                return broker;
            } else {
                throw new Exception("broker not find");
            }
        }

        public static BaseMdBroker DefaultMdBroker => GetMdBroker(ConfigUtil.DefaultMdBroker);

        public static BaseTdBroker GetTdBroker(string className) {
            BaseTdBroker broker;
            if (TdBrokerDic.TryGetValue(className, out broker)) {
                return broker;
            } else {
                throw new Exception("broker not find");
            }
        }

        public static BaseTdBroker DefaultTdBroker => GetTdBroker(ConfigUtil.DefaultTdBroker);
    }
}
