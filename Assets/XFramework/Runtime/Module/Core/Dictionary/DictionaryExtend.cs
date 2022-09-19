using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public static class DictionaryExtend
    {
        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, out TValue result)
        {
            if (self.TryGetValue(key, out result))
                return self.Remove(key);

            return false;
        }

        public static bool TryRemove<TKey, TValue>(this SortedDictionary<TKey, TValue> self, TKey key, out TValue result)
        {
            if (self.TryGetValue(key, out result))
                return self.Remove(key);

            return false;
        }
    }
}
