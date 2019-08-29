using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using IniParser;
using IniParser.Model;

namespace FreeQuant.Components {
    public class ConfigUtil {
        private static ConfigUtil mInstance = new ConfigUtil();

        private ConfigUtil() {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\config.ini";
            if (!File.Exists(path)) {
                File.Create(path).Close();
            }
        }
        public static ConfigUtil Config => mInstance;

        //        
        public string this[string type, string name] {
            get {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");
                string text = data[type][name];
                return text ?? "";
            }
            set {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile("config.ini");
                data[type][name] = value;
                parser.WriteFile("config.ini", data);
            }
        }

        //
        public string TdServer {
            get {
                string text = this["TdAccount", "Server"];
                if (text.Equals("")) {
                    text = "tcp://180.168.146.187:10100";
                    this["TdAccount", "Server"] = text;
                }
                return text;
            }
        }

        public string TdBroker {
            get {
                string text = this["TdAccount", "Broker"];
                if (text.Equals("")) {
                    text = "9999";
                    this["TdAccount", "Broker"] = text;
                }
                return text;
            }
        }

        public string TdInvestor {
            get {
                string text = this["TdAccount", "Investor"];
                if (text.Equals("")) {
                    text = "123456";
                    this["TdAccount", "Investor"] = text;
                }
                return text;
            }
        }

        public string TdPassword {
            get {
                string text = this["TdAccount", "Password"];
                if (text.Equals("")) {
                    text = "123456";
                    this["TdAccount", "Password"] = text;
                }
                return text;
            }
        }

        //
        public string MdServer {
            get {
                string text = this["MdAccount", "Server"];
                if (text.Equals("")) {
                    text = "tcp://180.168.146.187:10110";
                    this["MdAccount", "Server"] = text;
                }
                return text;
            }
        }

        public string MdBroker {
            get {
                string text = this["MdAccount", "Broker"];
                if (text.Equals("")) {
                    text = "9999";
                    this["MdAccount", "Broker"] = text;
                }
                return text;
            }
        }

        public string MdInvestor {
            get {
                string text = this["MdAccount", "Investor"];
                if (text.Equals("")) {
                    text = "123456";
                    this["MdAccount", "Investor"] = text;
                }
                return text;
            }
        }

        public string MdPassword {
            get {
                string text = this["MdAccount", "Password"];
                if (text.Equals("")) {
                    text = "123456";
                    this["MdAccount", "Password"] = text;
                }
                return text;
            }
        }

        //
        public string AppId {
            get {
                string text = this["BrokerAuth", "AppId"];
                if (text.Equals("")) {
                    text = "simnow_client_test";
                    this["BrokerAuth", "AppId"] = text;
                }
                return text;
            }
        }

        public string AuthCode {
            get {
                string text = this["BrokerAuth", "AuthCode"];
                if (text.Equals("")) {
                    text = "0000000000000000";
                    this["BrokerAuth", "AuthCode"] = text;
                }
                return text;
            }
        }

    }
}
