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

        public static VariableInfo GetVariableInfoFromUIExportData(UIExportData data)
        {
            VariableInfo info = new VariableInfo();
            info.type = GetVariableType(data);
            info.constStrVaiableName = "_" + data.VariableName + "_Name";
            info.privateVariableName = "_" + data.VariableName;
            info.variableName = data.VariableName;
            info.isGameObjectReference = data.CompReference == null;
            if (!info.isGameObjectReference)
                info.fullType = data.CompReference.GetType().FullName.Replace("UnityEngine.", "");

            return info;
        }

        /// <summary>
        /// 获取变量的简称类型
        /// </summary>
        private static string GetVariableType(UIExportData data)
        {
            string type = "";
            if (data.CompReference == null)
            {
                type = "GameObject";
            }
            else
            {
                type = data.CompReference.GetType().Name;
            }
            return type;
        }
    }
}
