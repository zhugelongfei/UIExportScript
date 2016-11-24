﻿using UnityEngine;
using UnityEditor;
using AutoExportScriptData;
using System.Collections.Generic;

public class OutLineWindow : ShowUIProgramDataBaseWindow<OutLineWindow.OutLineWindowData>
{
    public class OutLineWindowData
    {
        public string clsName;
    }

    public static string windowName = "Data Out Line";

    public new void OnGUI()
    {
        if (base.OnGUI())
            return;

        //Format
        Dictionary<string, List<UIProgramData>> className_Data_Dic = FormatUIProgramDataArrayToDic(pDataArray);

        foreach (var item in className_Data_Dic)
        {
            //Draw class name
            DrawParagraphSplitSymbol();
            GUILayout.Label("ClassName:" + item.Key);
            foreach (UIProgramData curObj in item.Value)
            {
                EditorGUILayout.BeginHorizontal();
                //Target game object
                EditorGUILayout.ObjectField(curObj, curObj.GetType(), true, fieldWidthOption);
                //Variables
                foreach (UIExportData data in curObj.ExportData)
                {
                    string type = data.CompReference == null ? "GameObject" : data.CompReference.GetType().Name;

                    DrawSplitChar();
                    GUILayout.Label(type, fieldWidthOption);

                    DrawSplitChar();
                    GUILayout.Label(data.VariableName, fieldWidthOption);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
    }

    private Dictionary<string, List<UIProgramData>> FormatUIProgramDataArrayToDic(UIProgramData[] pDataArray)
    {
        Dictionary<string, List<UIProgramData>> className_Data_Dic = new Dictionary<string, List<UIProgramData>>();
        for (int i = 0; i < pDataArray.Length; i++)
        {
            UIProgramData curObj = pDataArray[i];
            if (curObj == null) continue;

            if (!string.IsNullOrEmpty(curObj.CreateClassName))
            {
                AddValue(className_Data_Dic, curObj.CreateClassName, curObj);
            }

            foreach (UIExportData data in curObj.ExportData)
            {
                string className = string.IsNullOrEmpty(curObj.LocalClassName) ? this.data.clsName : curObj.LocalClassName;

                AddValue(className_Data_Dic, className, curObj);
            }
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