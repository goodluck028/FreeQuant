using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class BrokerManager {
        public static ConcurrentDictionary<string, BaseMdBroker> sMdBrokerDic = new ConcurrentDictionary<string, BaseMdBroker>();
        public static ConcurrentDictionary<string, BaseTdBroker> sTdBrokerDic = new ConcurrentDictionary<string, BaseTdBroker>();

        public static BaseMdBroker DefaultMdBroker {
            get {
                string key = ConfigUtil.DefaultMdBroker;
                BaseMdBroker broker;
                if (sMdBrokerDic.TryGetValue(key, out broker)) {
                    return broker;
                } else {
                    throw new Exception("broker not find");
                }
            }
        }
        public static BaseTdBroker DefaultTdBroker {
            get {
                string key = ConfigUtil.DefaultTdBroker;
                BaseTdBroker broker;
                if (sTdBrokerDic.TryGetValue(key, out broker)) {
                    return broker;
                } else {
                    throw new Exception("broker not find");
                }
            }
        }
    }
}
