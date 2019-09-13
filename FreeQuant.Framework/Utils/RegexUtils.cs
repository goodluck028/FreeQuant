using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class RegexUtils {
        //过滤合约
        public static bool MatchInstrument(string instId) {
            Regex re = new Regex(@"^[a-zA-Z]+\d+$", RegexOptions.None);
            return re.IsMatch(instId);
        }
        //提取短合约名
        public static string TakeShortInstrumentID(string instId) {
            string name = Regex.Replace(instId, @"[0-9]", "", RegexOptions.None);
            string month = Regex.Replace(instId, @"[a-zA-Z]", "", RegexOptions.None);
            return name + month.Substring(month.Length - 2);
        }
        // 提取品种名
        public static string TakeProductName(string instId)
        {
            return Regex.Replace(instId, @"[0-9]", "", RegexOptions.None);
        }
    }
}
