using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public static class GameObjectExtend
    {
        /// <summary>
        /// 获取对象里包含prefix前缀的所有对象，名称不能重复
        /// <para>result.key是去除前缀后的名称</para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="prefix">前缀</param>
        /// <param name="result">存储结果</param>
        public static void FindChildrenWithPrefix(this GameObject self, string prefix, Dictionary<string, GameObject> result)
        {
            FindWithPrefix(self, prefix, result);
        }

        /// <summary>
        /// 获取对象里包含prefix前缀的所有对象，名称不能重复
        /// <para>result.key是去除前缀后的名称</para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="prefix">前缀</param>
        /// <param name="result">存储结果</param>
        public static void FindChildrenWithPrefix(this Transform self, string prefix, Dictionary<string, GameObject> result)
        {
            self.gameObject.FindChildrenWithPrefix(prefix, result);
        }

        /// <summary>
        /// 查找一个对象的前缀
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="result"></param>
        private static void FindWithPrefix(GameObject obj, string prefix, Dictionary<string, GameObject> result)
        {
            if (prefix.IsNullOrEmpty())
                return;

            using var transforms = XList<Transform>.Create();
            obj.GetComponentsInChildren<Transform>(true, transforms);
            foreach (var trans in transforms)
            {
                if (prefix != trans.name && trans.name.StartsWith(prefix))
                {
                    string name = trans.name.Substring(prefix.Length);
                    if (!result.ContainsKey(name))
                        result.Add(name, trans.gameObject); 
                }
            }
        }

        /// <summary>
        /// 对象引用
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Reference Reference(this GameObject self)
        {
            return self.GetComponent<Reference>();
        }

        /// <summary>
        /// 对象引用
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Reference Reference(this Transform self)
        {
            return self.GetComponent<Reference>();
        }

        /// <summary>
        /// 设置GameObject的隐藏与显示
        /// </summary>
        /// <param name="self"></param>
        /// <param name="active"></param>
        public static void SetViewActive(this GameObject self, bool active)
        {
            if (self.activeSelf != active)
                self.SetActive(active);
        }

        /// <summary>
        /// 设置GameObject的隐藏与显示
        /// </summary>
        /// <param name="self"></param>
        /// <param name="active"></param>
        public static void SetViewActive(this Transform self, bool active)
        {
            self.gameObject.SetViewActive(active);
        }

        /// <summary>
        /// 设置CanvasGroup达到隐藏显示UI的效果
        /// </summary>
        /// <param name="self"></param>
        /// <param name="active"></param>
        public static void SetCanvasGroup(this GameObject self, bool active)
        {
            self.transform.SetCanvasGroup(active);
        }

        /// <summary>
        /// 设置CanvasGroup达到隐藏显示UI的效果
        /// </summary>
        /// <param name="self"></param>
        /// <param name="active"></param>
        public static void SetCanvasGroup(this Transform self, bool active)
        {
            if (!(self is RectTransform))
                return;

            CanvasGroup canvasGroup = self.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = self.gameObject.AddComponent<CanvasGroup>();

            canvasGroup.alpha = active ? 1 : 0;
            canvasGroup.interactable = active;
            canvasGroup.blocksRaycasts = active;
        }
    }
}
