using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XFramework
{
    public static class GameObjectExtend
    {
        /// <summary>
        /// ��ȡ���������prefixǰ׺�����ж������Ʋ����ظ�
        /// <para>result.key��ȥ��ǰ׺�������</para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="prefix">ǰ׺</param>
        /// <param name="result">�洢���</param>
        public static void FindChildrenWithPrefix(this GameObject self, string prefix, Dictionary<string, GameObject> result)
        {
            FindWithPrefix(self, prefix, result);
        }

        /// <summary>
        /// ��ȡ���������prefixǰ׺�����ж������Ʋ����ظ�
        /// <para>result.key��ȥ��ǰ׺�������</para>
        /// </summary>
        /// <param name="self"></param>
        /// <param name="prefix">ǰ׺</param>
        /// <param name="result">�洢���</param>
        public static void FindChildrenWithPrefix(this Transform self, string prefix, Dictionary<string, GameObject> result)
        {
            self.gameObject.FindChildrenWithPrefix(prefix, result);
        }

        /// <summary>
        /// ����һ�������ǰ׺
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
        /// ��������
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Reference Reference(this GameObject self)
        {
            return self.GetComponent<Reference>();
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Reference Reference(this Transform self)
        {
            return self.GetComponent<Reference>();
        }

        /// <summary>
        /// ����GameObject����������ʾ
        /// </summary>
        /// <param name="self"></param>
        /// <param name="active"></param>
        public static void SetViewActive(this GameObject self, bool active)
        {
            if (self.activeSelf != active)
                self.SetActive(active);
        }

        /// <summary>
        /// ����GameObject����������ʾ
        /// </summary>
        /// <param name="self"></param>
        /// <param name="active"></param>
        public static void SetViewActive(this Transform self, bool active)
        {
            self.gameObject.SetViewActive(active);
        }

        /// <summary>
        /// ����CanvasGroup�ﵽ������ʾUI��Ч��
        /// </summary>
        /// <param name="self"></param>
        /// <param name="active"></param>
        public static void SetCanvasGroup(this GameObject self, bool active)
        {
            self.transform.SetCanvasGroup(active);
        }

        /// <summary>
        /// ����CanvasGroup�ﵽ������ʾUI��Ч��
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
