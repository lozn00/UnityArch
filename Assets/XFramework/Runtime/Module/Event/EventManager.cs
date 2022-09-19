using System.Collections;
using System.Collections.Generic;
using System;

namespace XFramework
{
    public class EventManager : Singleton<EventManager>, IDisposable
    {
        private UnOrderMapList<Type, object> allEvents = new UnOrderMapList<Type, object>();

        /// <summary>
        /// Ìí¼Ó¼àÌý
        /// </summary>
        /// <param name="target"></param>
        public void AddTarget(object target)
        {
            Type targetType = target.GetType();
            if (TypesManager.Instance.TryGetEvents(targetType, out var types))
            {
                foreach (var argType in types)
                {
                    allEvents.TryAdd(argType, target);
                }
            }
        }

        /// <summary>
        /// Ìí¼Ó¼àÌý
        /// </summary>
        /// <param name="target"></param>
        /// <param name="argType"></param>
        public void AddListener(object target, Type argType)
        {
            Type targetType = target.GetType();
            if (!TypesManager.Instance.TryGetEvents(targetType, out var types))
                return;

            if (types.Contains(argType))
            {
                allEvents.TryAdd(argType, target);
            }
        }

        /// <summary>
        /// ÒÆ³ý¼àÌý
        /// </summary>
        /// <param name="target"></param>
        /// <param name="argType"></param>
        /// <returns></returns>
        public bool RemoveListener(object target, Type argType)
        {
            Type targetType = target.GetType();
            if (!TypesManager.Instance.TryGetEvents(targetType, out _))
                return false;

            return allEvents.Remove(argType, targetType);
        }

        /// <summary>
        /// ÒÆ³ýÄ¿±ê¼àÌý
        /// </summary>
        /// <param name="target"></param>
        public void RemoveTarget(object target)
        {
            Type targetType = target.GetType();
            if (TypesManager.Instance.TryGetEvents(targetType, out var types))
            {
                foreach (var type in types)
                {
                    allEvents.Remove(type, target);
                }
            }
        }

        /// <summary>
        /// ÒÆ³ýËùÓÐ¼àÌý
        /// </summary>
        public void RemoveAllListeners()
        {
            allEvents.Clear();
        }

        /// <summary>
        /// ÍÆËÍÊÂ¼þ
        /// </summary>
        /// <param name="args"></param>
        public void Publish<T>(T args) where T : struct
        {
            Type type = typeof(T);
            if (!allEvents.TryGetValue(type, out var list))
                return;

            foreach (var o in list)
            {
                try
                {
                    (o as IEvent<T>).RunEvent(args);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        public void Dispose()
        {
            RemoveAllListeners();
            Instance = null;
        }
    }
}
