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
    public class ToggleComponent : UIComponent, IAwake<Toggle>
    {
        public override UIBehaviour UIBehaviour => toggle;

        private Toggle toggle;

        public virtual void Initialize(Toggle toggle)
        {
            this.toggle = toggle;
        }

        protected override void OnDestroy()
        {
            this.RemoveAllClickListeners();
            this.toggle = null;
            base.OnDestroy();
        }

        public Toggle Get()
        {
            return this.toggle;
        }

        public void AddClickListener(UnityAction<bool> action)
        {
            this.toggle.AddClickListener(action);
        }

        public void RemoveClickListener(UnityAction<bool> action)
        {
            this.toggle.RemoveClickListener(action);
        }

        public void RemoveAllClickListeners()
        {
            this.toggle.RemoveAllClickListeners();
        }
    }

    public static class UIToggleExtensions
    {
        public static ToggleComponent GetToggle(this UI self)
        {
            ToggleComponent comp = self.GetUIComponent<ToggleComponent>();
            if (comp != null)
                return comp;

            Toggle toggle = self.GetComponent<Toggle>();
            if (!toggle)
                return null;

            comp = ObjectFactory.Create<ToggleComponent, Toggle>(toggle, true);
            self.AddUIComponent(comp);

            return comp;
        }

        public static ToggleComponent GetToggle(this UI self, string key)
        {
            UI ui = self.GetFromReference(key);
            return ui?.GetToggle();
        }

        public static void AddClickListener(this Toggle self, UnityAction<bool> action)
        {
            if (action == null)
                return;

            self.onValueChanged.AddListener(action);
        }

        public static void RemoveClickListener(this Toggle self, UnityAction<bool> action)
        {
            self.onValueChanged.RemoveListener(action);
        }

        public static void RemoveAllClickListeners(this Toggle self)
        {
            self.onValueChanged.RemoveAllListeners();
        }
    }
}
