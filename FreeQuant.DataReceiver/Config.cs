using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeQuant.Framework;

namespace FreeQuant.DataReceiver {
    /// <summary>
    /// 配置类
    /// </summary>
    internal class Config {
        //sqlserver服务器配置
        public static string Server {
            get {
                string text = ConfigUtil.Config["DataBase", "Server"];
                if (text.Equals("")) {
                    text = "Data Source = 127.0.0.1;Initial Catalog = hisdata_future;User Id = sa;Password = 123;";
                    ConfigUtil.Config["DataBase", "Server"] = text;
                }
                return text;
            }
        }

        //合约配置
        public static string Instruments {
            get {
                string text = ConfigUtil.Config["DataBase", "Instruments"];
                if (text.Equals("")) {
                    text = "rb,ni,al,zn,cu,ag,fu,bu,MA,ZC,I,J,JM,V,PP,EG";
                    ConfigUtil.Config["DataBase", "Instruments"] = text;
                }
                return text;
            }
        }
    }
}
