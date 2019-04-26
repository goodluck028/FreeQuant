using System;

namespace FreeQuant.Framework {
    [AttributeUsage(AttributeTargets.Method)]
    public class OnEventAttribute : Attribute {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class OnLogAttribute : Attribute {
    }
}
