using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XFramework
{
    [CreateAssetMenu(menuName = "UIReferenceConfig", fileName = "UIReferenceConfig")]
    public class UIReferenceConfig : ScriptableObject
    {
        /// <summary>
        /// 生成的类的目录
        /// </summary>
        [TextArea]
        public string ClassExploer = "Assets/Scripts/XFramework/Runtime/World/Game/UI";

        /// <summary>
        /// 代码模板文件路径
        /// </summary>
        [TextArea]
        public string CodeTemplateFilePath = "./Template/UICodeTemplate.txt";

        /// <summary>
        /// key的模板文件路径
        /// </summary>
        [TextArea]
        public string KeyTemplateFilePath = "./Template/UIReferenceKeyTemplate.txt";

        /// <summary>
        /// UI路径集合类的路径
        /// </summary>
        [TextArea]
        public string UIPathSetFilePath = "Assets/Scripts/XFramework/Runtime/Module/UI/UIPathSet.cs";

        /// <summary>
        /// UIType类的路径
        /// </summary>
        [TextArea]
        public string UITypeFilePath = "Assets/Scripts/XFramework/Runtime/Module/UI/UIType.cs";
    }
}
