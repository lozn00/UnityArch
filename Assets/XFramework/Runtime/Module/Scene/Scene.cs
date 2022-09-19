using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public class Scene : XObject, IUpdate, ILateUpdate, IFixedUpdate
    {
        /// <summary>
        /// ��������
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// ���ӳ���
        /// </summary>
        protected Dictionary<string, Scene> additiveScenes = new Dictionary<string, Scene>();

        public void Init(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// ҪԤ�ȼ��ص���Դ
        /// </summary>
        /// <param name="assetsInfo"></param>
        public virtual void GetAssets(Dictionary<string, Type> assetsInfo)
        {

        }

        /// <summary>
        /// ҪԤ�ȼ��صĳ�������
        /// </summary>
        /// <param name="objKeys"></param>
        public virtual void GetObjects(ICollection<string> objKeys)
        {

        }

        /// <summary>
        /// ��Ӹ��ӳ���
        /// </summary>
        /// <param name="scene"></param>
        public void AddAdditiveScene(Scene scene)
        {
            additiveScenes.Add(scene.Name, scene);
        }

        /// <summary>
        /// �Ƴ����ӳ���
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
