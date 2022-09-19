using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static XFramework.UIStart;

namespace XFramework
{
	[UIEvent(UIType.UIStart)]
    internal sealed class UIStartEvent : AUIEvent
    {
        public override string GetKey()
        {
            return UIPathSet.UIStart;
        }

        public override UI OnCreate()
        {
            return ObjectFactory.CreateNoInit<UIStart>();
        }
    }

    public partial class UIStart : UI, IAwake
	{	
		public void Initialize()
		{
            this.GetButton(KSetting)?.AddClickListener(OpenSetting);
            this.GetButton(KPlay)?.AddClickListener(GoMainScene);
            this.TestDoMove().Coroutine();
            this.TestDOText().Coroutine();
            this.AddChild<UIStartTestList>(KTestList);
		}

        private void OpenSetting()
        {
            //this.CreateChild(UIType.UISetting, GameObject.transform);
            UIHelper.Create(UIType.UISetting, UILayers.High);
        }

        private void GoMainScene()
        {
            var sceneManager = Common.Instance.Get<SceneController>();
            sceneManager.LoadSceneAsync<MainScene>(SceneName.Main).Coroutine();
        }

		private async Task TestDOText()
		{
			var t = this.GetText(KTestT);
			var tween = t.DoNumber(100, 0, 1f);
			tween.SetLoop(MiniLoopType.Yoyo, 2);
            t.DoFade(0.2f, 0.5f).SetLoop(MiniLoopType.Yoyo, 2);
			bool completed = await tween.Task;
			Log.Info(completed ? "TextCompleted" : "TextCancel");
		}

		private async Task TestDoMove()
        {
            var rectComp = this.GetRectTransform(KTestObj);
            var tagId = rectComp.TagId;

            var timerMgr = Common.Instance.Get<TimerManager>();
            await timerMgr.WaitAsync(1000);
            if (tagId != rectComp.TagId) return;

            var tween1 = rectComp.DoEulerAngleZ(-360, 0.5f);
            tween1.SetLoop(MiniLoopType.Restart, -1);
            for (int i = 0; i < 3; i++)
            {
                var tween = rectComp.DoAnchoredPositionX(300, 1f);
                bool completed = await tween.Task;
                if (!completed) return;

                tween = rectComp.DoAnchoredPositionY(-300, 1f);
                completed = await tween.Task;
                if (!completed) return;

                tween = rectComp.DoAnchoredPositionX(-300, 1f);
                completed = await tween.Task;
                if (!completed) return;

                tween = rectComp.DoAnchoredPositionY(0, 1f);
                completed = await tween.Task;
                if (!completed) return;
            }
            tween1.Cancel(true);
        }

		protected override void OnClose()
		{
			
		}
	}
}
