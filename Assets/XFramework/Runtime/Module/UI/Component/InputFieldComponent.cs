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
    public class InputFieldComponent : UIComponent, IAwake<InputField>
    {
        public override UIBehaviour UIBehaviour => input;

        private InputField input;

        public string Content => this.input.text;

        public virtual void Initialize(InputField input)
        {
            this.input = input;
        }

        protected override void OnDestroy()
        {
            this.RemoveAllValueChangedListeners();
            this.RemoveAllEndEditListeners();
            this.input = null;
            base.OnDestroy();
        }

        public InputField Get()
        {
            return this.input;
        }

        public void SetText(string value)
        {
            this.input.SetText(value);
        }

        public void SetText(string key, params object[] args)
        {
            this.input.SetText(key, args);
        }

        public void AddValueChangedListener(UnityAction<string> action)
        {
            this.input.AddValueChangedListener(action);
        }

        public void RemoveValueChangedListener(UnityAction<string> action)
        {
            this.input.RemoveValueChangedListener(action);
        }

        public void RemoveAllValueChangedListeners()
        {
            this.input.RemoveAllValueChangedListeners();
        }

        public void AddEndEditListener(UnityAction<string> action)
        {
            this.input.AddEndEditListener(action);
        }

        public void RemoveEndEditListener(UnityAction<string> action)
        {
            this.input.RemoveEndEditListener(action);
        }

        public void RemoveAllEndEditListeners()
        {
            this.input.RemoveAllEndEditListeners();
        }
    }

    public static class UIInputFieldExtensions
    {
        public static InputFieldComponent GetInputField(this UI self)
        {
            InputFieldComponent comp = self.GetUIComponent<InputFieldComponent>();
            if (comp != null)
                return comp;

            InputField input = self.GetComponent<InputField>();
            if (!input)
                return null;

            comp = ObjectFactory.Create<InputFieldComponent, InputField>(input, true);
            self.AddUIComponent(comp);

            return comp;
        }

        public static InputFieldComponent GetInputField(this UI self, string key)
        {
            UI ui = self.GetFromReference(key);
            return ui?.GetInputField();
        }

        public static void AddValueChangedListener(this InputField self, UnityAction<string> action)
        {
            if (action == null)
                return;

            self.onValueChanged.AddListener(action);
        }

        public static void RemoveValueChangedListener(this InputField self, UnityAction<string> action)
        {
            self.onValueChanged.RemoveListener(action);
        }

        public static void RemoveAllValueChangedListeners(this InputField self)
        {
            self.onValueChanged.RemoveAllListeners();
        }

        public static void AddEndEditListener(this InputField self, UnityAction<string> action)
        {
            if (action == null)
                return;

            self.onEndEdit.AddListener(action);
        }

        public static void RemoveEndEditListener(this InputField self, UnityAction<string> action)
        {
            self.onEndEdit.RemoveListener(action);
        }

        public static void RemoveAllEndEditListeners(this InputField self)
        {
            self.onEndEdit.RemoveAllListeners();
        }

        public static void SetText(this InputField self, string value)
        {
            self.text = value;
        }

        public static void SetText(this InputField self, string key, params object[] args)
        {
            self.text = string.Format(key, args);
        }
    }
}
