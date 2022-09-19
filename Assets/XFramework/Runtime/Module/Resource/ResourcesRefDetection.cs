using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    /// <summary>
    /// 检测资源是否有所引用
    /// </summary>
    public sealed class ResourcesRefDetection : CommonObject, ILateUpdate
    {
        private class AssetRef : XObject, IAwake<Object, XObject>, IAwake<GameObject, XObject, string, bool>
        {
            /// <summary>
            /// 是否来自实例化
            /// </summary>
            public bool IsInstantiate { get; private set; }

            public Object Object { get; private set; }

            public XObject Ref { get; private set; }

            public long RefId { get; private set; }

            public string Key { get; private set; }

            public bool IsFromPool { get; private set; }

            public void Initialize(Object o, XObject parent)
            {
                this.Object = o;
                this.Ref = parent;
                this.RefId = parent.TagId;
                this.Key = null;
                this.IsFromPool = false;
                this.IsInstantiate = false;
            }

            public void Initialize(GameObject obj, XObject parent, string key, bool isFromPool)
            {
                this.Object = obj;
                this.Ref = parent;
                this.RefId = parent.TagId;
                this.Key = key;
                this.IsFromPool = isFromPool;
                this.IsInstantiate = true;
            }

            /// <summary>
            /// 检查资源是否有效
            /// </summary>
            /// <returns></returns>
            public bool Check()
            {
                if (this.Object == null)
                    return false;

                if (this.Ref.IsDisposed)
                    return false;

                if (this.Ref.TagId != this.RefId)
                    return false;

                return true;
            }

            /// <summary>
            /// 释放资源
            /// </summary>
            private void Clear()
            {
                if (this.Object)
                {
                    if (this.IsInstantiate && this.Object is GameObject gameObject)
                    {
                        if (this.IsFromPool) 
                            GameObjectPool.Instance.Recycle(this.Key, gameObject);
                        else
                            ResourcesManager.Instance.Loader?.ReleaseInstance(gameObject);
                    }
                    else
                    {
                        ResourcesManager.Instance.Loader?.ReleaseAsset(this.Object);
                    }
                    this.Object = null;
                }
            }

            protected override void OnDestroy()
            {
                this.Clear();
                this.Ref = null;
                this.RefId = 0;
                this.Object = null;
                this.Key = null;
                this.IsInstantiate = false;
                this.IsFromPool = false;
            }
        }

        /// <summary>
        /// 所有加载的资源
        /// </summary>
        private Dictionary<int, AssetRef> assets = new Dictionary<int, AssetRef>();

        /// <summary>
        /// 所有失效的资源
        /// </summary>
        private Queue<int> invalidAssets = new Queue<int>();

        public void LateUpdate()
        {
            foreach (var item in assets)
            {
                int id = item.Key;
                var asset = item.Value;
                if (!asset.Check())
                {
                    asset.Dispose();
                    invalidAssets.Enqueue(id);
                }
            }

            while (invalidAssets.Count > 0)
            {
                int id = invalidAssets.Dequeue();
                assets.Remove(id);
            }
        }

        /// <summary>
        /// 添加资源进行管理，使asset关联parent，如果parent被释放了，则asset也会被释放
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="asset"></param>
        public int AddAsset(XObject parent, Object asset)
        {
            if (!asset)
                return 0;

            int id = asset.GetInstanceID();
            AssetRef assetRef = ObjectFactory.Create<AssetRef, Object, XObject>(asset, parent, true);
            assets.Add(asset.GetInstanceID(), assetRef);

            return id;
        }

        /// <summary>
        /// 添加实例化对象，使obj关联parent，如果parent被释放了，则obj也会被释放
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="obj"></param>
        /// <param name="key"></param>
        /// <param name="isFromPool"></param>
        public int AddInstantiate(XObject parent, GameObject obj, string key, bool isFromPool)
        {
            if (!obj)
                return 0;

            int id = obj.GetInstanceID();
            AssetRef assetRef = ObjectFactory.Create<AssetRef, GameObject, XObject, string, bool>(obj, parent, key, isFromPool, true);
            assets.Add(id, assetRef);

            return id;
        }

        public void Remove(int id)
        {
            if (this.assets.TryRemove(id, out AssetRef assetRef))
            {
                assetRef.Dispose();
            }
        }

        protected override void Destroy()
        {
            foreach (var asset in assets.Values)
            {
                asset.Dispose();
            }

            assets.Clear();
            invalidAssets.Clear();
        }
    }
}
