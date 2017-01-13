using UnityEngine;

namespace AutoExportScriptData
{
    internal class ErrorWindow : ShowUIProgramDataBaseWindow<ErrorWindow.ErrorWindowData>
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

                DrawUIProgramData(curObj);
            }
        }

    }
}