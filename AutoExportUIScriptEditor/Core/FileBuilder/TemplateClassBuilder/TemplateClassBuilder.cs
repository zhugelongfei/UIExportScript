using System.Collections.Generic;

namespace AutoExportScriptData
{
    internal class TemplateClassBuilder : AClassFileBuilder
    {
        public override void CreateFile(string ClassName, string filePath,
            Dictionary<string, List<UIExportData>> dic_ClassAndVariables)
        {
            VelocityEngineHandle velocity = new VelocityEngineHandle();
            velocity.Init();

            Dictionary<string, List<VariableInfo>> varInfoDic = new Dictionary<string, List<VariableInfo>>(dic_ClassAndVariables.Count);

            foreach (var classVarData in dic_ClassAndVariables)
            {
                List<VariableInfo> varInfoList = new List<VariableInfo>(classVarData.Value.Count);
                foreach (UIExportData variable in classVarData.Value)
                {
                    varInfoList.Add(VariableInfo.GetVariableInfoFromUIExportData(variable));
                }
                varInfoDic.Add(classVarData.Key, varInfoList);
            }

            velocity.AddValue("useGetAttribute", ToolsConfigManager.Instance.UseGetAttribute);
            velocity.AddValue("dataSafeCheck", ToolsConfigManager.Instance.DataSafeCheck);
            velocity.AddValue("codeStyle", ToolsConfigManager.Instance.UIDataCodeStyle);
            velocity.AddValue("classDic", varInfoDic);
            velocity.ExecuteFile(filePath, ClassName, ".cs");
        }
    }
}
