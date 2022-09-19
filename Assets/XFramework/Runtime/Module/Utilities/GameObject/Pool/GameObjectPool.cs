using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XFramework
{
    public class GameObjectPool : Singleton<GameObjectPool>, IDisposable
    {
        private Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();

        /// <summary>
        /// 存储池子事件的对象
        /// </summary>
        private UnOrderMapList<string, object> poolEvents = new UnOrderMapList<string, object>();

        /// <summary>
        /// UI对象池位置
        /// </summary>
        private Transform uiHidden = null;

        /// <summary>
        /// 非UI对象池位置
        /// </summary>
        private Transform notUIHideen = null;

        public void Init()
        {
            poolEvents.Clear();
            var types = TypesManager.Instance.GetTypes(typeof(PoolEventAttribute));
            foreach (var type in types)
            {
                XPoolEvent obj = Activator.CreateInstance(type) as XPoolEvent;
                if (obj == null)
                    continue;

                var attris = type.GetCustomAttributes(typeof(PoolEventAttribute), false);
                if (attris.Length == 0)
                    continue;

                foreach (PoolEventAttribute attri in attris)
                {
                    string key = attri.Key;
                    poolEvents.TryAdd(key, obj);
                }
            }
        }

        /// <summary>
        /// 执行池子事件
        /// </summary>
        /// <param name="obj"></param>
        private void RunPoolEvent(string key, Action<XPoolEvent> action)
        {
            if (poolEvents.TryGetValue(key, out var list))
            {
                foreach (var o in list)
                {
                    try
                    {
                        action?.Invoke(o as XPoolEvent);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e);
                    }
                }
            }
        }

        /// <summary>
        /// 取出
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject Fetch(string key)
        {
            GameObject obj = Dequeue(key);

            if (obj != null)
            {
                RunPoolEvent(key, e => e.FetchAfter(obj));
                obj.SetViewActive(true);
            }

            return obj;
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        public void Recycle(string key, GameObject obj)
        {
            if (!obj)
                return;

            bool isUI = obj.transform as RectTransform;

            RunPoolEvent(key, e => e.BeforeRecycle(obj));
            if (isUI)
                RecycleUI(obj);
            else
                RecycleNotUI(obj);

            Enqueue(key, obj);
            RunPoolEvent(key, e => e.RecycleAfter(obj));
        }

        private void RecycleUI(GameObject obj)
        {
            try
            {
                if (uiHidden == null)
                    uiHidden = Common.Instance?.Get<Global>().UI.Find("Hidden");

                obj.transform.SetParent(uiHidden, false);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private void RecycleNotUI(GameObject obj)
        {
            try
            {
                if (notUIHideen == null)
                {
                    Transform root = Common.Instance?.Get<Global>().GameRoot;
                    notUIHideen = root.Find("Pool");

                    if (notUIHideen == null)
                    {
                        GameObject pool = new GameObject("Pool");
                        pool.SetViewActive(false);
                        pool.transform.SetParent(root, false);
                        notUIHideen = pool.transform;
                    }
                }

                obj.transform.SetParent(notUIHideen, false);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private GameObject Dequeue(string key)
        {
            if (pool.TryGetValue(key, out Queue<GameObject> queue))
            {
                if (queue.Count > 0)
                    return queue.Dequeue();
            }

            return null;
        }

        private void Enqueue(string key, GameObject obj)
        {
            if (!pool.TryGetValue(key, out Queue<GameObject> queue))
            {
                queue = new Queue<GameObject>();
                pool.Add(key, queue);
            }

            queue.Enqueue(obj);
        }

        public void Clear()
        {
            foreach (var queue in pool.Values)
            {
                while (queue.Count > 0)
                {
                    GameObject obj = queue.Dequeue();
                    ResourcesManager.Instance?.Loader?.ReleaseInstance(obj);
                }
            }

            pool.Clear();
            poolEvents.Clear();
        }

        public void Dispose()
        {
            Clear();

            Instance = null;
        }
    }
}
