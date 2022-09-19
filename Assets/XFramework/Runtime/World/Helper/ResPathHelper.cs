using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public static class ResPathHelper
    {
        private static Dictionary<string, string> resPath = new Dictionary<string, string>();

        public static string GetUIPath(string key)
        {
            if (!resPath.TryGetValue(key, out string path))
            {
                path = $"UI/Prefabs/{key}";
                resPath.Add(key, path);
            }

            return path;
        }
    }
}
