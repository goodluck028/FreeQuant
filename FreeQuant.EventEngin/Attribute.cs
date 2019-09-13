using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeQuant.EventEngin {
    [AttributeUsage(AttributeTargets.Method)]
    public class OnEventAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnLogAttribute : Attribute {
    }
}
