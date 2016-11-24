using UnityEditor;
using AutoExportScriptData;

[CustomEditor(typeof(UIProgramData))]
public class UIProgramDataInspector : Editor
{
    private bool notExport = false;
    private UIProgramData thisData = null;

    private void Awake()
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

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //检测是否导出设置的值变化
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
    }

}