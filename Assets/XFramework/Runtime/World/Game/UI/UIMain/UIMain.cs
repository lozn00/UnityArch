using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static XFramework.UIMain;

namespace XFramework
{
	[UIEvent(UIType.UIMain)]
    internal sealed class UIMainEvent : AUIEvent
    {
        public override string GetKey()
        {
            return UIPathSet.UIMain;
        }

        public override UI OnCreate()
        {
            return ObjectFactory.CreateNoInit<UIMain>();
        }
    }

    public partial class UIMain : UI, IAwake
	{	
		public void Initialize()
		{
            this.GetButton(KSetting)?.AddClickListener(this.OpenSetting);
            this.GetButton(KQuit)?.AddClickListener(this.GoStartScene);
		}

        private void OpenSetting()
        {
            UIHelper.Create(UIType.UISetting, UILayers.High);
        }

        private void GoStartScene()
        {
            var sceneManager = Common.Instance.Get<SceneController>();
            sceneManager.LoadSceneAsync<StartScene>(SceneName.Start).Coroutine();
        }

        protected override void OnClose()
		{
			
		}
	}
}
