using System;

namespace FreeQuant.Framework {
    internal class Event {
        private EventType mEventType;
        private Type mValueType;
        private object mValue;

        internal Event(EventType eventType, Type valueType, object value) {
            mEventType = eventType;
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

        internal EventType EventType {
            get { return mEventType; }
        }
    }

    internal enum EventType {
        publish, request
    }
}
