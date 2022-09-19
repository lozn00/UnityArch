using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public sealed class ObjectPool : Singleton<ObjectPool>, IDisposable
    {
        private Dictionary<Type, Queue<object>> pool = new Dictionary<Type, Queue<object>>();

        public void Init()
        {

        }

        /// <summary>
        /// �ӳ�����ȡ��һ������û���򴴽�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Fetch<T>() where T : class, new()
        {
            return Fetch(typeof(T)) as T;
        }

        /// <summary>
        /// �ӳ�����ȡ��һ������û���򴴽�
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Fetch(Type type)
        {
            if (!Dequeue(type, out object obj))
            {
                object o = Activator.CreateInstance(type);
                obj = o;
            }

            return obj;
        }

        /// <summary>
        /// ���ն���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="o"></param>
        public void Recycle<T>(T o) where T : class
        {
            Enqueue(o);
        }

        private void Enqueue(object obj)
        {
            Type type = obj.GetType();
            if (!pool.TryGetValue(type, out Queue<object> queue))
            {
                queue = new Queue<object>();
                pool.Add(type, queue);
            }

            queue.Enqueue(obj);
        }

        private bool Dequeue(Type type, out object obj)
        {
            obj = default;

            if (pool.TryGetValue(type, out Queue<object> queue))
            {
                if (queue.Count > 0)
                {
                    obj = queue.Dequeue();
                }
            }

            return obj != null;
        }

        public void Dispose()
        {
            pool.Clear();
            //Instance = null;
        }
    }
}
