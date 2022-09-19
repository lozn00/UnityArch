using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace XFramework
{
    public sealed class ResourcesManager : Singleton<ResourcesManager>, IDisposable
    {
        public ResourcesLoader Loader { get; private set; }

        public ResourcesManager()
        {
            if (Instance is null) 
                Instance = this;
        }

        public void Init()
        {

        }

        public void SetLoader(ResourcesLoader loader)
        {
            Loader = loader;
            loader?.Init();
        }

        private static bool GetResourcesRefDetection(out ResourcesRefDetection refDetection)
        {
            refDetection = Common.Instance?.Get<ResourcesRefDetection>();
            return refDetection != null;
        }

        /// <summary>
        /// ͬ��������Դ
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Object LoadAsset(XObject parent, string key, Type type)
        {
            if (Instance is null)
                return null;

            if (!GetResourcesRefDetection(out var refDetection))
                return null;

            Object asset = Instance.Loader.LoadAsset(key, type);
            refDetection.AddAsset(parent, asset);
            return asset;
        }

        /// <summary>
        /// ͬ��������Դ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T LoadAsset<T>(XObject parent, string key) where T : Object
        {
            if (Instance is null)
                return null;

            if (!GetResourcesRefDetection(out var refDetection))
                return null;

            T asset = Instance.Loader.LoadAsset<T>(key);
            refDetection.AddAsset(parent, asset);
            return asset;
        }

        /// <summary>
        /// �첽������Դ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public async static Task<T> LoadAssetAsync<T>(XObject parent, string key) where T : Object
        {
            if (Instance is null)
                return null;

            if (!GetResourcesRefDetection(out var refDetection))
                return null;

            long tagId = parent.TagId;
            T asset = await Instance.Loader.LoadAssetAsync<T>(key);
            if (tagId != parent.TagId)
            {
                Instance.Loader.ReleaseAsset(asset);
                return null;
            }

            refDetection.AddAsset(parent, asset);
            return asset;
        }

        /// <summary>
        /// �첽������Դ
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async static Task<Object> LoadAssetAsync(XObject parent, string key, Type type)
        {
            if (Instance is null)
                return null;

            if (!GetResourcesRefDetection(out var refDetection))
                return null;

            long tagId = parent.TagId;
            Object asset = await Instance.Loader.LoadAssetAsync(key, type);
            if (tagId != parent.TagId)
            {
                Instance.Loader.ReleaseAsset(asset);
                return null;
            }

            refDetection.AddAsset(parent, asset);
            return asset;
        }

        /// <summary>
        /// ͬ�����ز�ʵ����
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="parentObj"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public static GameObject Instantiate(XObject parent, string key, Transform parentObj, bool isFromPool = false)
        {
            return Instantiate(parent, key, parentObj, Vector3.zero, isFromPool);
        }

        /// <summary>
        /// ͬ�����ز�ʵ����
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="parentObj"></param>
        /// <param name="position"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public static GameObject Instantiate(XObject parent, string key, Transform parentObj, Vector3 position, bool isFromPool = false)
        {
            return Instantiate(parent, key, parentObj, position, Quaternion.identity, isFromPool);
        }

        /// <summary>
        /// ͬ�����ز�ʵ����
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="parentObj"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public static GameObject Instantiate(XObject parent, string key, Transform parentObj, Vector3 position, Quaternion rotation, bool isFromPool = false)
        {
            if (Instance is null)
                return null;

            if (!GetResourcesRefDetection(out var refDetection))
                return null;

            GameObject obj = null;
            if (isFromPool)
                obj = Instance.GetGameObjectFromPool(key, parentObj, position, rotation);

            if (!obj)
                obj = Instance.Loader.Instantiate(key, parentObj, position, rotation);

            refDetection.AddInstantiate(parent, obj, key, isFromPool);
            return obj;
        }

        /// <summary>
        /// �첽���ز�ʵ����
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="parentObj"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public async static Task<GameObject> InstantiateAsync(XObject parent, string key, Transform parentObj, bool isFromPool = false)
        {
            return await InstantiateAsync(parent, key, parentObj, Vector3.zero, isFromPool);
        }

        /// <summary>
        /// �첽���ز�ʵ����
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="parentObj"></param>
        /// <param name="position"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public async static Task<GameObject> InstantiateAsync(XObject parent, string key, Transform parentObj, Vector3 position, bool isFromPool = false)
        {
            return await InstantiateAsync(parent, key, parentObj, position, Quaternion.identity, isFromPool);
        }

        /// <summary>
        /// �첽���ز�ʵ����
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="key"></param>
        /// <param name="parentObj"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public async static Task<GameObject> InstantiateAsync(XObject parent, string key, Transform parentObj, Vector3 position, Quaternion rotation, bool isFromPool = false)
        {
            if (Instance is null)
                return null;

            if (!GetResourcesRefDetection(out var refDetection))
                return null;

            long tagId = parent.TagId;
            GameObject obj = null;
            if (isFromPool)
                obj = Instance.GetGameObjectFromPool(key, parentObj, position, rotation);

            if (!obj)
                obj = await Instance.Loader.InstantiateAsync(key, parentObj, position, rotation);

            if (tagId != parent.TagId)
            {
                Instance.Loader.ReleaseInstance(obj);
                return null;
            }

            refDetection.AddInstantiate(parent, obj, key, isFromPool);
            return obj;
        }

        /// <summary>
        /// �ͷ�ʵ��������
        /// </summary>
        /// <param name="obj"></param>
        public static void ReleaseInstance(GameObject obj)
        {
            if (!obj)
                return;

            if (Instance is null)
                return;

            if (!GetResourcesRefDetection(out var refDetection))
                return;

            refDetection.Remove(obj.GetInstanceID());
        }

        /// <summary>
        /// �ͷ���Դ
        /// </summary>
        /// <param name="asset"></param>
        public static void ReleaseAsset(Object asset)
        {
            if (Instance is null)
                return;

            if (!GetResourcesRefDetection(out var refDetection))
                return;

            refDetection.Remove(asset.GetInstanceID());
        }

        /// <summary>
        /// �Ӷ��������GameObject
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        private GameObject GetGameObjectFromPool(string key, Transform parent, Vector3 position, Quaternion rotation)
        {
            GameObject obj = GameObjectPool.Instance.Fetch(key);
            if (obj)
            {
                Transform trans = obj.transform;
                trans.SetParent(parent, false);
                trans.position = position;
                trans.rotation = rotation;
            }

            return obj;
        }

        public void Dispose()
        {
            Loader?.Dispose();
            Loader = null;
            Instance = null;
        }
    }
}
