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

        private static GUIStyle noteStyle = null;

        private bool editEnable = false;

        private ToolsConfigManager toolCfg = null;

        [MenuItem("AutoExportUIScript/ConfigWindow")]
        public static void Open()
        {
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
            GUILayout.Label("AutoExportUIScript Global Settings", EditorStyles.boldLabel);

            editEnable = EditorGUILayout.BeginToggleGroup("Option Setting", editEnable);

            toolCfg.IsShowUIProgramDataHierarchyIcon = ShowBool("Hierarchy面板显示UIProgramData的状态", toolCfg.IsShowUIProgramDataHierarchyIcon);

            toolCfg.UseGetAttribute = EditorGUILayout.BeginToggleGroup("导出字段使用Get属性（保证数据的只读性）", toolCfg.UseGetAttribute);

            if (!toolCfg.UseGetAttribute)
            {
                GUILayout.Label("     Note:不使用Get可以提升字段的访问速度。", noteStyle);
                GUILayout.Label("     但是一定要注意，不要在外部更改这个字段", noteStyle);
            }
            EditorGUI.indentLevel += 1;
            toolCfg.DataSafeCheck = ShowBool("检测数据安全性（如果字段为空，则会返回一个新的实例）", toolCfg.DataSafeCheck);
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndToggleGroup();

            toolCfg.OpenGenerateGameObjectRef = ShowBool("Inspector显示Is Game Object Ref", toolCfg.OpenGenerateGameObjectRef);

            toolCfg.OpenGenerateArrayRef = ShowBool("Inspector显示Is Array Data", toolCfg.OpenGenerateArrayRef);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("代码类型：", GUILayout.Width(60.0f));
            toolCfg.UIDataCodeStyle = (ToolsConfigManager.CodeStyle)EditorGUILayout.EnumPopup("", toolCfg.UIDataCodeStyle, GUILayout.Width(100.0f));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndToggleGroup();

            if (GUI.Button(new Rect(position.xMax - position.xMin - OK_BTN_WIDTH - OK_BTN_BORDER, position.yMax - position.yMin - OK_BTN_HEIGHT - OK_BTN_BORDER, OK_BTN_WIDTH, OK_BTN_HEIGHT), "OK"))
            {
                toolCfg.SaveDataToIni();
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
            toolCfg = ToolsConfigManager.Instance;

            noteStyle = new GUIStyle();
            noteStyle.normal.textColor = Color.red;
            noteStyle.fontStyle = FontStyle.Bold;
        }

        public void OnDisable()
        {
            window = null;
            toolCfg = null;
            noteStyle = null;
        }
    }
}