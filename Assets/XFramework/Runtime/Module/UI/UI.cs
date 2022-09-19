using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public class UI : XObject
    {
        private GameObject gameObject;

        private Reference reference;

        private UI parent;

        /// <summary>
        /// 根UI的类型
        /// </summary>
        private string rootUIType;

        public GameObject GameObject => gameObject;

        public string Name { get; private set; }

        /// <summary>
        /// 层级
        /// </summary>
        public UILayers Layer { get; private set; }

        /// <summary>
        /// 对象引用组件
        /// </summary>
        public Reference RefComp
        {
            get
            {
                if (reference == null)
                    reference = gameObject.Reference();

                return reference;
            }
        }

        public UI Parent => parent;

        /// <summary>
        /// 子UI
        /// </summary>
        private Dictionary<string, UI> children = new Dictionary<string, UI>();

        /// <summary>
        /// 子组件
        /// </summary>
        private Dictionary<Type, UIComponent> uiComponents = new Dictionary<Type, UIComponent>();

        protected sealed override void OnStart() { }

        public void Init(GameObject obj, string name)
        {
            this.SetName(name);
            this.gameObject = obj;
            base.AddTarget();
        }

        public void SetName(string name)
        {
            if (this.Name == name)
                return;

            if (parent != null)
            {
                parent.children.Remove(this.Name);
                parent.children.Add(name, this);
            }

            this.Name = name;
        }

        private void RemoveFromChildren(UI child)
        {
            if (child == null)
                return;

            this.children.Remove(child.Name);
        }

        public void RemoveUIComponent(UIComponent component)
        {
            if (component == null)
                return;

            this.uiComponents.Remove(component.GetType());
        }

        /// <summary>
        /// 设置层级
        /// </summary>
        /// <param name="layer"></param>
        public void SetLayer(UILayers layer)
        {
            this.Layer = layer;
        }

        /// <summary>
        /// 设置UI的父类
        /// </summary>
        /// <param name="parent"></param>
        public void SetParent(UI parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// 根UI类型
        /// </summary>
        /// <returns></returns>
        public string RootUIType()
        {
            if (this.rootUIType.IsNullOrEmpty())
            {
                if (parent is null)
                    this.rootUIType = this.Name;
                else
                    this.rootUIType = parent.RootUIType();
            }

            return this.rootUIType;
        }

        /// <summary>
        /// 根UI
        /// </summary>
        /// <returns></returns>
        public UI RootUI()
        {
            string rootType = this.RootUIType();
            if (rootUIType.IsNullOrEmpty())
                return null;

            return Common.Instance.Get<UIManager>().Get(rootType);
        }

        public T GetParent<T>() where T : UI
        {
            return this.parent as T;
        }

        /// <summary>
        /// 获取引用组件里的对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UI GetFromReference(string key)
        {
            UI ui = this.GetChild(key);
            if (ui != null)
                return ui;

            GameObject obj = this.RefComp.GetChild(key);
            if (!obj)
                return null;

            ui = ObjectFactory.CreateNoInit<UI>(true);
            ui.Init(obj, key);
            this.AddChild(ui);

            return ui;
        }

        /// <summary>
        /// 创建一个子对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parentKey"></param>
        /// <returns></returns>
        public UI CreateChild(string key, string parentKey)
        {
            UI parentUI = this.GetFromReference(parentKey);
            if (parentUI is null)
                return null;

            Transform parent = parentUI.GameObject.transform;
            return this.CreateChild(key, parent);
        }

        /// <summary>
        /// 创建一个子对象，带参数
        /// </summary>
        /// <typeparam name="P1"></typeparam>
        /// <param name="key"></param>
        /// <param name="p1"></param>
        /// <param name="parentKey"></param>
        /// <returns></returns>
        public UI CreateChild<P1>(string key, P1 p1, string parentKey)
        {
            UI parentUI = this.GetFromReference(parentKey);
            if (parentUI is null)
                return null;

            Transform parent = parentUI.GameObject.transform;
            return this.CreateChild(key, p1, parent);
        }

        /// <summary>
        /// 创建一个子对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public UI CreateChild(string key, Transform parent)
        {
            UI ui = UIHelper.Create(key, parent, false);
            if (ui is null)
                return null;

            this.AddChild(ui);
            ObjectHelper.Awake(ui);

            return ui;
        }

        /// <summary>
        /// 创建一个子对象，带参数
        /// </summary>
        /// <typeparam name="P1"></typeparam>
        /// <param name="key"></param>
        /// <param name="p1"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public UI CreateChild<P1>(string key, P1 p1, Transform parent)
        {
            UI ui = UIHelper.Create(key, parent, false);
            if (ui is null)
                return null;

            this.AddChild(ui);
            ObjectHelper.Awake(ui, p1);

            return ui;
        }

        /// <summary>
        /// 添加一个UI组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="component"></param>
        public void AddUIComponent<T>(T component) where T : UIComponent, new()
        {
            Type type = typeof(T);
            if (!uiComponents.ContainsKey(type))
            {
                component.SetParent(this);
                uiComponents.Add(type, component);
            }
        }

        /// <summary>
        /// 获取一个UI组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUIComponent<T>() where T : UIComponent, new()
        {
            Type type = typeof(T);
            if (uiComponents.TryGetValue(type, out var comp))
                return comp as T;

            return null;
        }

        /// <summary>
        /// 获取自身的组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetComponent<T>() where T : Component
        {
            return gameObject.GetComponent<T>();
        }

        /// <summary>
        /// 获取子UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public UI GetChild(string uiType)
        {
            children.TryGetValue(uiType, out UI child);
            return child;
        }

        /// <summary>
        /// 获取子UI
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public T GetChild<T>(string uiType) where T : UI
        {
            children.TryGetValue(uiType, out UI child);
            return child as T;
        }

        /// <summary>
        /// 包含子UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public bool ContainChild(string uiType)
        {
            return children.ContainsKey(uiType);
        }

        /// <summary>
        /// 添加一个子UI
        /// </summary>
        /// <param name="ui"></param>
        public void AddChild(UI ui)
        {
            if (ui == this)
                return;

            if (!ContainChild(ui.Name))
            {
                ui.SetLayer(this.Layer);
                ui.SetParent(this);
                children.Add(ui.Name, ui);
            }
            else
            {
                Log.Error($"重复添加ChildUI, name = {ui.Name}, parent = {ui}");
                ui?.Dispose();
            }
        }

        /// <summary>
        /// 添加一个子UI，用不同的类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public T AddChild<T>(string key, bool isFromPool = false) where T : UI, new()
        {
            GameObject obj = this.RefComp.GetChild(key);
            if (!obj)
            {
                Log.Error($"未找到key为{key}的对象");
                return null;
            }

            T child = ObjectFactory.CreateNoInit<T>(isFromPool);
            child.Init(obj, key);
            this.AddChild(child);
            ObjectHelper.Awake(child);
            return child;
        }

        /// <summary>
        /// 添加一个子UI，带一个参数，用不同的类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <param name="key"></param>
        /// <param name="args"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        public T AddChild<T, P1>(string key, P1 args, bool isFromPool = false) where T : UI, new()
        {
            GameObject obj = this.RefComp.GetChild(key);
            if (!obj)
            {
                Log.Error($"未找到key为{key}的对象");
                return null;
            }

            T child = ObjectFactory.CreateNoInit<T>(isFromPool);
            child.Init(obj, key);
            this.AddChild(child);
            ObjectHelper.Awake(child, args);
            return child;
        }

        /// <summary>
        /// 移除一个子UI
        /// </summary>
        /// <param name="uiType"></param>
        public void RemoveChild(string uiType)
        {
            if (children.TryGetValue(uiType, out UI ui))
            {
                children.Remove(ui.Name);
                ui.Dispose();
            }
        }

        public int GetSiblingIndex()
        {
            return this.gameObject.transform.GetSiblingIndex();
        }

        public void SetSiblingIndex(int index)
        {
            this.gameObject.transform.SetSiblingIndex(index);
        }

        public void SetAsLastSibling()
        {
            this.gameObject.transform.SetAsLastSibling();
        }

        public void SetAsFirstSibling()
        {
            this.gameObject.transform.SetAsFirstSibling();
        }

        /// <summary>
        /// 关闭本界面
        /// </summary>
        protected void Close()
        {
            if (parent is null)
                UIHelper.Remove(Name);
            else
                Dispose();
        }

        /// <summary>
        /// 关闭时调用
        /// </summary>
        protected virtual void OnClose()
        {

        }

        protected sealed override void OnDestroy()
        {
            foreach (UIComponent comp in uiComponents.Values)
            {
                comp.Dispose();
            }
            uiComponents.Clear();

            foreach (UI child in children.Values)
            {
                child.Dispose();
            }
            children.Clear();

            OnClose();

            if (parent != null && !parent.IsDisposed)
                parent.RemoveFromChildren(this);

            parent = null;
            Name = null;
            rootUIType = null;
            reference = null;
            gameObject = null;
        }
    }
}
