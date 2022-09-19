using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XFramework
{
    public class UIStartTestList : UIChildrenList
    {
        public override void Initialize()
        {
            base.Initialize();
            this.InitList();
        }

        private void InitList()
        {
            foreach (var child in this.Children())
            {
                int index = child.GetSiblingIndex();

                void ClickEvent()
                {
                    Log.Debug($"点击了index为<color=red>{index}</color>的button, RootUI = <color=red>{child.RootUIType()}</color>, ParentUI = <color=red>{child.Parent.Name}</color>");
                }

                child.GetButton()?.AddClickListener(ClickEvent);
            }

            this[0].GetButton()?.ClickInvoke();
        }
    }
}
