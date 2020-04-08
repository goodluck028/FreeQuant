using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    public static class RegexUtils {
        //过滤合约
        public static bool MatchInstrument(string instID) {
            Regex re = new Regex(@"^[a-zA-Z]+\d+$", RegexOptions.None);
            return re.IsMatch(instID);
        }
        //提取短合约名
        public static string TakeShortInstrumentID(string instID) {
            string name = Regex.Replace(instID, @"[0-9]", "", RegexOptions.None);
            string month = Regex.Replace(instID, @"[a-zA-Z]", "", RegexOptions.None);
            return name + month.Substring(month.Length - 2);
        }
        // 提取品种名
        public static string TakeProductName(string instID)
        {
            return Regex.Replace(instID, @"[0-9]", "", RegexOptions.None);
        }
    }
}
