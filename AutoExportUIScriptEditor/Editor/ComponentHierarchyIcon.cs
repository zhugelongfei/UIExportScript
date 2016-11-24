using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[InitializeOnLoad]
public class ComponentHierarchyIcon<T>
    where T : Component
{
    private Texture2D feiIcon;
    private Dictionary<int, T> allComponentDic;

    /// <summary>
    /// 启用
    /// </summary>
    public ComponentHierarchyIcon()
    {
        if (ConfigData.curUse != null)
            ConfigData.curUse.Close();

        feiIcon = AssetDatabase.LoadAssetAtPath(AutoExportScriptData.FilePathManager.Instance.starIconFilePath, typeof(Texture2D)) as Texture2D;
        if (feiIcon == null)
            return;
        allComponentDic = new Dictionary<int, T>();

        EditorApplication.hierarchyWindowItemOnGUI += OnDrawHierarchyWindowItem;
    }

    /// <summary>
    /// 关闭
    /// </summary>
    public void Close()
    {
        feiIcon = null;
        if (allComponentDic != null)
            allComponentDic.Clear();
        allComponentDic = null;

        EditorApplication.hierarchyWindowItemOnGUI -= OnDrawHierarchyWindowItem;
    }

    /// <summary>
    /// 静态构造函数，初始化作用
    /// </summary>
    static ComponentHierarchyIcon()
    {
        if (ConfigData.IsShowUIProgramDataHierarchyIcon)
        {
            ConfigData.curUse = new ComponentHierarchyIcon<AutoExportScriptData.UIProgramData>();
        }
    }

    /// <summary>
    /// 绘制物体的特殊标示
    /// </summary>
    private void OnDrawHierarchyWindowItem(int instanceID, Rect selectionRect)
    {
        if (!allComponentDic.ContainsKey(instanceID) || allComponentDic[instanceID] == null)
        {
            T data = GetComponentFromObj(instanceID);
            if (data == null)
            {
                allComponentDic.Remove(instanceID);
                return;
            }

            allComponentDic[instanceID] = data;
        }

        Rect drawRect = new Rect(selectionRect.x + selectionRect.width - 16, selectionRect.y, 16, 16);
        GUI.DrawTexture(drawRect, feiIcon);
    }

    private T GetComponentFromObj(int instanceID)
    {
        GameObject data = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (data == null) return null;
        return data.GetComponent<T>();
    }
}