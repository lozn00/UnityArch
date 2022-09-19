using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public sealed class Common : Singleton<Common>, IDisposable, IUpdate, ILateUpdate, IFixedUpdate
    {
        private Dictionary<Type, CommonObject> common = new Dictionary<Type, CommonObject>();

        private Dictionary<Type, IUpdate> updateEvent = new Dictionary<Type, IUpdate>();
        private Dictionary<Type, ILateUpdate> lateUpdateEvent = new Dictionary<Type, ILateUpdate>();
        private Dictionary<Type, IFixedUpdate> fixedUpdateEvent = new Dictionary<Type, IFixedUpdate>();

        /// <summary>
        /// 设置公共对象
        /// </summary>
        /// <param name="obj"></param>
        public void Set(CommonObject obj)
        {
            Type type = obj.GetType();
            if (!common.ContainsKey(type))
            {
                common.Add(type, obj);

                if (obj is IUpdate u && !updateEvent.ContainsKey(type))
                {
                    updateEvent.Add(type, u);
                }

                if (obj is ILateUpdate l && !lateUpdateEvent.ContainsKey(type))
                {
                    lateUpdateEvent.Add(type, l);
                }

                if (obj is IFixedUpdate f && !fixedUpdateEvent.ContainsKey(type))
                {
                    fixedUpdateEvent.Add(type, f);
                }
            }
        }

        /// <summary>
        /// 获取公共对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : CommonObject, new()
        {
            return Get(typeof(T)) as T;
        }

        /// <summary>
        /// 获取公共对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public CommonObject Get(Type type)
        {
            common.TryGetValue(type, out var obj);
            return obj;
        }

        /// <summary>
        /// 移除公共对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        public void Remove<T>(T o) where T : CommonObject
        {
            Type type = o.GetType();
            common.Remove(type);
        }

        /// <summary>
        /// 移除公共对象
        /// </summary>
        /// <param name="type"></param>
        public void Remove(Type type)
        {
            common.Remove(type);
            updateEvent.Remove(type);
            lateUpdateEvent.Remove(type);
            fixedUpdateEvent.Remove(type);
        }

        public void Dispose()
        {
            using XList<CommonObject> objs = XList<CommonObject>.Create();
            objs.AddRange(common.Values);

            Queue<CommonObject> queue = new Queue<CommonObject>(objs);
            foreach (var obj in objs)
            {
                if (obj.GetType() == typeof(Global))
                {
                    queue.Enqueue(obj);
                    continue;
                }
                obj.Dispose();
            }

            while(queue.Count > 0)
            {
                CommonObject obj = queue.Dequeue();
                obj.Dispose();
            }

            common.Clear();
            updateEvent.Clear();
            lateUpdateEvent.Clear();
            fixedUpdateEvent.Clear();
            Instance = null;
        }

        public void FixedUpdate()
        {
            foreach (IFixedUpdate o in fixedUpdateEvent.Values)
            {
                o.FixedUpdate();
            }
        }

        public void LateUpdate()
        {
            foreach (ILateUpdate o in lateUpdateEvent.Values)
            {
                o.LateUpdate();
            }
        }

        public void Update()
        {
            foreach (IUpdate o in updateEvent.Values)
            {
                o.Update();
            }
        }
    }
}
