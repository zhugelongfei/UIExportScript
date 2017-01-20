using UnityEngine;

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
        public static void ForEachData(GameObject thisObjRoot, SetUIProgramDataCompReference handle)
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
    }
}