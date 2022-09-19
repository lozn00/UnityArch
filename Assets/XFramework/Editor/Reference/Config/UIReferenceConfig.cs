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
        /// ���ɵ����Ŀ¼
        /// </summary>
        [TextArea]
        public string ClassExploer = "Assets/Scripts/XFramework/Runtime/World/Game/UI";

        /// <summary>
        /// ����ģ���ļ�·��
        /// </summary>
        [TextArea]
        public string CodeTemplateFilePath = "./Template/UICodeTemplate.txt";

        /// <summary>
        /// key��ģ���ļ�·��
        /// </summary>
        [TextArea]
        public string KeyTemplateFilePath = "./Template/UIReferenceKeyTemplate.txt";

        /// <summary>
        /// UI·���������·��
        /// </summary>
        [TextArea]
        public string UIPathSetFilePath = "Assets/Scripts/XFramework/Runtime/Module/UI/UIPathSet.cs";

        /// <summary>
        /// UIType���·��
        /// </summary>
        [TextArea]
        public string UITypeFilePath = "Assets/Scripts/XFramework/Runtime/Module/UI/UIType.cs";
    }
}
