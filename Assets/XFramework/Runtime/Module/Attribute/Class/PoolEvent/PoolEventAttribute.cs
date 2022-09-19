using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public class PoolEventAttribute : BaseAttribute
    {
        public string Key { get; private set; }

        public PoolEventAttribute(string key)
        {
            this.Key = key;
        }
    }
}
