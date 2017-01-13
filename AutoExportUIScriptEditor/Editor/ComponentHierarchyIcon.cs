using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace AutoExportScriptData
{
    internal class ComponentHierarchyIcon<T>
        where T : Component
    {
        public static ComponentHierarchyIcon<T> curUse = null;
        private Texture2D feiIcon;
        private Dictionary<int, T> allComponentDic;

        public ComponentHierarchyIcon()
        {
            feiIcon = AssetDatabase.LoadAssetAtPath(FilePathManager.Instance.GetStarIconFilePath(), typeof(Texture2D)) as Texture2D;
            if (feiIcon == null)
            {
                Debug.LogError("Star icon is not find. Please check ini path.");
                return;
            }
            allComponentDic = new Dictionary<int, T>();

            EditorApplication.hierarchyWindowItemOnGUI += OnDrawHierarchyWindowItem;
        }

        /// <summary>
        /// 启用
        /// </summary>
        public static void Open()
        {
            Close();

            curUse = new ComponentHierarchyIcon<T>();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public static void Close()
        {
            if (curUse != null)
                curUse.Stop();
        }

        public void Stop()
        {
            if (feiIcon != null)
                Object.DestroyImmediate(feiIcon);
            feiIcon = null;
            if (allComponentDic != null)
                allComponentDic.Clear();
            allComponentDic = null;

            EditorApplication.hierarchyWindowItemOnGUI -= OnDrawHierarchyWindowItem;
        }

        /// <summary>
        /// 静态构造函数，初始化作用
        /// </summary>
        public static void Init()
        {
            if (ToolsConfigManager.Instance.IsShowUIProgramDataHierarchyIcon)
            {
                Open();
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
}