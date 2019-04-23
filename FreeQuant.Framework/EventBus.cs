using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FreeQuant.Framework {
    public static class EventBus {

        //
        private static EventQueue mQueue = new EventQueue(ThreadPriority.Highest);
        private static EventQueue mLogQueue = new EventQueue(ThreadPriority.Lowest);
        private static ConcurrentDictionary<Type, ConcurrentBag<IEventHandler>> mSubscriberMap = new ConcurrentDictionary<Type, ConcurrentBag<IEventHandler>>();
        private static ConcurrentDictionary<Type, ConcurrentBag<IEventHandler>> mResponserMap = new ConcurrentDictionary<Type, ConcurrentBag<IEventHandler>>();
        private static ConcurrentDictionary<Type, ConcurrentBag<IEventHandler>> mLogSubscriberMap = new ConcurrentDictionary<Type, ConcurrentBag<IEventHandler>>();
        //

        static EventBus() {
            mQueue.OnEvent += _onEvent;
            mLogQueue.OnEvent += _onLog;
        }

        public static void Run() {
            ModuleLoader.LoadModules();
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="t"></param>
        public static void Publish<T>(T t) {
            mQueue.post(new Event(EventType.publish, typeof(T), t));
        }

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="act"></param>
        public static void Subscribe<T>(Action<T> act) {
            ActionWrapper<T> wrapper = new ActionWrapper<T>(act);
            ConcurrentBag<IEventHandler> bag;
            if (mSubscriberMap.TryGetValue(typeof(T), out bag)) {
                foreach (ActionWrapper<T> w in bag) {
                    if (w.Action == act) {
                        return;
                    }
                }
            } else {
                bag = new ConcurrentBag<IEventHandler>();
                mSubscriberMap.TryAdd(typeof(T), bag);
            }
            bag.Add(wrapper);
        }

        /// <summary>
        /// 退订事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="act"></param>
        public static void UnSubscribe<T>(Action<T> act) {
            ConcurrentBag<IEventHandler> bag;
            if (mSubscriberMap.TryGetValue(typeof(T), out bag)) {
                ConcurrentBag<IEventHandler> removeBag = new ConcurrentBag<IEventHandler>();
                foreach (ActionWrapper<T> w in bag) {
                    if (w.Action == act) {
                        removeBag.Add(w);
                    }
                }

                foreach (IEventHandler r in removeBag) {
                    IEventHandler one = r;
                    bag.TryTake(out one);
                }

            }
        }

        /// <summary>
        /// 注册请求响应
        /// </summary>
        /// <param name="act"></param>
        public static void RegistResponser<T, M>(Action<Request<T, M>> act) {
            ActionWrapper<Request<T, M>> wrapper = new ActionWrapper<Request<T, M>>(act);
            ConcurrentBag<IEventHandler> bag;
            if (mResponserMap.TryGetValue(typeof(T), out bag)) {
                foreach (ActionWrapper<Request<T, M>> w in bag) {
                    if (w.Action == act) {
                        return;
                    }
                }
            } else {
                bag = new ConcurrentBag<IEventHandler>();
                mResponserMap.TryAdd(typeof(T), bag);
            }
            bag.Add(wrapper);
        }

        /// <summary>
        /// 解除请求响应
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="act"></param>
        public static void UnRegistResponser<T, M>(Action<Request<T, M>> act) {
            ConcurrentBag<IEventHandler> bag;
            if (mResponserMap.TryGetValue(typeof(T), out bag)) {
                ConcurrentBag<IEventHandler> removeBag = new ConcurrentBag<IEventHandler>();
                foreach (ActionWrapper<Request<T, M>> w in bag) {
                    if (w.Action == act) {
                        removeBag.Add(w);
                    }
                }

                foreach (IEventHandler r in removeBag) {
                    IEventHandler one = r;
                    bag.TryTake(out one);
                }

            }
        }

        /// <summary>
        /// 新建请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <param name="req"></param>
        /// <returns></returns>
        public static Request<T, M> NewRequest<T, M>(T req) {
            return new Request<T, M>(req);
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="M"></typeparam>
        /// <param name="request"></param>
        internal static void SendRequest<T, M>(Request<T, M> request) {
            mQueue.post(new Event(EventType.request, typeof(Request<T, M>), request));
        }

        /// <summary>
        /// 处理各种事件
        /// </summary>
        /// <param name="evt"></param>
        internal static void _onEvent(Event evt) {
            Type t = evt.ValueType;
            ConcurrentBag<IEventHandler> bag;
            switch (evt.EventType) {
                case EventType.publish:
                    if (mSubscriberMap.TryGetValue(t, out bag)) {
                        foreach (IEventHandler wrapper in bag) {
                            wrapper.Handle(evt);
                        }
                    }
                    break;
                case EventType.request:
                    if (mResponserMap.TryGetValue(t, out bag) && bag.Count > 0) {
                        foreach (IEventHandler wrapper in bag) {
                            wrapper.Handle(evt);
                        }
                    } else {
                        ((IRequest)(evt.Value)).Error(new Exception("没有响应模块"));
                    }
                    break;
            }


        }



        /// <summary>
        /// 发布日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void PublishLog<T>(T t) {
            mLogQueue.post(new Event(EventType.publish, typeof(T), t));
        }

        /// <summary>
        /// 订阅日志
        /// </summary>
        /// <param name="act"></param>
        public static void SubscribeLog<T>(Action<T> act) {
            ActionWrapper<T> wrapper = new ActionWrapper<T>(act);
            ConcurrentBag<IEventHandler> bag;
            if (mLogSubscriberMap.TryGetValue(typeof(T), out bag)) {
                foreach (ActionWrapper<T> w in bag) {
                    if (w.Action == act) {
                        return;
                    }
                }
            } else {
                bag = new ConcurrentBag<IEventHandler>();
                mLogSubscriberMap.TryAdd(typeof(T), bag);
            }
            bag.Add(wrapper);
        }

        /// <summary>
        /// 退订日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="act"></param>
        public static void UnSubscribeLog<T>(Action<T> act) {
            ConcurrentBag<IEventHandler> bag;
            if (mLogSubscriberMap.TryGetValue(typeof(T), out bag)) {
                ConcurrentBag<IEventHandler> removeBag = new ConcurrentBag<IEventHandler>();
                foreach (ActionWrapper<T> w in bag) {
                    if (w.Action == act) {
                        removeBag.Add(w);
                    }
                }

                foreach (IEventHandler r in removeBag) {
                    IEventHandler one = r;
                    bag.TryTake(out one);
                }

            }
        }

        /// <summary>
        /// 处理各种事件
        /// </summary>
        /// <param name="evt"></param>
        internal static void _onLog(Event evt) {
            Type t = evt.ValueType;
            ConcurrentBag<IEventHandler> bag;
            if (mLogSubscriberMap.TryGetValue(t, out bag)) {
                foreach (IEventHandler wrapper in bag) {
                    wrapper.Handle(evt);
                }
            }
        }

    }

    internal class ActionWrapper<T> : IEventHandler {
        private Action<T> mAction;

        public Action<T> Action {
            get { return mAction; }
        }

        public ActionWrapper(Action<T> act) {
            mAction = act;
        }

        public void Handle(Event evt) {
            mAction?.Invoke((T)(evt.Value));
        }
    }

    internal interface IEventHandler {
        void Handle(Event evt);
    }

}
