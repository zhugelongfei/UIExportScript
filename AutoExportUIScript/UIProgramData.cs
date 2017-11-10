using UnityEngine;
using System.Collections.Generic;

namespace AutoExportScriptData
{
    [System.Serializable]
    public class UIExportData
    {
        public bool isGameObjectRef = false;                //是否是GameObject的引用

        public bool isArrayData = false;                    //是否是数组引用
        public Component[] CompReferenceArray = null;       //数组组件数据

        public Component CompReference = null;              //组件引用
        public string VariableName = null;                  //变量名
    }

    [AddComponentMenu("UIExpand/Program Data")]
    public class UIProgramData : MonoBehaviour
    {
        public delegate void SetUIProgramDataCompReference(UIProgramData programData, UIExportData exportData);

        public bool notExport = false;              //是否不导出此物体
        public string CreateClassName = null;       //需要生成的类的名字
        public string LocalClassName = null;        //所在类的名字

        public UIExportData[] ExportData = null;    //所有要导出的数据

        /// <summary>
        /// 遍历指定物体节点下的所有UIProgramData，然后根据回调进行引用赋值
        /// </summary>
        /// <param name="thisObjRoot">物体根节点</param>
        /// <param name="handle">赋值回调</param>
        public static void ForEachDataByCode(GameObject thisObjRoot, SetUIProgramDataCompReference handle)
        {
            if (handle == null || thisObjRoot == null)
                return;

            UIProgramData[] allData = thisObjRoot.GetComponentsInChildren<UIProgramData>(true);
            if (null == allData || allData.Length == 0)
                return;

            for (int iLoop = 0; iLoop < allData.Length; iLoop++)
            {
                UIProgramData programData = allData[iLoop];
                if (null != programData.ExportData && programData.ExportData.Length > 0)
                {
                    for (int iIndex = 0; iIndex < programData.ExportData.Length; iIndex++)
                    {
                        UIExportData exportData = programData.ExportData[iIndex];
                        handle(programData, exportData);
                    }
                }
            }
        }

        /*
         * UIExportData 三种数据容器 1：Dic 2：List 3：LinkedList
         *      据测试表明，使用已经开辟连续控件List的速度要比其他二者要快很多。
         * 而LinkedList由于每次Add都去寻找碎片内存空间，所以速度是最慢的。其次
         * 是计算hash值的Dic。因一般UI的数据引用都不会超出1K，故使用List初始容
         * 器为1K即可。
         */
        public static List<UIExportData> dataList = new List<UIExportData>(1024);

        /// <summary>
        /// 遍历指定物体节点下的所有UIProgramData，然后根据字段名反射赋值
        /// </summary>
        /// <param name="rootObj">物体根节点</param>
        /// <param name="uiData">UIData对象</param>
        public static void ForEachDataByAttribute(GameObject rootObj, object uiData)
        {
            dataList.Clear();
            if (rootObj == null || uiData == null)
                return;

            UIProgramData[] allData = rootObj.GetComponentsInChildren<UIProgramData>(true);
            if (null == allData || allData.Length == 0)
                return;

            for (int iLoop = 0; iLoop < allData.Length; iLoop++)
            {
                UIProgramData programData = allData[iLoop];
                if (null != programData.ExportData && programData.ExportData.Length > 0)
                {
                    for (int iIndex = 0; iIndex < programData.ExportData.Length; iIndex++)
                    {
                        UIExportData exportData = programData.ExportData[iIndex];
                        dataList.Add(exportData);
                    }
                }
            }

            System.Reflection.FieldInfo[] fieldInfoArr = uiData.GetType().GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            foreach (var field in fieldInfoArr)
            {
                object[] objAtts = field.GetCustomAttributes(typeof(UIDataAttribute), false);
                if (objAtts != null && objAtts.Length > 0)
                {
                    for (int j = 0, jMax = objAtts.Length; j < jMax; j++)
                    {
                        if (objAtts[j] == null)
                            continue;
                        UIDataAttribute dataAtt = objAtts[j] as UIDataAttribute;

                        SetFieldValue(uiData, field, dataAtt);
                    }
                }
            }
        }

        private static void SetFieldValue(object uiData, System.Reflection.FieldInfo field, UIDataAttribute dataAtt)
        {
            for (int x = 0, xMax = dataList.Count; x < xMax; x++)
            {
                UIExportData exportData = dataList[x];
                if (exportData != null && exportData.VariableName.Equals(dataAtt.FieldName))
                {
                    if (exportData.isArrayData)
                    {
                        System.Array dataArr = null;
                        if (exportData.isGameObjectRef)
                        {
                            dataArr = System.Array.CreateInstance(typeof(GameObject), exportData.CompReferenceArray.Length);
                        }
                        else
                        {
                            dataArr = System.Array.CreateInstance(exportData.CompReferenceArray[0].GetType(), exportData.CompReferenceArray.Length);
                        }

                        for (int y = 0; y < exportData.CompReferenceArray.Length; y++)
                        {
                            if (exportData.isGameObjectRef)
                            {
                                dataArr.SetValue(exportData.CompReferenceArray[y].gameObject, y);
                            }
                            else
                            {
                                dataArr.SetValue(exportData.CompReferenceArray[y], y);
                            }
                        }
                        field.SetValue(uiData, dataArr);
                    }
                    else
                    {
                        if (exportData.isGameObjectRef)
                        {
                            field.SetValue(uiData, exportData.CompReference.gameObject);
                        }
                        else
                        {
                            field.SetValue(uiData, exportData.CompReference);
                        }
                    }
                    dataList[x] = null;
                    break;
                }
            }
        }
    }
}
