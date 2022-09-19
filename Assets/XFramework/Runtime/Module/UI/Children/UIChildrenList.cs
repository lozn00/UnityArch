using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace XFramework
{
    public class UIChildrenList : UI, IAwake
    {
        private Dictionary<int, string> childrenList = new Dictionary<int, string>();

        public virtual UIChild this[int id] => this.GetChild(id);

        public virtual void Initialize()
        {
            this.RefreshChildrenList();
        }

        protected override void OnClose()
        {
            this.childrenList.Clear();
            base.OnClose();
        }

        public void RemoveFormChildrenList(int id)
        {
            this.childrenList.Remove(id);
        }

        /// <summary>
        /// 列表里所有的子对象
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<UIChild> Children()
        {
            foreach (var name in this.childrenList.Values)
            {
                yield return base.GetChild<UIChild>(name);
            }
        }

        /// <summary>
        /// 刷新所有子对象
        /// </summary>
        private void RefreshChildrenList()
        {
            this.childrenList.Clear();
            foreach (Transform child in this.GameObject.transform)
            {
                int index = child.GetSiblingIndex();
                UIChild ui = ObjectFactory.CreateNoInit<UIChild>(true);
                string name = this.Name + index;
                ui.Init(child.gameObject, name);
                ui.SetId(index);
                this.AddChildToList(index, ui);
            }
        }

        /// <summary>
        /// 添加子对象到列表
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ui"></param>
        private void AddChildToList(int id, UIChild ui)
        {
            if (!this.childrenList.ContainsKey(id))
            {
                this.childrenList.Add(id, ui.Name);
                base.AddChild(ui);
            }
            else
            {
                Log.Error($"重复添加ChildUI, name = {ui.Name}, parent = {ui}");
            }
        }

        /// <summary>
        /// 从列表里获取子对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UIChild GetChild(int id)
        {
            this.childrenList.TryGetValue(id, out string name);
            if (name.IsNullOrEmpty())
                return null;

            return base.GetChild<UIChild>(name);
        }

        /// <summary>
        /// 从列表里获取子对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T GetChild<T>(int id) where T : UIChild
        {
            return this.GetChild(id) as T;
        }

        /// <summary>
        /// 从列表里移除子对象
        /// </summary>
        /// <param name="id"></param>
        public void RemoveChild(int id)
        {
            UI child = this.GetChild(id);
            this.childrenList.Remove(id);
            child?.Dispose();
        }

        /// <summary>
        /// 设置列表里某子对象的索引，同时会将其子对象的id改为index
        /// </summary>
        /// <param name="id"></param>
        /// <param name="index"></param>
        /// <param name="changeId"></param>
        public void SetSiblingIndex(int id, int index)
        {
            if (id == index)
                return;

            UIChild child = this.GetChild(id);
            if (child != null)
                return;

            UIChild ui = this.GetChild(index);
            if (ui != null)
            {
                this.childrenList[id] = ui.Name;
                ui.SetId(id);
            }
            else
            {
                this.RemoveFormChildrenList(id);
            }

            this.childrenList[index] = child.Name;
            child.SetId(index);
            child.SetSiblingIndex(index);
        }

        /// <summary>
        /// 创建一个子对象到列表里，id默认为其索引号
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public T CreateChild<T>(string uiType) where T : UIChild
        {
            int index = this.GameObject.transform.childCount;
            return this.CreateChild<T>(index, uiType);
        }

        /// <summary>
        /// 创建一个子对象到列表里，id默认为其索引号，带一个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <param name="uiType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T CreateChild<T, P1>(string uiType, P1 args) where T : UIChild
        {
            int index = this.GameObject.transform.childCount;
            return this.CreateChild<T, P1>(index, uiType, args);
        }

        /// <summary>
        /// 创建一个子对象到列表里，并指定一个id，之后可以通过id得到它
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public T CreateChild<T>(int id, string uiType) where T : UIChild
        {
            UIChild ui = UIHelper.Create(uiType, this.GameObject.transform, false) as UIChild;
            if (ui is null)
                return null;

            string name = this.Name + id;
            ui.SetName(name);
            ui.SetId(id);
            this.AddChildToList(id, ui);
            ObjectHelper.Awake(ui);

            return ui as T;
        }

        /// <summary>
        /// 创建一个子对象到列表里，并指定一个id，之后可以通过id得到它，带一个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="P1"></typeparam>
        /// <param name="id"></param>
        /// <param name="uiType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public T CreateChild<T, P1>(int id, string uiType, P1 args) where T : UIChild
        {
            UIChild ui = UIHelper.Create(uiType, this.GameObject.transform, false) as  UIChild;
            if (ui is null)
                return null;

            string name = this.Name + id;
            ui.SetName(name);
            ui.SetId(id);
            this.AddChildToList(id, ui);
            ObjectHelper.Awake(ui, args);

            return ui as T;
        }
    }
}
