using UnityEditor;
using AutoExportScriptData;

[CustomEditor(typeof(UIProgramData))]
public class UIProgramDataInspector : Editor
{
    private bool notExport = false;
    private UIProgramData thisData = null;

    private void OnEnable()
    {
        thisData = (target as UIProgramData);
        notExport = thisData.notExport;

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

        if (EditorGUILayout.PropertyField(exportDataArray, UnityEngine.GUILayout.ExpandWidth(false)))
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
                    EditorGUILayout.PropertyField(exportData.FindPropertyRelative("getGameObject"));

                    SerializedProperty isArrayData = exportData.FindPropertyRelative("isArrayData");
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
                        EditorGUILayout.PropertyField(exportData.FindPropertyRelative("CompReference"));
                    }
                    EditorGUI.indentLevel -= 1;
                }
                EditorGUILayout.Space();
            }
            EditorGUI.indentLevel -= 1;
        }

        //不导出数据开关
        notExport = EditorGUILayout.Toggle("Not Export This Data", notExport);
        if (notExport != thisData.notExport)
        {
            //值变化，更改自身以下所有节点的导出值
            UIProgramData[] datas = thisData.GetComponentsInChildren<UIProgramData>(true);
            foreach (var item in datas)
            {
                item.notExport = notExport;
            }
        }

        if (UnityEngine.GUILayout.Button("Set the child parentClass name"))
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