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
    public class ImageComponent : GraphicComponent, IAwake<Image>
    {
        protected override Graphic Graphic => img;

        private Image img;

        public virtual void Initialize(Image img)
        {
            this.img = img;
        }

        protected override void OnDestroy()
        {
            this.img = null;
            base.OnDestroy();
        }

        public Image Get()
        {
            return this.img;
        }

        public void SetSprite(Sprite sprite, bool setNativeSize)
        {
            this.Get().SetSprite(sprite, setNativeSize);
        }

        public async Task SetSprite(string key, bool setNativeSize)
        {
            await this.Get().SetSprite(this, key, setNativeSize);
        }

        public void SetNativeSize()
        {
            this.Get().SetNativeSize();
        }

        public void SetFillAmount(float fillAmount)
        {
            this.Get().fillAmount = fillAmount;
        }

        public float GetFillAmount()
        {
            return this.Get().fillAmount;
        }

        public MiniTween DoFillAmount(float endValue, float duration)
        {
            return this.Get().DoFillAmount(this, endValue, duration);
        }

        public MiniTween DoFillAmount(float startValue, float endValue, float duration)
        {
            return this.Get().DoFillAmount(this, startValue, endValue, duration);
        }
    }

    public static class UIImageExtensions
    {
        public static ImageComponent GetImage(this UI self)
        {
            ImageComponent comp = self.GetUIComponent<ImageComponent>();
            if (comp != null)
                return comp;

            Image img = self.GetComponent<Image>();
            if (!img)
                return null;

            comp = ObjectFactory.Create<ImageComponent, Image>(img, true);
            self.AddUIComponent(comp);

            return comp;
        }

        public static ImageComponent GetImage(this UI self, string key)
        {
            UI ui = self.GetFromReference(key);
            return ui?.GetImage();
        }
        
        public static void SetSprite(this Image self, Sprite sprite, bool setNativeSize)
        {
            self.sprite = sprite;
            if (setNativeSize)
                self.SetNativeSize();
        }

        public async static Task SetSprite(this Image self, XObject parent, string key, bool setNativeSize)
        {
            Sprite sprite = await ResourcesManager.LoadAssetAsync<Sprite>(parent, key);
            if (sprite == null)
                return;

            self.SetSprite(sprite, setNativeSize);
        }

        public static void SetFillAmount(this Image self, float fillAmount)
        {
            self.fillAmount = fillAmount;
        }

        #region MiniTween

        public static MiniTween DoFillAmount(this Image self, XObject parent, float endValue, float duration)
        {
            return self.DoFillAmount(parent, self.fillAmount, endValue, duration);
        }

        public static MiniTween DoFillAmount(this Image self, XObject parent, float startValue, float endValue, float duration)
        {
            return self.DoFloat(parent, startValue, endValue, duration, self.SetFillAmount);
        }

        #endregion
    }
}
