using UnityEngine;
using UnityEditor;
using AutoExportScriptData;

public class ErrorWindow : ShowUIProgramDataBaseWindow<ErrorWindow.ErrorWindowData>
{
    public class ErrorWindowData
    {
        public string clsName;
        public string errorLog;
    }

    public static string windowName = "Data Error";

    public new void OnGUI()
    {
        if (base.OnGUI())
            return;

        DrawParagraphSplitSymbol();

        GUILayout.Label(data.errorLog);

        //List for variable
        for (int i = 0; i < pDataArray.Length; i++)
        {
            UIProgramData curObj = pDataArray[i];
            if (curObj == null) continue;
            EditorGUILayout.BeginHorizontal();

            //Target game object
            EditorGUILayout.ObjectField(curObj, curObj.GetType(), true, fieldWidthOption);

            //Variables
            foreach (UIExportData data in curObj.ExportData)
            {
                string className = string.IsNullOrEmpty(curObj.LocalClassName) ? this.data.clsName : curObj.LocalClassName;
                string type = data.CompReference == null ? "GameObject" : data.CompReference.GetType().Name;

                DrawSplitChar();
                GUILayout.Label(type, fieldWidthOption);

                DrawSplitChar();
                GUILayout.Label(className + "." + data.VariableName, fieldWidthOption);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

}