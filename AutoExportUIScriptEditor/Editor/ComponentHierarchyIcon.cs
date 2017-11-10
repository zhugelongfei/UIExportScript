using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace AutoExportScriptData
{
    internal class UIProgramDataHierarchyIcon
    {
        public static UIProgramDataHierarchyIcon curUse = null;
        private Texture2D correctIcon = null;
        private Texture2D errorIcon = null;

        private Dictionary<int, UIProgramData> allComponentDic = null;
        private GUIStyle style = new GUIStyle();
        private Rect drawRect = new Rect();

        public UIProgramDataHierarchyIcon()
        {
            correctIcon = AssetDatabase.LoadAssetAtPath(FilePathManager.Instance.GetCorrectIconFilePath(), typeof(Texture2D)) as Texture2D;
            errorIcon = AssetDatabase.LoadAssetAtPath(FilePathManager.Instance.GetErrorIconFilePath(), typeof(Texture2D)) as Texture2D;
            if (correctIcon == null || errorIcon == null)
            {
                Debug.LogError("Icon is not find. Please check ini path.");
                Debug.LogError(FilePathManager.Instance.GetCorrectIconFilePath());
                return;
            }
            allComponentDic = new Dictionary<int, UIProgramData>();

            EditorApplication.hierarchyWindowItemOnGUI += OnDrawHierarchyWindowItem;

            style.normal.textColor = Color.white;
            style.fontStyle = FontStyle.Bold;
            style.fontSize = 12;

            drawRect.width = 16;
            drawRect.height = 16;
        }

        /// <summary>
        /// 启用
        /// </summary>
        public static void Open()
        {
            Close();

            curUse = new UIProgramDataHierarchyIcon();
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
            correctIcon = null;
            if (allComponentDic != null)
                allComponentDic.Clear();
            allComponentDic = null;

            EditorApplication.hierarchyWindowItemOnGUI -= OnDrawHierarchyWindowItem;
        }

        /// <summary>
        /// 绘制物体的特殊标示
        /// </summary>
        private void OnDrawHierarchyWindowItem(int instanceID, Rect selectionRect)
        {
            if (!allComponentDic.ContainsKey(instanceID) || allComponentDic[instanceID] == null)
            {
                UIProgramData data = GetComponentFromObj(instanceID);
                if (data == null)
                {
                    allComponentDic.Remove(instanceID);
                    return;
                }

                allComponentDic[instanceID] = data;
            }

            Texture2D icon = CheckProDataIsRight(instanceID) ? correctIcon : errorIcon;

            drawRect.x = selectionRect.x + selectionRect.width - 16;
            drawRect.y = selectionRect.y;

            GUI.DrawTexture(drawRect, icon);

            drawRect.y += 1;

            GameObject gameObj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            UIProgramData[] dataList = gameObj.GetComponentsInChildren<UIProgramData>();

            string countStr = dataList.Length.ToString();

            drawRect.x -= countStr.Length * 10;

            GUI.Label(drawRect, countStr, style);
        }

        private UIProgramData GetComponentFromObj(int instanceID)
        {
            GameObject data = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (data == null) return null;
            return data.GetComponent<UIProgramData>();
        }

        /// <summary>
        /// 检测UI挂载的数据是否正确
        /// </summary>
        private bool CheckProDataIsRight(int instanceID)
        {
            if (allComponentDic[instanceID] == null)
                return false;

            UIProgramData data = allComponentDic[instanceID];

            if (data.ExportData == null)
                return false;

            foreach (var item in data.ExportData)
            {
                if (string.IsNullOrEmpty(item.VariableName))
                    return false;

                if (item.isArrayData)
                {
                    foreach (var comp in item.CompReferenceArray)
                    {
                        if (comp == null)
                            return false;
                    }
                }
                else
                {
                    if (item.CompReference == null)
                        return false;
                }
            }

            return true;
        }

    }
}