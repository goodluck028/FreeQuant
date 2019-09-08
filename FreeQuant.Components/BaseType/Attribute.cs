using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.Components {
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class InstrumentsAttribute:Attribute
    {
        private string[] mInstruments;
        public InstrumentsAttribute(params string[] instruments)
        {
            mInstruments = instruments;
        }

        public string[] Instruments => mInstruments;
    }
}
