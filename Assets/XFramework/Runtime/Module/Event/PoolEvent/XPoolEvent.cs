using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public abstract class XPoolEvent
    {
        /// <summary>
        /// �ó���
        /// </summary>
        /// <param name="obj"></param>
        public virtual void FetchAfter(GameObject obj)
        {

        }

        /// <summary>
        /// ����ǰ
        /// </summary>
        /// <param name="obj"></param>
        public virtual void BeforeRecycle(GameObject obj)
        {

        }

        /// <summary>
        /// ���պ�
        /// </summary>
        /// <param name="obj"></param>
        public virtual void RecycleAfter(GameObject obj)
        {

        }
    }
}
