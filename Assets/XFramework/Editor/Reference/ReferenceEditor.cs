using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace XFramework
{
    [CustomEditor(typeof(Reference))]
    public class ReferenceEditor : Editor
    {
        private GameObject gameObject;
        private string beforePrefix;
        protected HashSet<string> prefixKeys = new HashSet<string>();
        protected HashSet<string> dragKeys = new HashSet<string>();

        private static bool foldoutElements = true;
        private static bool foldoutDragElementsName = true;

        protected const string FindPrefixName = "findPrefix";
        protected const string ElementsName = "elements";
        protected const string DragElementsName = "dragElements";

        protected virtual void OnEnable()
        {
            prefixKeys.Clear();
            dragKeys.Clear();
            gameObject = (target as Reference).gameObject;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Space();

            GUILayout.BeginVertical();
            SerializedProperty prefixProperty = serializedObject.FindProperty(FindPrefixName);
            EditorGUILayout.PropertyField(prefixProperty);
            GUILayout.EndVertical();

            string prefix = prefixProperty.stringValue;
            if (prefix != beforePrefix)
            {
                RefreshPrefixChildren();
                beforePrefix = prefix;
            }

            DisplaySingleElements("Prefix Elements", ElementsName, ref foldoutElements, prefixKeys);
            DisplaySingleElements("Dragged Elements", DragElementsName, ref foldoutDragElementsName, dragKeys, DrawDragRemoveButton);

            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            if (GUILayout.Button("Refresh"))
            {
                RefreshPrefixChildren();
            }
            GUILayout.EndVertical();

            DragElement();
        }

        /// <summary>
        /// ˢ������ǰ׺�Ķ���
        /// </summary>
        private void RefreshPrefixChildren()
        {
            if (Application.isPlaying)
                return;

            SerializedProperty prefixProperty = serializedObject.FindProperty(FindPrefixName);
            SerializedProperty elements = serializedObject.FindProperty(ElementsName);

            string prefix = prefixProperty.stringValue;
            elements.ClearArray();
            if (!string.IsNullOrEmpty(prefix))
            {
                Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
                gameObject.FindChildrenWithPrefix(prefix, objects);
                int i = 0;
                foreach (var item in objects)
                {
                    string key = item.Key;
                    GameObject obj = item.Value;
                    elements.InsertArrayElementAtIndex(i);
                    SerializedProperty elem = elements.GetArrayElementAtIndex(i);
                    elem.FindPropertyRelative("Key").stringValue = key;
                    elem.FindPropertyRelative("Object").objectReferenceValue = obj;
                    ++i;
                }

                if (i > 0)
                    SetDirty();
            }

            Save();
        }

        /// <summary>
        /// ��ʾ����Ԫ���б�
        /// </summary>
        /// <param name="propertyName"></param>
        private void DisplaySingleElements(string label, string propertyName, ref bool foldout, HashSet<string> keys, System.Action<int> action = null)
        {
            EditorGUILayout.Space();

            keys.Clear();
            SerializedProperty elements = serializedObject.FindProperty(propertyName);
            int size = elements.arraySize;
            foldout = EditorGUILayout.Foldout(foldout, $"{label}, Size is {size}", true);
            if (foldout)
            {
                //EditorGUILayout.LabelField($"{label}, Size is {size}");
                for (int i = 0; i < size; i++)
                {
                    GUILayout.BeginVertical();
                    GUILayout.BeginHorizontal();

                    SerializedProperty elem = elements.GetArrayElementAtIndex(i);
                    string key = elem.FindPropertyRelative("Key").stringValue;
                    GameObject obj = elem.FindPropertyRelative("Object").objectReferenceValue as GameObject;
                    EditorGUILayout.TextField(key);
                    EditorGUILayout.ObjectField(obj, typeof(GameObject), this);

                    action?.Invoke(i);
                    keys.Add(key);

                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }
            }
        }

        /// <summary>
        /// ������קԪ�ص��Ƴ���ť
        /// </summary>
        /// <param name="index"></param>
        private void DrawDragRemoveButton(int index)
        {
            if (GUILayout.Button("�Ƴ�"))
            {
                RemoveDragElement(index);
            }
        }

        /// <summary>
        /// ��קԪ��
        /// </summary>
        private void DragElement()
        {
            if (Application.isPlaying)
                return;

            var eventType = Event.current.type;
            if (eventType == UnityEngine.EventType.DragUpdated || eventType == UnityEngine.EventType.DragPerform)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (eventType == UnityEngine.EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    foreach (Object o in DragAndDrop.objectReferences)
                    {
                        if (o is GameObject obj)
                        {
                            // ���������Լ�
                            if (obj == gameObject)
                                continue;
                            AddDragElement(obj);
                        }
                    }
                }

                Event.current.Use();
            }
        }

        /// <summary>
        /// ���һ����ק��Ԫ��
        /// </summary>
        /// <param name="obj"></param>
        private void AddDragElement(GameObject obj)
        {
            string key = obj.name;
            if (prefixKeys.Contains(key) || dragKeys.Contains(key))
            {
                Debug.LogError("��ק��Object�����Ѿ������б���");
                return;
            }

            SerializedProperty elements = serializedObject.FindProperty(DragElementsName);
            int size = elements.arraySize;

            elements.InsertArrayElementAtIndex(size);
            SerializedProperty elem = elements.GetArrayElementAtIndex(size);
            elem.FindPropertyRelative("Key").stringValue = obj.name;
            elem.FindPropertyRelative("Object").objectReferenceValue = obj;

            SetDirty();
            Save();
        }

        /// <summary>
        /// ɾ����ק��Ԫ��
        /// </summary>
        /// <param name="index"></param>
        private void RemoveDragElement(int index)
        {
            SerializedProperty elements = serializedObject.FindProperty(DragElementsName);
            elements.DeleteArrayElementAtIndex(index);
            SetDirty();
            Save();
        }

        /// <summary>
        /// ����޸�
        /// </summary>
        protected new void SetDirty()
        {
            EditorUtility.SetDirty(target);
        }

        /// <summary>
        /// ��������
        /// </summary>
        protected void Save()
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.UpdateIfRequiredOrScript();
        }
    }
}
