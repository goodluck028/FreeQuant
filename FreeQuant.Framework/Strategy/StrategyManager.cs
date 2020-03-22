using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FreeQuant.Framework {
    public static class StrategyLoader {
        static StrategyLoader() {
            LoadStrategy();
        }

        private static Dictionary<string, BaseStrategy> sStgDic = new Dictionary<string, BaseStrategy>();
        public static void LoadStrategy() {
            //获取文件列表 
            string[] files = new string[] { };
            try {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "\\strategys";
                if (Directory.Exists(dir)) {
                    files = Directory.GetFiles(dir);
                } else {
                    Directory.CreateDirectory(dir);
                }
            } catch (Exception ex) {
                LogUtil.EnginLog(ex.StackTrace);
            }

            //加载策略
            foreach (string f in files) {
                Assembly assembly = Assembly.LoadFrom(f);
                Type[] types = assembly.GetTypes();
                foreach (Type t in types) {
                    //
                    if (!t.IsSubclassOf(typeof(BaseStrategy)))
                        continue;
                    BaseStrategy stg = Activator.CreateInstance(t) as BaseStrategy;
                    if (stg == null)
                        continue;
                    if (sStgDic.ContainsKey(t.FullName)) {
                        LogUtil.EnginLog("策略命名空间重复");
                        continue;
                    }
                    //
                    sStgDic.Add(t.FullName, stg);
                    stg.Start();
                }
            }
        }
    }
}
