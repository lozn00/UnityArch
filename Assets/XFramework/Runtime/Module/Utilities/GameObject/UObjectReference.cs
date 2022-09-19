using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace XFramework
{
    public class UObjectReference : XObject, IAwake<GameObject>, IAwake<Transform>, IAwake<GameObject, string>, IAwake<Transform, string>, IAwake<GameObject, bool>, IAwake<Transform, bool>, IAwake<GameObject, string, bool>, IAwake<Transform, string, bool>
    {
        /// <summary>
        /// ���ڲ����Ӷ����ǰ׺
        /// </summary>
        private string prefix;

        /// <summary>
        /// �ͷź�������еĵ���¼�
        /// </summary>
        private bool removeAllListener;

        /// <summary>
        /// Ĭ��ǰ׺
        /// </summary>
        private const string defaultPrefix = "rd_";

        private GameObject gameObject;
        public GameObject GameObject
        {
            get { return gameObject; }
            set
            {
                if (value != gameObject) 
                {
                    Clear();
                    gameObject = value;
                    if (gameObject != null)
                        gameObject.FindChildrenWithPrefix(prefix, references);
                }
            }
        }

        /// <summary>
        /// ��ͷΪprefix�Ķ���
        /// </summary>
        private Dictionary<string, GameObject> references = new Dictionary<string, GameObject>();

        /// <summary>
        /// gameObject���ϵ����
        /// </summary>
        private UnOrderMap<string, Type, Component> components = new UnOrderMap<string, Type, Component>();

        public void Initialize(GameObject obj)
        {
            Initialize(obj, defaultPrefix, true);
        }

        public void Initialize(Transform trans)
        {
            Initialize(trans, defaultPrefix, true);
        }

        public void Initialize(GameObject obj, string prefix)
        {
            Initialize(obj, prefix, true);
        }

        public void Initialize(Transform trans, string prefix)
        {
            Initialize(trans, prefix, true);
        }

        public void Initialize(GameObject obj, bool remove)
        {
            Initialize(obj, defaultPrefix, remove);
        }

        public void Initialize(Transform trans, bool remove)
        {
            Initialize(trans, defaultPrefix, remove);
        }

        public void Initialize(GameObject obj, string prefix, bool remove)
        {
            this.removeAllListener = remove;
            this.prefix = prefix;
            this.GameObject = obj;
        }

        public void Initialize(Transform trans, string prefix, bool remove)
        {
            this.removeAllListener = remove;
            this.prefix = prefix;
            this.GameObject = trans.gameObject;
        }

        /// <summary>
        /// ���Ի�ȡ��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGet<T>(out T component) where T : Component
        {
            component = Get<T>();
            return component != null;
        }

        /// <summary>
        /// ���Ի�ȡ��������
        /// </summary>
        /// <param name="type"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGet(Type type, out Component component)
        {
            component = Get(type);
            return component != null;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : Component
        {
            return Get(typeof(T)) as T;
        }

        /// <summary>
        /// ��ȡ��������
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Component Get(Type type)
        {
            string name = this.gameObject.name;
            if (components.TryGetValue(name, type, out Component component))
                return component;

            component = this.gameObject.GetComponent(type);
            if (component != null)
                components.Add(name, type, component);

            return component;
        }

        /// <summary>
        /// ���Ի�ȡ�Ӷ���
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool TryGetChild(string key, out GameObject obj)
        {
            obj = GetChild(key);
            return obj != null;
        }

        /// <summary>
        /// ��ȡ�Ӷ���
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public GameObject GetChild(string key)
        {
            if (references.TryGetValue(key, out GameObject obj))
                return obj;

            return null;
        }

        /// <summary>
        /// ���Դ�ĳ���Ӷ����ϻ�ȡһ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGetChild<T>(string key, out T component) where T : Component
        {
            component = GetChild<T>(key);
            return component != null;
        }

        /// <summary>
        /// ���Դ�ĳ���Ӷ����ϻ�ȡһ�����
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <param name="component"></param>
        /// <returns></returns>
        public bool TryGetChild(string key, Type type, out Component component)
        {
            component = GetChild(type, key);
            return component != null;
        }

        /// <summary>
        /// ��ĳ���Ӷ����ϻ�ȡһ�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetChild<T>(string key) where T : Component
        {
            Type type = typeof(T);

            return GetChild(type, key) as T;
        }

        /// <summary>
        /// ��ĳ���Ӷ����ϻ�ȡһ�����
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Component GetChild(Type type, string key)
        {
            if (components.TryGetValue(key, type, out Component o))
                return o;

            o = GetChild(key).GetComponent(type);
            if (o != null)
                components.Add(key, type, o);

            return o;
        }

        /// <summary>
        /// �Ƴ�Button��Toggle�������¼�����
        /// </summary>
        /// <param name="type"></param>
        public void RemoveAllListeners()
        {
            RemoveButtonListener<Button>();
            RemoveButtonListener<XButton>();
            RemoveToggleListener<Toggle>();
            RemoveToggleListener<XToggle>();
        }

        /// <summary>
        /// �Ƴ�Button�������¼�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void RemoveButtonListener<T>() where T : Button
        {
            Type type = typeof(T);
            foreach (var item in components.Values)
            {
                if (item.TryGetValue(type, out var component))
                {
                    if (component == null)
                        continue;

                    if (component is T btn)
                    {
                        btn.onClick.RemoveAllListeners();
                        if (component is XButton xBtn)
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// �Ƴ�Toggle�������¼�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private void RemoveToggleListener<T>() where T : Toggle
        {
            Type type = typeof(T);
            foreach (var item in components.Values)
            {
                if (item.TryGetValue(type, out var component))
                {
                    if (component == null)
                        continue;

                    if (component is T toggle)
                    {
                        toggle.onValueChanged.RemoveAllListeners();
                        if (component is XToggle xToggle)
                        {

                        }
                    }
                }
            }
        }

        private void Clear()
        {
            if (removeAllListener)
                RemoveAllListeners();

            references.Clear();
            components.Clear();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameObject = null;
            removeAllListener = true;
        }
    }
}
