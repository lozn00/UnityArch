using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static XFramework.UISetting;

namespace XFramework
{
	[UIEvent(UIType.UISetting)]
    internal sealed class UISettingEvent : AUIEvent
    {
        public override string GetKey()
        {
            return UIPathSet.UISetting;
        }

        public override UI OnCreate()
        {
            return ObjectFactory.CreateNoInit<UISetting>();
        }
    }

    public partial class UISetting : UI, IAwake
	{	
		public void Initialize()
		{
            this.GetButton(KExit)?.AddClickListener(this.Close);
		}
		
		protected override void OnClose()
		{
			
		}
	}
}
