namespace AutoExportScriptData
{
    internal class VariableInfo
    {
        public string type { get; set; }
        public string constStrVaiableName { get; set; }
        public string privateVariableName { get; set; }
        public string variableName { get; set; }
        public bool isGameObjectReference { get; set; }
        public string fullType { get; set; }
        public bool isArrayRef { get; set; }

        /// <summary>
        /// 根据UIExportData信息生成VariableInfo
        /// </summary>
        /// <param name="data">导出数据</param>
        /// <returns>变量信息</returns>
        public static VariableInfo GetVariableInfoFromUIExportData(UIExportData data)
        {
            VariableInfo info = new VariableInfo();

            info.isArrayRef = data.isArrayData;
            info.isGameObjectReference = data.isGameObjectRef;

            info.variableName = data.VariableName;
            info.privateVariableName = "_" + data.VariableName;
            info.constStrVaiableName = "_" + data.VariableName + "_Name";

            string arrayType = "[]";
            System.Type compType = null;

            if (data.isArrayData)
            {
                compType = data.CompReferenceArray[0].GetType();
                arrayType = "[]";
            }
            else
            {
                compType = data.CompReference.GetType();
                arrayType = "";
            }

            if (data.isGameObjectRef)
            {
                info.fullType = "GameObject";
            }
            else
            {
                info.fullType = compType.FullName.Replace("UnityEngine.", "");
            }
            info.type = info.fullType + arrayType;

            return info;
        }
    }
}
