using UnityEditor;
using UnityEngine;

namespace AutoExportScriptData
{
    internal class DataConfigWindow : EditorWindow
    {
        private static DataConfigWindow window = null;

        private const float OK_BTN_WIDTH = 100;
        private const float OK_BTN_HEIGHT = 20;
        private const float OK_BTN_BORDER = 2;

        private static GUIStyle noteStyle = new GUIStyle();

        private bool editEnable = false;

        [MenuItem("AutoExportUIScript/ConfigWindow")]
        public static void Open()
        {
            if (window == null)
                GetWindow<DataConfigWindow>(false, "Config", true).Show();
            noteStyle.normal.textColor = Color.red;
            noteStyle.fontStyle = FontStyle.Bold;
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
            GUILayout.Label("AutoExportUIScript Global Settings", EditorStyles.boldLabel);

            editEnable = EditorGUILayout.BeginToggleGroup("Option Setting", editEnable);

            ToolsConfigManager.Instance.IsShowUIProgramDataHierarchyIcon = ShowBool("Hierarchy面板显示UIProgramData的状态", ToolsConfigManager.Instance.IsShowUIProgramDataHierarchyIcon);

            ToolsConfigManager.Instance.UseGetAttribute = EditorGUILayout.BeginToggleGroup("导出字段使用Get属性（保证数据的只读性）", ToolsConfigManager.Instance.UseGetAttribute);

            if (!ToolsConfigManager.Instance.UseGetAttribute)
            {
                GUILayout.Label("     Note:不使用Get可以提升字段的访问速度。", noteStyle);
                GUILayout.Label("     但是一定要注意，不要在外部更改这个字段", noteStyle);
            }
            EditorGUI.indentLevel += 1;
            ToolsConfigManager.Instance.DataSafeCheck = ShowBool("检测数据安全性（如果字段为空，则会返回一个新的实例）", ToolsConfigManager.Instance.DataSafeCheck);
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndToggleGroup();

            ToolsConfigManager.Instance.OpenGenerateGameObjectRef = ShowBool("Inspector显示Is Game Object Ref", ToolsConfigManager.Instance.OpenGenerateGameObjectRef);

            ToolsConfigManager.Instance.OpenGenerateArrayRef = ShowBool("Inspector显示Is Array Data", ToolsConfigManager.Instance.OpenGenerateArrayRef);

            EditorGUILayout.EndToggleGroup();

            if (GUI.Button(new Rect(position.xMax - position.xMin - OK_BTN_WIDTH - OK_BTN_BORDER, position.yMax - position.yMin - OK_BTN_HEIGHT - OK_BTN_BORDER, OK_BTN_WIDTH, OK_BTN_HEIGHT), "OK"))
            {
                ToolsConfigManager.Instance.SaveDataToIni();
                Close();
            }
        }

        private bool ShowBool(string label, bool value)
        {
            return EditorGUILayout.ToggleLeft(label, value);
        }

        public void OnEnable()
        {
            window = this;
            window.minSize = new Vector2(350, 250);
        }

        public void OnDisable()
        {
            window = null;
        }
    }
}