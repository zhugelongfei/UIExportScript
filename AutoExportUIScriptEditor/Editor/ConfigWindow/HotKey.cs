using UnityEditor;

namespace AutoExportScriptData
{
    internal class HotKey : Editor
    {
        [MenuItem("AutoExportUIScript/Config/Show Hierarchy Icon %1")]
        public static void OpenOrCloseHierarchyUIProgramDataIcon()
        {
            ToolsConfigManager.Instance.IsShowUIProgramDataHierarchyIcon = !ToolsConfigManager.Instance.IsShowUIProgramDataHierarchyIcon;
            ToolsConfigManager.Instance.SaveDataToIni();
            EditorApplication.RepaintHierarchyWindow();
        }

    }
}