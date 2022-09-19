using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public class MainScene : Scene, IAwake
    {
        public void Initialize()
        {
            UIHelper.Create(UIType.UIMain, UILayers.Low);
        }
    }
}
