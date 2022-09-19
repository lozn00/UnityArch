using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public abstract class XPoolEvent
    {
        /// <summary>
        /// 拿出后
        /// </summary>
        /// <param name="obj"></param>
        public virtual void FetchAfter(GameObject obj)
        {

        }

        /// <summary>
        /// 回收前
        /// </summary>
        /// <param name="obj"></param>
        public virtual void BeforeRecycle(GameObject obj)
        {

        }

        /// <summary>
        /// 回收后
        /// </summary>
        /// <param name="obj"></param>
        public virtual void RecycleAfter(GameObject obj)
        {

        }
    }
}
