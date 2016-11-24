using UnityEditor;

public class HotKey : Editor
{

    [MenuItem("AutoExportUIScript/Config/Show Hierarchy Icon %1")]
    public static void OpenOrCloseHierarchyUIProgramDataIcon()
    {
        ConfigData.IsShowUIProgramDataHierarchyIcon = !ConfigData.IsShowUIProgramDataHierarchyIcon;
    }

}