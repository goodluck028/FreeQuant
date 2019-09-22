using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FreeQuant.EventEngin;

namespace FreeQuant.Framework {
    public class StrategyLoader :IComponent {
        public void OnLoad() {}

        public void OnReady() {
            LoadStrategy();
        }

        private Dictionary<string, BaseStrategy> mStgMap = new Dictionary<string, BaseStrategy>();
        public void LoadStrategy() {
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
                    string[] instIds = null;
                    foreach (Attribute attr in t.GetCustomAttributes()) {
                        if (attr is InstrumentsAttribute) {
                            if (t.IsInterface || t.IsAbstract)
                                continue;
                            //
                            instIds = (attr as InstrumentsAttribute).Instruments;
                        }
                    }
                    //
                    if (!t.IsSubclassOf(typeof(BaseStrategy)))
                        continue;
                    BaseStrategy stg = Activator.CreateInstance(t) as BaseStrategy;
                    if (stg == null)
                        continue;
                    if (mStgMap.ContainsKey(t.FullName)) {
                        LogUtil.EnginLog("策略命名空间重复");
                        continue;
                    }
                    //
                    mStgMap.Add(t.FullName, stg);
                    EventBus.Register(stg);
                    stg.AddInstruments(instIds);
                    stg.Start();
                }
            }
        }
    }
}
