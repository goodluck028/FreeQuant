using FreeQuant.EventEngin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FreeQuant.Framework {
    public class BrokerMonitor : IComponent {
        private Timer mTimer;

        public void OnLoad() {
            mTimer = new Timer(_check, null, 1000 * 60, 1000 * 60);
        }

        public void OnReady() { }

        private void _check(object state) {
            BrokerEvent.MonitorEvent evt = new BrokerEvent.MonitorEvent();
            EventBus.PostEvent(evt);
        }
    }
}
