using UnityEngine;

namespace AutoExportScriptData
{
    [System.Serializable]
    public class UIExportData
    {
        public bool getGameObject = false;

        public bool isArrayData = false;
        public Component[] CompReferenceArray = null;

        public Component CompReference = null;      //组件引用，如果为NULL，则去获取GameObject
        public string VariableName = null;          //变量名
    }

    [AddComponentMenu("UIExpand/Program Data")]
    public class UIProgramData : MonoBehaviour
    {
        public delegate void SetUIProgramDataCompReference(UIProgramData programData, UIExportData exportData);

        public bool notExport = false;            //是否不导出此物体
        public string CreateClassName = null;       //需要生成的类的名字
        public string LocalClassName = null;        //所在类的名字

        public UIExportData[] ExportData = null;    //所有要导出的数据

        public static void ForEachData(GameObject thisObjRoot, SetUIProgramDataCompReference handle)
        {
            if (handle == null || thisObjRoot == null)
                return;

            UIProgramData[] allData = thisObjRoot.GetComponentsInChildren<UIProgramData>(true);
            if (null == allData || allData.Length <= 0)
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