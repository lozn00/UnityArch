using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace XFramework
{
    public sealed class UIManager : CommonObject
    {
        private Dictionary<string, UI> allUIs = new Dictionary<string, UI>();

        private Dictionary<int, Transform> allLayers = new Dictionary<int, Transform>();

        protected override void Init()
        {
            Transform ui = Common.Instance.Get<Global>().UI;
            Reference reference = ui.Reference();
            Transform obj=reference.gameObject.transform.Find("Low");

            allLayers.Add((int)UILayers.Low, obj);// reference.GetChild<Transform>("Low"));
            allLayers.Add((int)UILayers.Mid, reference.GetChild<Transform>("Mid"));
            allLayers.Add((int)UILayers.High, reference.GetChild<Transform>("High"));
        }

        protected override void Destroy()
        {
            Clear();
            allLayers.Clear();
        }

        /// <summary>
        /// 创建UI并初始化
        /// </summary>
        /// <param name="info"></param>
        /// <param name="isFromPool"></param>
        /// <returns></returns>
        private UI CreateInner(string uiType, Transform parentObj, bool add)
        {
            Remove(uiType);

            UI ui = GetUIEventManager()?.OnCreate(uiType);
            if (ui is null)
                return null;

            GameObject obj = GetGameObject(ui, uiType, parentObj);
            ui.Init(obj, uiType);

            if (add)
                allUIs.Add(uiType, ui);

            return ui;
        }

        /// <summary>
        /// 根据UIInfo获取UI对象
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private GameObject GetGameObject(XObject parent, string uiType, Transform parentObj)
        {
            string key = GetUIEventManager()?.GetKeyWithUIType(uiType);
            if (key.IsNullOrEmpty())
                return null;

            GameObject obj = ResourcesManager.Instantiate(parent, key, parentObj, true);
            RectTransform rectTransform = obj?.GetComponent<RectTransform>();
            rectTransform?.SetAnchoredPositionZToZero();

            return obj;
        }

        private UIEventManager GetUIEventManager()
        {
            return Common.Instance.Get<UIEventManager>();
        }

        public UI Create(string uiType, Transform parentObj, bool awake = true)
        {
            UI ui = CreateInner(uiType, parentObj, false);
            if (awake)
                ObjectHelper.Awake(ui);

            return ui;
        }

        public UI Create<P1>(string uiType, P1 p1, Transform parentObj)
        {
            UI ui = CreateInner(uiType, parentObj, false);
            ObjectHelper.Awake(ui, p1);

            return ui;
        }

        public UI Create(string uiType, UILayers layer)
        {
            Transform parentObj = GetUILayer(layer, UILayers.Low);
            UI ui = CreateInner(uiType, parentObj, true);

            if (ui != null)
            {
                ui.SetLayer(layer);
            }

            ObjectHelper.Awake(ui);

            return ui;
        }

        public UI Create<P1>(string uiType, P1 p1, UILayers layer)
        {
            Transform parentObj = GetUILayer(layer, UILayers.Low);
            UI ui = CreateInner(uiType, parentObj, true);

            if (ui != null)
            {
                ui.SetLayer(layer);
            }

            ObjectHelper.Awake(ui, p1);

            return ui;
        }

        /// <summary>
        /// 获取UI层级对象
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public Transform GetUILayer(UILayers layer, UILayers defaultLayer = UILayers.Low)
        {
            allLayers.TryGetValue((int)layer, out Transform parent);
            return parent != null ? parent : allLayers[(int)defaultLayer];
        }

        /// <summary>
        /// 移除UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public bool Remove(string uiType)
        {
            if (allUIs.TryRemove(uiType, out UI child))
            {
                GetUIEventManager()?.OnRemove(child);
                child?.Dispose();
                return true;
            }

            return false;
        }

        /// <summary>
        /// 获取UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public UI Get(string uiType)
        {
            TryGet(uiType, out UI ui);
            return ui;
        }

        /// <summary>
        /// 尝试获取一个UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <param name="ui"></param>
        /// <returns></returns>
        public bool TryGet(string uiType, out UI ui)
        {
            return allUIs.TryGetValue(uiType, out ui);
        }

        /// <summary>
        /// 是否包含UI
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public bool Contain(string uiType)
        {
            return allUIs.ContainsKey(uiType);
        }

        /// <summary>
        /// 清除所有的UI
        /// </summary>
        public void Clear()
        {
            using var list = XList<string>.Create();
            list.AddRange(allUIs.Keys);
            foreach (var uiType in list)
            {
                Remove(uiType);
            }

            allUIs.Clear();
        }
    }
}
