using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.EventSystems;

namespace XFramework
{
    public interface IUIBehaviour
    {
        UIBehaviour UIBehaviour { get; }
    }

    public abstract class UIComponent : XObject, IUIBehaviour
    {
        public abstract UIBehaviour UIBehaviour { get; }

        public UI Parent { get; private set; }

        public RectTransformComponent RectTransform => Parent?.GetRectTransform();

        internal void SetParent(UI parent)
        {
            this.Parent = parent;
        }

        protected override void OnDestroy()
        {
            if (Parent != null && !Parent.IsDisposed)
            {
                Parent.RemoveUIComponent(this);
            }

            Parent = null;

            base.OnDestroy();
        }
    }
}
