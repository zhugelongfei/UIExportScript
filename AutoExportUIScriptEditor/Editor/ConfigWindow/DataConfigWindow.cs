using UnityEditor;
using UnityEngine;

public class DataConfigWindow :EditorWindow
{
    private static DataConfigWindow window = null;

    private const float border = 20;
    private const float fieldWidth = 150;
    private GUILayoutOption fieldWidthOption = GUILayout.Width(fieldWidth);

    private static GUIStyle splitTextStyle = new GUIStyle();
    private const float separatorWidth = 5;
    private GUILayoutOption separatorWidthOption = GUILayout.Width(separatorWidth);

    [MenuItem("AutoExportUIScript/ConfigWindow")]
    public static void Open()
    {
        splitTextStyle.normal.textColor = Color.red;

        if (window == null)
            GetWindow<DataConfigWindow>(false, "Config", true).Show();
    }

    public void OnGUI()
    {
        //Press esc key to close this window
        Event eventData = Event.current;
        if (eventData.isKey)
        {
            if (eventData.keyCode == KeyCode.Escape)
            {
                if (eventData.type == EventType.KeyDown)
                {
                    Close();
                    return;
                }
            }
        }
        ToolsConfigManager.Instance.IsShowUIProgramDataHierarchyIcon = GUILayout.Toggle(ToolsConfigManager.Instance.IsShowUIProgramDataHierarchyIcon, "是否显示UIProgramData的Icon");

    }

    public void OnEnable()
    {
        window = this;
    }

    public void OnDisable()
    {
        window = null;
    }
}