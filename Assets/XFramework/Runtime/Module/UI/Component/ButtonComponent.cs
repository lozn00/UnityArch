using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XFramework
{
    public class ButtonComponent : UIComponent, IAwake<Button>
    {
        public override UIBehaviour UIBehaviour => btn;

        private Button btn;

        public virtual void Initialize(Button button)
        {
            this.btn = button;
        }

        protected override void OnDestroy()
        {
            this.RemoveAllClickListeners();
            this.btn = null;
            base.OnDestroy();
        }

        public Button Get()
        {
            return this.btn;
        }

        public void AddClickListener(UnityAction action)
        {
            this.btn.AddClickListener(action);
        }

        public void RemoveClickListener(UnityAction action)
        {
            this.Get().RemoveClickListener(action);
        }

        public void RemoveAllClickListeners()
        {
            this.Get().RemoveAllClickListeners();
        }

        /// <summary>
        /// 直接触发点击事件
        /// </summary>
        public void ClickInvoke()
        {
            this.Get().ClickInvoke();
        }
    }

    public static class UIButtonExtensions
    {
        public static ButtonComponent GetButton(this UI self)
        {
            ButtonComponent comp = self.GetUIComponent<ButtonComponent>();
            if (comp != null)
                return comp;

            Button btn = self.GetComponent<Button>();
            if (!btn)
                return null;

            comp = ObjectFactory.Create<ButtonComponent, Button>(btn, true);
            self.AddUIComponent(comp);

            return comp;
        }

        public static ButtonComponent GetButton(this UI self, string key)
        {
            UI ui = self.GetFromReference(key);
            return ui?.GetButton();
        }

        public static void AddClickListener(this Button self, UnityAction action)
        {
            if (action == null)
                return;

            self.onClick.AddListener(action);
        }

        public static void RemoveClickListener(this Button self, UnityAction action)
        {
            self.onClick.RemoveListener(action);
        }

        public static void RemoveAllClickListeners(this Button self)
        {
            self.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 直接触发点击事件
        /// </summary>
        /// <param name="self"></param>
        public static void ClickInvoke(this Button self)
        {
            self.onClick.Invoke();
        }
    }
}
