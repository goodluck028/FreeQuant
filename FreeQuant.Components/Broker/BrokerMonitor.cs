using FreeQuant.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FreeQuant.Components {
    [Component]
    public class BrokerMonitor
    {
        private Timer mTimer;
        public BrokerMonitor()
        {
            mTimer = new Timer(_check, null, 1000 * 60, 1000 * 60);
        }

        private void _check(object state) {
            BrokerEvent.MonitorEvent evt = new BrokerEvent.MonitorEvent();
            EventBus.PostEvent(evt);
        }
    }
}
