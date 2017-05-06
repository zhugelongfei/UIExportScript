using UnityEngine;
using System.Collections.Generic;

namespace AutoExportScriptData
{
    internal class OutLineWindow : ShowUIProgramDataBaseWindow<OutLineWindow.OutLineWindowData>
    {
        public class OutLineWindowData
        {
            public string clsName;
        }

        public static string windowName = "Data Out Line";
        private Vector2 viewPos;
        private string searchTxt = "";

        public new void OnGUI()
        {
            GUILayout.Label("Search:");
            searchTxt = GUILayout.TextField(searchTxt, GUILayout.Width(200));
            DrawParagraphSplitSymbol();

            if (base.OnGUI())
                return;

            //Format
            Dictionary<string, List<UIProgramData>> className_Data_Dic = FormatUIProgramDataArrayToDic(pDataArray);

            bool isSearchTarget = false;

            viewPos = GUILayout.BeginScrollView(viewPos);

            foreach (var item in className_Data_Dic)
            {
                //Draw class name
                DrawParagraphSplitSymbol();
                GUILayout.Label("ClassName:" + item.Key);
                foreach (UIProgramData curObj in item.Value)
                {
                    if (string.IsNullOrEmpty(searchTxt))
                    {
                        //Not find
                        DrawUIProgramData(curObj);
                    }
                    else
                    {
                        //Find search target
                        isSearchTarget = false;

                        foreach (var exportData in curObj.ExportData)
                        {
                            if (exportData.VariableName.Contains(searchTxt))
                            {
                                isSearchTarget = true;
                                break;
                            }
                        }
                        if (isSearchTarget)
                            DrawUIProgramData(curObj);
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        private Dictionary<string, List<UIProgramData>> FormatUIProgramDataArrayToDic(UIProgramData[] pDataArray)
        {
            Dictionary<string, List<UIProgramData>> className_Data_Dic = new Dictionary<string, List<UIProgramData>>();
            for (int i = 0; i < pDataArray.Length; i++)
            {
                UIProgramData curObj = pDataArray[i];
                if (curObj == null || curObj.notExport) continue;

                if (!string.IsNullOrEmpty(curObj.CreateClassName))
                {
                    AddValue(className_Data_Dic, curObj.CreateClassName, curObj);
                }

                string className = string.IsNullOrEmpty(curObj.LocalClassName) ? this.data.clsName : curObj.LocalClassName;

                AddValue(className_Data_Dic, className, curObj);
            }
            return className_Data_Dic;
        }

        private void AddValue(Dictionary<string, List<UIProgramData>> className_Data_Dic, string key, UIProgramData value)
        {
            if (!className_Data_Dic.ContainsKey(key))
            {
                className_Data_Dic[key] = new List<UIProgramData>();
            }
            className_Data_Dic[key].Add(value);
        }
    }
}