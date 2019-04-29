using System;

namespace FreeQuant.Framework {
    internal class Event {
        private Type mValueType;
        private object mValue;

        internal Event(Type valueType, object value) {
            mValueType = valueType;
            mValue = value;
        }

        internal object Value {
            get {
                return mValue;
            }
        }

        internal Type ValueType {
            get {
                return mValueType;
            }
        }
    }

    public class Error
    {
        private Exception mException;

        public Error(Exception exception)
        {
            mException = exception;
        }

        public Exception Exception => mException;
    }
}
