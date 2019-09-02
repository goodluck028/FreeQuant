using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Components;

namespace FreeQuant.DataReceiver {
    internal class MySqlConfig {
        private static MySqlConfig mInstance = new MySqlConfig();
        private MySqlConfig() { }
        public static MySqlConfig Config => mInstance;
        //
        public string IP {
            get {
                string text = ConfigUtil.Config["MySql", "IP"];
                if (text.Equals("")) {
                    text = "127.0.0.1:3306";
                    ConfigUtil.Config["MySql", "IP"] = text;
                }
                return text;
            }
        }

        public string UserName {
            get {
                string text = ConfigUtil.Config["MySql", "UserName"];
                if (text.Equals("")) {
                    text = "client";
                    ConfigUtil.Config["MySql", "UserName"] = text;
                }
                return text;
            }
        }

        public string Password {
            get {
                string text = ConfigUtil.Config["MySql", "Password"];
                if (text.Equals("")) {
                    text = "123";
                    ConfigUtil.Config["MySql", "Password"] = text;
                }
                return text;
            }
        }

        public string Database {
            get {
                string text = ConfigUtil.Config["MySql", "Database"];
                if (text.Equals("")) {
                    text = "hisdata_future";
                    ConfigUtil.Config["MySql", "Database"] = text;
                }
                return text;
            }
        }

        public string Instruments {
            get {
                string text = ConfigUtil.Config["Product", "Instruments"];
                if (text.Equals("")) {
                    text = "rb,ni,al,zn,cu,ag,fu,bu,MA,ZC,I,J,JM,V,PP,EG";
                    ConfigUtil.Config["Product", "Instruments"] = text;
                }
                return text;
            }
        }
    }
}
