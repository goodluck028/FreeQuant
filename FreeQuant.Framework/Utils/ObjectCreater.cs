using System;
using System.Collections;
using System.IO;
using System.Reflection;


namespace FreeQuant.Framework {
    public static class ObjectCreater {
        private static ArrayList mList = new ArrayList();
        public static void LoadAndCreate() {
            //获取文件列表 
            string[] files = new string[] { };
            try {
                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            //加载模块
            foreach (string f in files) {
                if (!f.EndsWith(".dll") && !f.EndsWith(".exe"))
                    continue;
                try {
                    Assembly assembly = Assembly.LoadFrom(f);
                    Type[] types = assembly.GetTypes();
                    foreach (Type t in types) {
                        foreach (Attribute attr in t.GetCustomAttributes()) {
                            if (attr is AutoCreateAttribute) {
                                if (t.IsInterface || t.IsAbstract)
                                    continue;
                                //
                                object obj = Activator.CreateInstance(t);
                                if (obj == null)
                                    continue;
                                //
                                mList.Add(obj);
                            }
                        }
                    }
                } catch (Exception e) {
                }

            }
        }
    }
}
