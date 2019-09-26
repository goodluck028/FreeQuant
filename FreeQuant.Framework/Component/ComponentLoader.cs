using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;


namespace FreeQuant.Framework {


    public static class ComponentLoader {
        private static List<IComponent> mList = new List<IComponent>();

        public static void LoadAndCreate() {
            //获取文件列表 
            string[] files = new string[] { };
            try {
                files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory);
            } catch (Exception e) {
                Console.WriteLine(e);
            }

            //加载组件
            foreach (string f in files) {
                if (!f.EndsWith(".dll") && !f.EndsWith(".exe"))
                    continue;
                try {
                    Assembly assembly = Assembly.LoadFrom(f);
                    Type[] types = assembly.GetTypes();
                    foreach (Type t in types) {
                        if (t.GetInterface("IComponent") != null) {
                            if (t.IsInterface || t.IsAbstract)
                                continue;
                            //
                            IComponent component = Activator.CreateInstance(t) as IComponent;
                            if (component == null)
                                continue;
                            //
                            component.OnLoad();
                            mList.Add(component);
                        }
                    }
                } catch (Exception e) {
                }

            }

            //启动组件
            foreach (IComponent component in mList) {
                component.OnReady();
            }
        }
    }
}
