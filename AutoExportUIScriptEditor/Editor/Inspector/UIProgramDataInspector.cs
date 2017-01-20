using UnityEditor;

namespace AutoExportScriptData
{
    [CustomEditor(typeof(UIProgramData)),CanEditMultipleObjects]
    internal class UIProgramDataInspector : Editor
    {
        private UIProgramData thisData = null;

        private void OnEnable()
        {
            thisData = (target as UIProgramData);

            SerializedProperty prop = serializedObject.FindProperty("ExportData");

            if (prop != null && prop.arraySize == 0)
            {
                //在添加脚本对象时，就初始化data的长度为1，不用手动设置长度，节省开发时间
                prop.arraySize = 1;
                serializedObject.ApplyModifiedProperties();

                thisData.ExportData[0].VariableName = thisData.name;
            }
        }

        private void OnDisable()
        {
            thisData = null;
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

            SerializedObject serObj = new SerializedObject(target);
            SerializedProperty exportDataArray = serObj.FindProperty("ExportData");

            EditorGUILayout.PropertyField(serObj.FindProperty("CreateClassName"));
            EditorGUILayout.PropertyField(serObj.FindProperty("LocalClassName"));

            if (EditorGUILayout.PropertyField(exportDataArray))
            {
                EditorGUI.indentLevel += 1;

                //显示ExportData下边的节点信息
                exportDataArray.arraySize = EditorGUILayout.IntField("Size", exportDataArray.arraySize);

                for (int index = 0; index < exportDataArray.arraySize; index++)
                {
                    SerializedProperty exportData = exportDataArray.GetArrayElementAtIndex(index);
                    if (EditorGUILayout.PropertyField(exportData))
                    {
                        EditorGUI.indentLevel += 1;
                        //显示Element Index下的节点信息
                        if (ToolsConfigManager.Instance.OpenGenerateGameObjectRef)
                            EditorGUILayout.PropertyField(exportData.FindPropertyRelative("isGameObjectRef"));

                        SerializedProperty isArrayData = exportData.FindPropertyRelative("isArrayData");

                        if (ToolsConfigManager.Instance.OpenGenerateArrayRef)
                            EditorGUILayout.PropertyField(isArrayData);

                        EditorGUILayout.PropertyField(exportData.FindPropertyRelative("VariableName"));
                        if (isArrayData.boolValue)
                        {
                            //数组组件引用
                            exportData.FindPropertyRelative("CompReference").objectReferenceValue = null;
                            SerializedProperty compArray = exportData.FindPropertyRelative("CompReferenceArray");
                            if (compArray != null && compArray.arraySize == 0)
                            {
                                compArray.arraySize = 2;
                            }
                            EditorGUILayout.PropertyField(compArray, true);
                        }
                        else
                        {
                            //单一组件引用
                            exportData.FindPropertyRelative("CompReferenceArray").ClearArray();
                            SerializedProperty compRef = exportData.FindPropertyRelative("CompReference");
                            if (compRef.objectReferenceValue == null)
                            {
                                compRef.objectReferenceValue = thisData.transform;
                            }
                            EditorGUILayout.PropertyField(compRef);
                        }
                        EditorGUI.indentLevel -= 1;
                    }
                    EditorGUILayout.Space();
                }
                EditorGUI.indentLevel -= 1;
            }

            //不导出数据开关
            EditorGUILayout.PropertyField(serObj.FindProperty("notExport"));
            if (UnityEngine.GUILayout.Button("Set the child notExport"))
            {
                //更改自身以下所有节点的导出值
                UIProgramData[] datas = thisData.GetComponentsInChildren<UIProgramData>(true);
                foreach (var item in datas)
                {
                    item.notExport = serObj.FindProperty("notExport").boolValue;
                }
            }

            if (UnityEngine.GUILayout.Button("Set the child parentClass"))
            {
                UIProgramData[] datas = thisData.GetComponentsInChildren<UIProgramData>(true);
                foreach (var item in datas)
                {
                    item.LocalClassName = thisData.CreateClassName;
                }
            }

            //保存数值变化
            serObj.ApplyModifiedProperties();
        }
    }
}