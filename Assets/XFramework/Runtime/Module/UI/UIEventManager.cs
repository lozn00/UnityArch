using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFramework
{
    public sealed class UIEventManager : CommonObject
    {
        private Dictionary<string, AUIEvent> uiEvents = new Dictionary<string, AUIEvent>();

        protected override void Init()
        {
            var types = TypesManager.Instance.GetTypes(typeof(UIEventAttribute));
            foreach (var type in types)
            {
                AUIEvent aUIEvent = Activator.CreateInstance(type) as AUIEvent;
                if (aUIEvent == null)
                    continue;

                var attris = type.GetCustomAttributes(typeof(UIEventAttribute), false);
                if (attris.Length == 0)
                    continue;

                foreach (UIEventAttribute attri in attris)
                {
                    uiEvents.Add(attri.UIType, aUIEvent);
                }
            }
        }

        protected override void Destroy()
        {
            uiEvents.Clear();
        }

        /// <summary>
        /// 通过UIType获取一个Key，这个Key要确保能创建出GameObject
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public string GetKeyWithUIType(string uiType)
        {
            try
            {
                return uiEvents[uiType].GetKey();
            }
            catch (Exception e)
            {
                Log.Error($"CreateUI error!, UIType = {uiType}\n{e}");
                return null;
            }
        }

        /// <summary>
        /// 创建UI时执行
        /// </summary>
        /// <param name="uiType"></param>
        public UI OnCreate(string uiType)
        {
            try
            {
                return uiEvents[uiType].OnCreate();
            }
            catch (Exception e)
            {
                Log.Error($"CreateUI error!, UIType = {uiType}\n{e}");
                return null;
            }
        }

        /// <summary>
        /// 移除UI时执行
        /// </summary>
        /// <param name="uiType"></param>
        public void OnRemove(UI ui)
        {
            string uiType = ui.Name;
            try
            {
                uiEvents[uiType].OnRemove(ui);
            }
            catch (Exception e)
            {
                Log.Error($"RemoveUI error!, UIType = {uiType}\n{e}");
            }
        }
    }
}
