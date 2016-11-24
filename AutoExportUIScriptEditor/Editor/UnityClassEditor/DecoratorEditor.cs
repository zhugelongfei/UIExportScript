using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DecoratorEditor : Editor
{
    private static readonly object[] EMPTY_ARRAY = new object[0];

    private Type decoratedEditorType;

    private Type editedObjectType;

    private Editor editorInstance;

    private static Dictionary<string, MethodInfo> dic_Methods = new Dictionary<string, MethodInfo>();

    private static Assembly editorAssembly = Assembly.GetAssembly(typeof(Editor));

    protected Editor EditorInstance
    {
        get
        {
            if (editorInstance == null && targets != null && targets.Length > 0)
            {
                editorInstance = Editor.CreateEditor(targets, decoratedEditorType);
            }
            if (editorInstance == null)
            {
                Debug.LogError("Could not create editor !");
            }
            return editorInstance;
        }
    }

    public DecoratorEditor(string editorTypeName)
    {
        decoratedEditorType = editorAssembly.GetTypes().Where(t => t.Name == editorTypeName).FirstOrDefault();

        Init();

        Type originalEditedType = GetCustomEditorType(decoratedEditorType);

        if (originalEditedType != editedObjectType)
        {
            throw new ArgumentException(string.Format("Type {0} does not match the editor {1} type {2}",
                          editedObjectType, editorTypeName, originalEditedType));
        }

    }

    private void Init()
    {
        editedObjectType = GetCustomEditorType(this.GetType());
    }

    private Type GetCustomEditorType(Type type)
    {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;

        CustomEditor[] attributes = this.GetType().GetCustomAttributes(typeof(CustomEditor), true) as CustomEditor[];
        FieldInfo field = attributes.Select(editor => editor.GetType().GetField("m_InspectedType", flags)).First();
        return field.GetValue(attributes[0]) as Type; ;
    }

    private void OnDisable()
    {
        if (editorInstance != null)
        {
            DestroyImmediate(editorInstance);
        }
    }

    protected void CallInspectorMethod(string methodName)
    {
        MethodInfo method = null;
        if (!dic_Methods.ContainsKey(methodName))
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;

            method = decoratedEditorType.GetMethod(methodName, flags);
            if (method != null)
            {
                dic_Methods[methodName] = method;
            }
            else
            {
                Debug.LogError(string.Format("Could not find method {0}", methodName));
            }
        }
        else
        {
            method = dic_Methods[methodName];
        }

        if (method != null)
        {
            method.Invoke(EditorInstance, EMPTY_ARRAY);
        }
    }

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4

    public void OnSceneGUI()
    {
        CallInspectorMethod("OnSceneGUI");
    }
#endif

    protected override void OnHeaderGUI()
    {
        CallInspectorMethod("OnHeaderGUI");
    }

    public override void OnInspectorGUI()
    {
        EditorInstance.OnInspectorGUI();
    }

    public override void DrawPreview(Rect previewArea)
    {
        EditorInstance.DrawPreview(previewArea);
    }

    public override string GetInfoString()
    {
        return EditorInstance.GetInfoString();
    }

    public override GUIContent GetPreviewTitle()
    {
        return EditorInstance.GetPreviewTitle();
    }

    public override bool HasPreviewGUI()
    {
        return EditorInstance.HasPreviewGUI();
    }

    public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
    {
        EditorInstance.OnInteractivePreviewGUI(r, background);
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        EditorInstance.OnPreviewGUI(r, background);
    }

    public override void OnPreviewSettings()
    {
        EditorInstance.OnPreviewSettings();
    }

    public override void ReloadPreviewInstances()
    {
        EditorInstance.ReloadPreviewInstances();
    }

    public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height)
    {
        return EditorInstance.RenderStaticPreview(assetPath, subAssets, width, height);
    }

    public override bool RequiresConstantRepaint()
    {
        return EditorInstance.RequiresConstantRepaint();
    }

    public override bool UseDefaultMargins()
    {
        return EditorInstance.UseDefaultMargins();
    }

}