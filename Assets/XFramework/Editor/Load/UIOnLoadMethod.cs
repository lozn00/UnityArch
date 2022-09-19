using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace XFramework
{
    internal class UIOnLoadMethod
    {
        [InitializeOnLoadMethod]
        private static void InitializeCompnent()
        {
            UnityEditor.ObjectFactory.componentWasAdded += ComponentWasAdded;
        }

        /// <summary>
        /// 创建组件后执行的方法
        /// </summary>
        /// <param name="component"></param>
        private static void ComponentWasAdded(Component component)
        {
            if (component is Button && !(component is XButton))
            {
                GameObject obj = component.gameObject;
                Object.DestroyImmediate(component, true);
                obj.AddComponent<XButton>();
                Image image = obj.GetComponent<Image>();
                if (image)
                    image.raycastTarget = true;
            }
            else if (component is Image img)
            {
                bool notExist = !img.GetComponent<Button>() && !img.GetComponentInParent<Toggle>();
                img.raycastTarget = !notExist;
            }
            else if (component is Text && !(component is XText))
            {
                GameObject obj = component.gameObject;
                Object.DestroyImmediate(component, true);
                XText xt = obj.AddComponent<XText>();
                if (!xt.GetComponentInParent<InputField>())
                    xt.raycastTarget = false;
            }
            else if (component is Toggle && !(component is XToggle))
            {
                GameObject obj = component.gameObject;
                Object.DestroyImmediate(component, true);
                obj.AddComponent<XToggle>();
            }
            else if (component is InputField input)
            {
                Image image = input.GetComponent<Image>();
                if (image)
                    image.raycastTarget = true;
            }
            else if (component is ScrollRect scroll)
            {
                Image image = scroll.GetComponent<Image>();
                if (image)
                    image.raycastTarget = true;
            }
            else if (component is Reference)
            {
                GameObject obj = component.gameObject;
                if (obj.GetComponents<Reference>().Length > 1)
                {
                    Object.DestroyImmediate(component, true);
                    //EditorUtility.DisplayDialog("发生错误", "已经包含了一个Reference组件", "记住了");
                }
            }
        }
    }
}
