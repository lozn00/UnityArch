using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public sealed class Global : CommonObject
    {
        public Transform GameRoot { get; private set; }

        public Transform UI { get; private set; }

        public GameObject Loading { get; private set; }

        protected override void Init()
        {
            GameRoot = GameObject.Find("/GameRoot").transform;
            UI = GameRoot.Find("UI");
            Loading = UI.Find("High/Loading").gameObject;
        }

        protected override void Destroy()
        {
            GameRoot = null;
            UI = null;
            Loading = null;
        }
    }
}
