using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace XFramework
{
    public class StartScene : Scene, IAwake, IEvent<int>, IEvent<float>
    {
        public override void GetObjects(ICollection<string> objKeys)
        {
            objKeys.Add(UIPathSet.UIStart);
            objKeys.Add(UIPathSet.UISetting);
        } 

        public void Initialize()
        {
            Log.Debug($"进入Start场景");
            UIHelper.Create(UIType.UIStart, UILayers.Low);
            EventManager.Instance.Publish(1);
            //TestConfig testConfig = TestConfigManager.Instance.Get(1);
            //Log.Info(JsonHelper.ToJson(testConfig));
        }

        public override void Update()
        {
            base.Update();
            Log.Debug("StartScene Update");
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Log.Debug("离开Start场景");
        }

        public void RunEvent(int args)
        {
            Log.Debug($"TestEvent {args}");
        }

        public void RunEvent(float args)
        {
            throw new System.NotImplementedException();
        }
    }
}
