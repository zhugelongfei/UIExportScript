//#define Use_Custom_Transform_Editor

#if Use_Custom_Transform_Editor
using UnityEditor;
using UnityEngine;
using AutoExportScriptData;

[CustomEditor(typeof(Transform))]
public class TransformEditor : DecoratorEditor
{
    public TransformEditor()
        : base("TransformInspector")
    {

    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Add UIProgram Data"))
        {
            (target as Transform).gameObject.AddComponent<UIProgramData>();
        }
    }
}
#endif