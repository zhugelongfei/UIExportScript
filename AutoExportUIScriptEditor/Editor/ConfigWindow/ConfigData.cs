public class ConfigData
{
    public static ComponentHierarchyIcon<AutoExportScriptData.UIProgramData> curUse = null;

    private static bool isShowUIProgramDataHierarchyIcon = true;
    public static bool IsShowUIProgramDataHierarchyIcon
    {
        get { return isShowUIProgramDataHierarchyIcon; }
        set
        {
            if (isShowUIProgramDataHierarchyIcon == value)
                return;

            isShowUIProgramDataHierarchyIcon = value;
            if (isShowUIProgramDataHierarchyIcon)
            {
                curUse = new ComponentHierarchyIcon<AutoExportScriptData.UIProgramData>();
            }
            else
            {
                curUse.Close();
                curUse = null;
            }
        }
    }

}