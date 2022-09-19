using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public class MonoSingleton<T> : MonoBehaviour where T : Object
    {
        protected static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<T>();

                return instance;
            }
        }
    }
}
