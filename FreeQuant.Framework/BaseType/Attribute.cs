using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Framework {
    [AttributeUsage(AttributeTargets.Class)]
    public class InstrumentsAttribute : Attribute {
        private string[] mInstruments;
        public InstrumentsAttribute(params string[] instruments) {
            mInstruments = instruments;
        }

        public string[] Instruments => mInstruments;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class StrategyNameAttribute : Attribute {
        private string mName;
        public StrategyNameAttribute(string name) {
            mName = name;
        }

        public string Name => mName;
    }
}
