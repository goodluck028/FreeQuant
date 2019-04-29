using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace FreeQuant.Framework {
    public static class EventBus {
        //
        private static EventQueue mQueue = new EventQueue(ThreadPriority.Highest);
        private static EventQueue mLogQueue = new EventQueue(ThreadPriority.Lowest);
        private static ConcurrentDictionary<Type, HashSet<InvokeWrapper>> mEventInvokeMap = new ConcurrentDictionary<Type, HashSet<InvokeWrapper>>();
        private static ConcurrentDictionary<Type, HashSet<InvokeWrapper>> mLogInvokeMap = new ConcurrentDictionary<Type, HashSet<InvokeWrapper>>();

        //
        static EventBus() {
            mQueue.OnEvent += _onEvent;
            mLogQueue.OnEvent += _onLog;
        }

        public static void Run() {
            ModuleLoader.LoadAllModules();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="obj"></param>
        public static void Register(object obj) {
            if (obj == null)
                return;
            Type t = obj.GetType();
            MethodInfo[] infos = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo md in infos) {
                //方法只能有一个参数
                ParameterInfo[] parameters = md.GetParameters();
                if (parameters.Length != 1)
                    continue;
                //判断是否加了指定特性
                Type pType = parameters[0].ParameterType;
                foreach (Attribute attr in md.GetCustomAttributes()) {
                    if (attr is OnEventAttribute) {
                        HashSet<InvokeWrapper> set;
                        if (mEventInvokeMap.TryGetValue(pType, out set)) {
                            lock (mEventInvokeMap) {
                                set.Add(new InvokeWrapper(obj, md));
                            }
                        } else {
                            set = new HashSet<InvokeWrapper>();
                            set.Add(new InvokeWrapper(obj, md));
                            mEventInvokeMap.TryAdd(pType, set);
                        }
                    } else if (attr is OnLogAttribute) {
                        HashSet<InvokeWrapper> set;
                        if (mLogInvokeMap.TryGetValue(pType, out set)) {
                            lock (mEventInvokeMap) {
                                set.Add(new InvokeWrapper(obj, md));
                            }
                        } else {
                            set = new HashSet<InvokeWrapper>();
                            set.Add(new InvokeWrapper(obj, md));
                            mLogInvokeMap.TryAdd(pType, set);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 解除注册
        /// </summary>
        /// <param name="obj"></param>
        public static void UnRegister(object obj) {
            if (obj == null)
                return;
            Type t = obj.GetType();
            MethodInfo[] infos = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            foreach (MethodInfo md in infos) {
                //方法只能有一个参数
                ParameterInfo[] parameters = md.GetParameters();
                if (parameters.Length != 1)
                    continue;
                //判断是否加了指定特性
                Type pType = parameters[0].ParameterType;
                foreach (Attribute attr in md.GetCustomAttributes()) {
                    if (attr is OnEventAttribute) {
                        HashSet<InvokeWrapper> set;
                        if (mEventInvokeMap.TryGetValue(pType, out set)) {
                            lock (mEventInvokeMap) {
                                set.Remove(new InvokeWrapper(obj, md));
                            }
                        }
                    } else if (attr is OnLogAttribute) {
                        HashSet<InvokeWrapper> set;
                        if (mLogInvokeMap.TryGetValue(pType, out set)) {
                            lock (mEventInvokeMap) {
                                set.Remove(new InvokeWrapper(obj, md));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        /// <param name="t"></param>
        public static void PostEvent<T>(T t) {
            mQueue.post(new Event(typeof(T), t));
        }

        /// <summary>
        /// 处理各种事件
        /// </summary>
        /// <param name="evt"></param>
        internal static void _onEvent(Event evt) {
            if (evt == null)
                return;
            Type t = evt.ValueType;
            HashSet<InvokeWrapper> set;
            if (mEventInvokeMap.TryGetValue(t, out set)) {
                lock (mEventInvokeMap) {
                    foreach (InvokeWrapper wrapper in set) {
                        try {
                            wrapper.Invoke(evt.Value);
                        } catch (Exception e) {
                            EventBus.PostLog(new Error(e));
                        }
                    }
                }
            }
        }



        /// <summary>
        /// 发布日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void PostLog<T>(T t) {
            mLogQueue.post(new Event(typeof(T), t));
        }

        /// <summary>
        /// 处理各种事件
        /// </summary>
        /// <param name="evt"></param>
        internal static void _onLog(Event evt) {
            if (evt == null)
                return;
            Type t = evt.ValueType;
            HashSet<InvokeWrapper> set;
            if (mLogInvokeMap.TryGetValue(t, out set)) {
                lock (mLogInvokeMap) {
                    foreach (InvokeWrapper wrapper in set) {
                        try {
                            wrapper.Invoke(evt.Value);
                        } catch (Exception e) {
                            EventBus.PostLog(new Error(e));
                        }
                    }
                }
            }
        }

    }

    internal class InvokeWrapper {
        private object mObj;
        private MethodInfo mMethod;

        public override int GetHashCode() {
            return mObj.GetHashCode();
        }

        public override bool Equals(object obj) {
            InvokeWrapper wrapper = obj as InvokeWrapper;
            if (wrapper == null) {
                return false;
            } else {
                return mObj.Equals(wrapper.Obj);
            }
        }

        public object Obj => mObj;

        public InvokeWrapper(object obj, MethodInfo method) {
            mObj = obj;
            mMethod = method;
        }

        public void Invoke(object obj) {
            mMethod.Invoke(mObj, new object[] { obj });
        }
    }

}
