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

namespace FreeQuant.Framework {
    public static class ConfigUtil {
        static ConfigUtil() {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\config.ini";
            if (!File.Exists(path)) {
                File.Create(path).Close();
            }
        }

        private static IniConfig mConfig = new IniConfig();
        public static IniConfig Config => mConfig;

        public class IniConfig {
            public string this[string type, string name] {
                get {
                    var parser = new FileIniDataParser();
                    IniData data = parser.ReadFile("config.ini");
                    string text = data[type][name];
                    if (text == null) {
                        data[type][name] = "";
                        parser.WriteFile("config.ini", data);
                        return "";
                    } else {
                        return text;
                    }
                }
                set {
                    var parser = new FileIniDataParser();
                    IniData data = parser.ReadFile("config.ini");
                    data[type][name] = value;
                    parser.WriteFile("config.ini", data);
                }
            }
        }

        //
        public static string DefaultMdBroker {
            get {
                string text = Config["Broker", "DefaultMdBroker"];
                if (text.Equals("")) {
                    text = "Broker.Xapi2.XapiMdBroker";
                    Config["Broker", "DefaultMdBroker"] = text;
                }
                return text;
            }
        }
        public static string DefaultTdBroker {
            get {
                string text = Config["Broker", "DefaultTdBroker"];
                if (text.Equals("")) {
                    text = "Broker.Xapi2.XapiTdBroker";
                    Config["Broker", "DefaultTdBroker"] = text;
                }
                return text;
            }
        }

        //
        public static string TdServer {
            get {
                string text = Config["TdAccount", "Server"];
                if (text.Equals("")) {
                    text = "tcp://180.168.146.187:10100";
                    Config["TdAccount", "Server"] = text;
                }
                return text;
            }
        }

        public static string TdBroker {
            get {
                string text = Config["TdAccount", "Broker"];
                if (text.Equals("")) {
                    text = "9999";
                    Config["TdAccount", "Broker"] = text;
                }
                return text;
            }
        }

        public static string TdInvestor {
            get {
                string text = Config["TdAccount", "Investor"];
                if (text.Equals("")) {
                    text = "123456";
                    Config["TdAccount", "Investor"] = text;
                }
                return text;
            }
        }

        public static string TdPassword {
            get {
                string text = Config["TdAccount", "Password"];
                if (text.Equals("")) {
                    text = "123456";
                    Config["TdAccount", "Password"] = text;
                }
                return text;
            }
        }

        //
        public static string MdServer {
            get {
                string text = Config["MdAccount", "Server"];
                if (text.Equals("")) {
                    text = "tcp://180.168.146.187:10110";
                    Config["MdAccount", "Server"] = text;
                }
                return text;
            }
        }

        public static string MdBroker {
            get {
                string text = Config["MdAccount", "Broker"];
                if (text.Equals("")) {
                    text = "9999";
                    Config["MdAccount", "Broker"] = text;
                }
                return text;
            }
        }

        public static string MdInvestor {
            get {
                string text = Config["MdAccount", "Investor"];
                if (text.Equals("")) {
                    text = "123456";
                    Config["MdAccount", "Investor"] = text;
                }
                return text;
            }
        }

        public static string MdPassword {
            get {
                string text = Config["MdAccount", "Password"];
                if (text.Equals("")) {
                    text = "123456";
                    Config["MdAccount", "Password"] = text;
                }
                return text;
            }
        }

        //
        public static string AppId {
            get {
                string text = Config["BrokerAuth", "AppId"];
                if (text.Equals("")) {
                    text = "simnow_client_test";
                    Config["BrokerAuth", "AppId"] = text;
                }
                return text;
            }
        }

        public static string AuthCode {
            get {
                string text = Config["BrokerAuth", "AuthCode"];
                if (text.Equals("")) {
                    text = "0000000000000000";
                    Config["BrokerAuth", "AuthCode"] = text;
                }
                return text;
            }
        }

    }
}
