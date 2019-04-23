using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FreeQuant.Framework {
    internal class EventQueue {
        private BlockingCollection<Event> mQueue = new BlockingCollection<Event>();
        private Thread mThread;
        private ThreadPriority mPriority;
        //
        public EventQueue(ThreadPriority priority) {
            mPriority = priority;
            start();
        }

        public bool post(Event msg) {
            return mQueue.TryAdd(msg, 1000);

        }

        private void start() {
            if (mThread == null || !mThread.IsAlive) {
                mThread = new Thread(() => {
                    try {
                        while (true) {
                            Event e = mQueue.Take();
                            OnEvent?.Invoke(e);
                        }
                    } catch (Exception ex) {
                        Console.WriteLine(ex.StackTrace);
                    }
                });
                mThread.Priority = mPriority;
                mThread.Start();
            }

        }

        public event Action<Event> OnEvent;
    }
}
