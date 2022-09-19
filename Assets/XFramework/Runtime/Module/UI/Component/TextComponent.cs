using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace XFramework
{
    public class TextComponent : GraphicComponent, IAwake<Text>
    {
        protected override Graphic Graphic => textComp;

        private Text textComp;

        public string Content => textComp.text;

        public virtual void Initialize(Text t)
        {
            this.textComp = t;
        }

        protected override void OnDestroy()
        {
            this.textComp = null;
            base.OnDestroy();
        }

        public Text Get()
        {
            return this.textComp;
        }

        public void SetText(string content)
        {
            this.Get().SetText(content);
        }

        public void SetText(string key, params object[] args)
        {
            this.Get().SetText(key, args);
        }

        public void SetCount(long count)
        {
            this.Get().SetCount(count);
        }

        public void SetNumber(long number)
        {
            this.Get().SetNumber(number);
        }

        public void SetFontSize(int size)
        {
            this.Get().SetFontSize(size);
        }

        public MiniTween DoCount(long endValue, float duration)
        {
            return this.Get().DoCount(this, endValue, duration);
        }

        public MiniTween DoCount(long startValue, long endValue, float duration)
        {
            return this.Get().DoCount(this, startValue, endValue, duration);
        }

        public MiniTween DoNumber(long endValue, float duration)
        {
            return this.Get().DoNumber(this, endValue, duration);
        }

        public MiniTween DoNumber(long startValue, long endValue, float duration)
        {
            return this.Get().DoNumber(this, startValue, endValue, duration);
        }
    }

    public static class UITextExtensions
    {
        public static TextComponent GetText(this UI self)
        {
            TextComponent comp = self.GetUIComponent<TextComponent>();
            if (comp != null)
                return comp;

            Text textComp = self.GetComponent<Text>();
            if (!textComp)
                return null;

            comp = ObjectFactory.Create<TextComponent, Text>(textComp, true);
            self.AddUIComponent(comp);

            return comp;
        }

        public static TextComponent GetText(this UI self, string key)
        {
            UI ui = self.GetFromReference(key);
            return ui?.GetText();
        }

        public static void SetText(this Text self, string content)
        {
            self.text = content;
        }

        public static void SetText(this Text self, string key, params object[] args)
        {
            self.text = string.Format(key, args);
        }

        public static void SetCount(this Text self, long count)
        {
            self.text = count.ToString();
        }

        public static void SetNumber(this Text self, long number)
        {
            self.text = number.ToString();
        }

        public static void SetFontSize(this Text self, int size)
        {
            self.fontSize = size;
        }

        #region MiniTween

        public static MiniTween DoCount(this Text self, XObject parent, long endValue, float duration)
        {
            long.TryParse(self.text, out long startValue);
            return self.DoCount(parent, startValue, endValue, duration);
        }

        public static MiniTween DoCount(this Text self, XObject parent, long startValue, long endValue, float duration)
        {
            return self.DoLong(parent, startValue, endValue, duration, self.SetCount);
        }

        public static MiniTween DoNumber(this Text self, XObject parent, long endValue, float duration)
        {
            long.TryParse(self.text, out long startValue);
            return self.DoNumber(parent, startValue, endValue, duration);
        }

        public static MiniTween DoNumber(this Text self, XObject parent, long startValue, long endValue, float duration)
        {
            return self.DoLong(parent, startValue, endValue, duration, self.SetNumber);
        }

        private static MiniTween DoLong(this Text self, XObject parent, long startValue, long endValue, float duration, Action<long> setValue)
        {
            var tweenMgr = Common.Instance.Get<MiniTweenManager>();
            if (tweenMgr is null)
                return null;

            var tween = tweenMgr.To(parent, startValue, endValue, duration);
            tween.AddListener(n =>
            {
                if (!self)
                {
                    tween.Cancel();
                    return;
                }

                setValue.Invoke(n);
            });

            return tween;
        }

        #endregion
    }
}
