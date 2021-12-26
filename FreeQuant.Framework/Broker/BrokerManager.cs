using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework
{
    /// <summary>
    /// broker管理器，通过该类加载、获取broker
    /// </summary>
    public class BrokerManager
    {
        //行情borker字典
        private ConcurrentDictionary<string, BaseMdBroker> MdBrokerDic = new ConcurrentDictionary<string, BaseMdBroker>();
        //交易broker字典
        private ConcurrentDictionary<string, BaseTdBroker> TdBrokerDic = new ConcurrentDictionary<string, BaseTdBroker>();

        //只能内部初始化
        internal BrokerManager()
        {
            LoadBroker();
        }

        //从本地dll文件中动态加载borker
        private void LoadBroker()
        {
            //获取文件列表 
            string[] files = new string[] { };
            try
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "\\brokers";
                if (Directory.Exists(dir))
                {
                    files = Directory.GetFiles(dir);
                }
                else
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception ex)
            {
                LogUtil.SysLog(ex.StackTrace);
            }

            //加载borker
            foreach (string f in files)
            {
                Assembly assembly;
                try
                {
                    assembly = Assembly.LoadFrom(f);
                }
                catch (Exception e)
                {
                    continue;
                }
                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    //
                    if (t.IsSubclassOf(typeof(BaseMdBroker)))
                    {
                        if (MdBrokerDic.ContainsKey(t.FullName))
                            throw new Exception($"行情borker重复， {t.FullName}");
                        MdBrokerDic[t.FullName] = Activator.CreateInstance(t) as BaseMdBroker;
                    }
                    else if (t.IsSubclassOf(typeof(BaseTdBroker)))
                    {
                        if (TdBrokerDic.ContainsKey(t.FullName))
                            throw new Exception($"交易borker重复， {t.FullName}");
                        TdBrokerDic[t.FullName] = Activator.CreateInstance(t) as BaseTdBroker;
                    }
                }


            }
        }

        /// <summary>
        /// 根据类名查找行情borker
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public BaseMdBroker GetMdBrokerByClassName(string className)
        {
            BaseMdBroker broker;
            if (MdBrokerDic.TryGetValue(className, out broker))
            {
                return broker;
            }
            else
            {
                throw new Exception("broker not find");
            }
        }

        //配置文件中默认的行情broker
        public BaseMdBroker DefaultMdBroker => GetMdBrokerByClassName(ConfigUtil.DefaultMdBroker);

        /// <summary>
        /// 根据类名查找交易borker
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public BaseTdBroker GetTdBrokerByClassName(string className)
        {
            BaseTdBroker broker;
            if (TdBrokerDic.TryGetValue(className, out broker))
            {
                return broker;
            }
            else
            {
                throw new Exception("broker not find");
            }
        }

        //配置文件中默认的交易broker
        public BaseTdBroker DefaultTdBroker => GetTdBrokerByClassName(ConfigUtil.DefaultTdBroker);
    }
}
