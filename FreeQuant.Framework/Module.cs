using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace FreeQuant.Framework {
    public abstract class BaseModule {
        public abstract void Start();
    }

    public static class ModuleLoader {
        private static List<BaseModule> mModules = new List<BaseModule>();
        public static void LoadAllModules() {
            //获取文件列表 
            string[] files = new string[] { };
            try {
                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            //加载模块
            foreach (string f in files) {
                if (!f.EndsWith(".dll"))
                    continue;
                try {
                    Assembly assembly = Assembly.LoadFrom(f);
                    Type[] types = assembly.GetTypes();
                    foreach (Type t in types) {
                        if (!t.IsSubclassOf(typeof(BaseModule)))
                            continue;

                        BaseModule mdl = Activator.CreateInstance(t) as BaseModule;
                        if (mdl == null)
                            continue;

                        mModules.Add(mdl);
                        mdl.Start();
                    }
                } catch (Exception e) {
                    Console.WriteLine(e);
                }

            }
        }
    }
}
