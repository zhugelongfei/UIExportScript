using UnityEngine;
using UnityEditor;
using AutoExportScriptData;

public class ShowUIProgramDataBaseWindow<D> : EditorWindow
{
    protected static ShowUIProgramDataBaseWindow<D> window = null;

    protected const float border = 20;
    protected const float fieldWidth = 150;
    protected GUILayoutOption fieldWidthOption = GUILayout.Width(fieldWidth);

    protected UIProgramData[] pDataArray;
    protected D data;

    private static GUIStyle splitTextStyle = new GUIStyle();
    private const float separatorWidth = 5;
    private GUILayoutOption separatorWidthOption = GUILayout.Width(separatorWidth);

    private int mvCount = 0;
    private int maxVariableCount
    {
        get { return mvCount; }
        set
        {
            if (mvCount != value)
            {
                mvCount = value;
                if (window != null)
                    window.minSize = new Vector2((fieldWidth + 2) * (1 + mvCount * 2) + (mvCount * 2) * (separatorWidth + 2) + border, 300);
            }
        }
    }

    public static void Open<T>(string windowName, UIProgramData[] pDataArray, D data)
        where T : ShowUIProgramDataBaseWindow<D>
    {
        splitTextStyle.normal.textColor = Color.red;

        if (window == null)
        {
            window = GetWindow<T>(false, windowName, true);
            window.Show();
            window.pDataArray = pDataArray;
            window.data = data;
        }
    }

    public bool OnGUI()
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
                    return true;
                }
            }
        }

        //Processor null data
        if (pDataArray == null)
        {
            Close();
            return true;
        }

        //Calculate max variable count
        for (int i = 0; i < pDataArray.Length; i++)
        {
            UIProgramData curObj = pDataArray[i];
            if (curObj == null) continue;
            maxVariableCount = Mathf.Max(curObj.ExportData.Length, maxVariableCount);
        }

        //Title
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Game Object", fieldWidthOption);
        for (int i = 0; i < maxVariableCount; i++)
        {
            DrawSplitChar();
            GUILayout.Label("Component Type " + (i + 1).ToString(), fieldWidthOption);
            DrawSplitChar();
            GUILayout.Label("Variable Name " + (i + 1).ToString(), fieldWidthOption);
        }
        EditorGUILayout.EndHorizontal();

        return false;
    }

    protected void DrawUIProgramData(UIProgramData pData)
    {
        EditorGUILayout.BeginHorizontal();
        //Target game object
        EditorGUILayout.ObjectField(pData, pData.GetType(), true, fieldWidthOption);
        //Variables
        foreach (UIExportData data in pData.ExportData)
        {
            string type = "Null";

            if (data.getGameObject)
            {
                if (data.isArrayData)
                    type = "GameObject[]";
                else
                    type = "GameObject";
            }
            else
            {
                if (data.isArrayData)
                {
                    if (data.CompReferenceArray == null || data.CompReferenceArray.Length == 0 || data.CompReferenceArray[0] == null)
                        type = "ArrayTypeError";
                    else
                        type = data.CompReferenceArray[0].GetType().Name + "[]";
                }
                else
                {
                    type = data.CompReference.GetType().Name;
                }
            }

            DrawSplitChar();
            GUILayout.Label(type, fieldWidthOption);

            DrawSplitChar();
            GUILayout.Label(data.VariableName, fieldWidthOption);
        }
        EditorGUILayout.EndHorizontal();
    }

    protected void DrawSplitChar()
    {
        GUILayout.Label("|", splitTextStyle, separatorWidthOption);
    }

    protected void DrawParagraphSplitSymbol()
    {
        GUILayout.Label("");
    }

    public void OnEnable()
    {
        window = this;
    }

    public void OnDisable()
    {
        window = null;
        pDataArray = null;
    }
}