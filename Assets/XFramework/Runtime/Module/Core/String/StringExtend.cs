using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public static class StringExtend
    {
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }
    }
}
