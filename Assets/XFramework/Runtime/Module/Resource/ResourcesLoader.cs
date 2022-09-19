using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Threading.Tasks;
using System.IO;

namespace XFramework
{
    public abstract class ResourcesLoader : IDisposable
    {
        public virtual void Init()
        {

        }

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract Object LoadAsset(string key, Type type);

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract T LoadAsset<T>(string key) where T : Object;

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract Task<T> LoadAssetAsync<T>(string key) where T : Object;

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public abstract Task<Object> LoadAssetAsync(string key, Type type);

        /// <summary>
        /// 同步加载并实例化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public abstract GameObject Instantiate(string path, Transform parent);

        /// <summary>
        /// 同步加载并实例化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public abstract GameObject Instantiate(string path, Transform parent, Vector3 position);

        /// <summary>
        /// 同步加载并实例化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="quaternion"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public abstract GameObject Instantiate(string path, Transform parent, Vector3 position, Quaternion quaternion);

        /// <summary>
        /// 异步加载并实例化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public abstract Task<GameObject> InstantiateAsync(string path, Transform parent);

        /// <summary>
        /// 异步加载并实例化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public abstract Task<GameObject> InstantiateAsync(string path, Transform parent, Vector3 position);

        /// <summary>
        /// 异步加载并实例化
        /// </summary>
        /// <param name="path"></param>
        /// <param name="parent"></param>
        /// <param name="position"></param>
        /// <param name="quaternion"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public abstract Task<GameObject> InstantiateAsync(string path, Transform parent, Vector3 position, Quaternion quaternion);

        /// <summary>
        /// 释放实例化对象
        /// </summary>
        /// <param name="obj"></param>
        public abstract void ReleaseInstance(GameObject obj);

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="asset"></param>
        public abstract void ReleaseAsset(Object asset);

        public virtual void Dispose()
        {

        }
    }
}
