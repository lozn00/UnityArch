using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public class Scene : XObject, IUpdate, ILateUpdate, IFixedUpdate
    {
        /// <summary>
        /// 场景名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 附加场景
        /// </summary>
        protected Dictionary<string, Scene> additiveScenes = new Dictionary<string, Scene>();

        public void Init(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// 要预先加载的资源
        /// </summary>
        /// <param name="assetsInfo"></param>
        public virtual void GetAssets(Dictionary<string, Type> assetsInfo)
        {

        }

        /// <summary>
        /// 要预先加载的场景对象
        /// </summary>
        /// <param name="objKeys"></param>
        public virtual void GetObjects(ICollection<string> objKeys)
        {

        }

        /// <summary>
        /// 添加附加场景
        /// </summary>
        /// <param name="scene"></param>
        public void AddAdditiveScene(Scene scene)
        {
            additiveScenes.Add(scene.Name, scene);
        }

        /// <summary>
        /// 移除附加场景
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool RemoveAdditiveScene(string name)
        {
            return additiveScenes.Remove(name);
        }

        public virtual void Update()
        {
            foreach (var additiveScene in additiveScenes.Values)
            {
                additiveScene.Update();
            }
        }

        public virtual void LateUpdate()
        {
            foreach (var additiveScene in additiveScenes.Values)
            {
                additiveScene.LateUpdate();
            }
        }

        public virtual void FixedUpdate()
        {
            foreach (var additiveScene in additiveScenes.Values)
            {
                additiveScene.FixedUpdate();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            foreach (var additiveScene in additiveScenes.Values)
            {
                additiveScene.Dispose();
            }
            additiveScenes.Clear();
        }
    }
}
