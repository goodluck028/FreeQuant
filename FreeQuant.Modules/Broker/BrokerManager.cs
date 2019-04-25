using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace FreeQuant.Modules {
    internal class BrokerManager {
        private static BrokerManager mIntance = new BrokerManager();
        private BrokerManager() {

        }

        public static BrokerManager Intance => mIntance;

        private static Dictionary<string, IMdProvider> mMdMap = new Dictionary<string, IMdProvider>();
        private static Dictionary<string, ITdProvider> mTdMap = new Dictionary<string, ITdProvider>();

        private void loadProvider() {
            //获取文件列表 
            string[] files = new string[] { };
            try {
                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "\\providers");
            } catch (Exception ex) {
                FqLog.EnginLog(ex.StackTrace);
            }

            //加载策略
            foreach (string f in files) {
                Assembly assembly = Assembly.LoadFrom(f);
                Type[] types = assembly.GetTypes();
                foreach (Type t in types) {
                    if (t.IsAssignableFrom(typeof(IMdProvider))) {
                        IMdProvider md = Activator.CreateInstance(t) as IMdProvider;
                        mMdMap.Add(t.Name, md);
                    }

                    if (t.IsAssignableFrom(typeof(ITdProvider))) {
                        ITdProvider td = Activator.CreateInstance(t) as ITdProvider;
                        mTdMap.Add(t.Name, td);
                    }
                }
            }
        }

        private void logInMd(string providerName) {
            IMdProvider md;
            if (mMdMap.TryGetValue(providerName, out md)) {
                md.Login();
            }
        }

        private void loginTd(string providerName) {
            ITdProvider td;
            if (mTdMap.TryGetValue(providerName, out td)) {
                td.Login();
            }
        }
    }
}
