using UnityEditor;

namespace AutoExportScriptData
{
    [CustomEditor(typeof(UIProgramData)), CanEditMultipleObjects]
    internal class UIProgramDataInspector : Editor
    {
        private UIProgramData thisData = null;
        private SerializedObject serObj;
        private SerializedProperty prop_CreateClassName;
        private SerializedProperty prop_LocalClassName;
        private SerializedProperty prop_ExportData;

        private void OnEnable()
        {
            thisData = (target as UIProgramData);
            serObj = serializedObject;
            prop_ExportData = serObj.FindProperty("ExportData");
            prop_CreateClassName = serObj.FindProperty("CreateClassName");
            prop_LocalClassName = serObj.FindProperty("LocalClassName");

            if (prop_ExportData != null && prop_ExportData.arraySize == 0)
            {
                //在添加脚本对象时，就初始化data的长度为1，不用手动设置长度，节省开发时间
                prop_ExportData.arraySize = 1;

                SerializedProperty firstProp = prop_ExportData.GetArrayElementAtIndex(0);
                firstProp.FindPropertyRelative("VariableName").stringValue = thisData.name;
                firstProp.FindPropertyRelative("CompReference").objectReferenceValue = thisData.transform;
                serObj.ApplyModifiedProperties();
            }
        }

        private void OnDisable()
        {
            thisData = null;
            serObj = null;
            prop_CreateClassName = null;
            prop_LocalClassName = null;
            prop_ExportData = null;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            EditorGUILayout.PropertyField(prop_CreateClassName);
            EditorGUILayout.PropertyField(prop_LocalClassName);

            if (EditorGUILayout.PropertyField(prop_ExportData))
            {
                EditorGUI.indentLevel += 1;

                //显示ExportData下边的节点信息
                prop_ExportData.arraySize = EditorGUILayout.IntField("Size", prop_ExportData.arraySize);

                for (int index = 0; index < prop_ExportData.arraySize; index++)
                {
                    SerializedProperty exportData = prop_ExportData.GetArrayElementAtIndex(index);
                    if (EditorGUILayout.PropertyField(exportData))
                    {
                        EditorGUI.indentLevel += 1;

                        SerializedProperty isGameObject = exportData.FindPropertyRelative("isGameObjectRef");
                        SerializedProperty isArrayData = exportData.FindPropertyRelative("isArrayData");
                        SerializedProperty singleComp = exportData.FindPropertyRelative("CompReference");
                        SerializedProperty arrayComp = exportData.FindPropertyRelative("CompReferenceArray");

                        //Is Game Object
                        if (ToolsConfigManager.Instance.OpenGenerateGameObjectRef)
                            EditorGUILayout.PropertyField(isGameObject);

                        //Is Array Data
                        bool oldIsArray = isArrayData.boolValue;
                        if (ToolsConfigManager.Instance.OpenGenerateArrayRef)
                            EditorGUILayout.PropertyField(isArrayData);

                        //Is Array Change
                        if (isArrayData.boolValue != oldIsArray)
                        {
                            if (isArrayData.boolValue)
                            {
                                singleComp.objectReferenceValue = null;
                                if (arrayComp != null && arrayComp.arraySize == 0)
                                {
                                    arrayComp.arraySize = 2;
                                }
                            }
                            else
                            {
                                arrayComp.ClearArray();
                                singleComp.objectReferenceValue = thisData.transform;
                            }
                        }

                        EditorGUILayout.PropertyField(exportData.FindPropertyRelative("VariableName"));
                        if (isArrayData.boolValue)
                        {
                            //数组组件引用
                            if (EditorGUILayout.PropertyField(arrayComp, false))
                            {
                                EditorGUI.indentLevel += 1;

                                arrayComp.arraySize = EditorGUILayout.IntField("Size", arrayComp.arraySize);

                                SerializedProperty firstProp = arrayComp.GetArrayElementAtIndex(0);

                                //第一个组件没有赋值，则认为不可继续
                                firstProp.objectReferenceValue = EditorGUILayout.ObjectField("Type Element", firstProp.objectReferenceValue, typeof(UnityEngine.Component), true);

                                if (firstProp.objectReferenceValue != null)
                                {
                                    System.Type compType = firstProp.objectReferenceValue.GetType();
                                    for (int i = 1; i < arrayComp.arraySize; i++)
                                    {
                                        SerializedProperty comp = arrayComp.GetArrayElementAtIndex(i);
                                        comp.objectReferenceValue = EditorGUILayout.ObjectField("Element " + i.ToString(), comp.objectReferenceValue, compType, true);
                                        if(comp.objectReferenceValue != null && comp.objectReferenceValue.GetType() != compType)
                                        {
                                            //非第一个引用的类型不对
                                            comp.objectReferenceValue = null;
                                        }
                                    }
                                }
                                EditorGUI.indentLevel -= 1;
                            }
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(singleComp);
                        }
                        EditorGUI.indentLevel -= 1;
                    }
                }
                EditorGUI.indentLevel -= 1;
            }

            //不导出数据开关
            EditorGUILayout.PropertyField(serObj.FindProperty("notExport"));

            EditorGUILayout.Separator();
            UnityEngine.GUILayout.Label("Set Child Setting:", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (UnityEngine.GUILayout.Button("NotExport"))
            {
                //更改自身以下所有节点的导出值
                bool notExport = serObj.FindProperty("notExport").boolValue;
                UIProgramData[] datas = thisData.GetComponentsInChildren<UIProgramData>(true);
                foreach (var item in datas)
                {
                    item.notExport = notExport;
                }
            }

            if (UnityEngine.GUILayout.Button("ParentClass"))
            {
                UIProgramData[] datas = thisData.GetComponentsInChildren<UIProgramData>(true);
                foreach (var item in datas)
                {
                    item.LocalClassName = thisData.CreateClassName;
                }
            }
            EditorGUILayout.EndHorizontal();

            //保存数值变化
            serObj.ApplyModifiedProperties();
        }

    }
}
