using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using FreeQuant.Modules.BaseType;
using Newtonsoft.Json;

namespace FreeQuant.Modules {
    public class ConfigUtil {
        public static Config Config {
            get {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\config.json";
                Config config;
                if (File.Exists(path)) {
                    string json = File.ReadAllText(path);
                    config = JsonConvert.DeserializeObject<Config>(json);
                } else {
                    config = new Config();
                    config.MyTdAccount = new Account("tcp://218.202.237.33:10000", "9999", "123456", "123456");
                    config.MyMdAccount = new Account("tcp://218.202.237.33:10001", "9999", "123456", "123456");
                    config.MyServerInfo = new ServerInfo("12345678");
                    File.Create(path).Close();
                    File.WriteAllText(path, JsonConvert.SerializeObject(config, Formatting.Indented));
                }
                return config;
            }
        }
    }

    public class Config {
        private Account myTdAccount;
        private Account myMdAccount;
        private ServerInfo myServerInfo;

        public Account MyTdAccount {
            get { return myTdAccount; }
            set { myTdAccount = value; }
        }

        public Account MyMdAccount {
            get { return myMdAccount; }
            set { myMdAccount = value; }
        }

        public ServerInfo MyServerInfo {
            get { return myServerInfo; }
            set { myServerInfo = value; }
        }
    }
}
