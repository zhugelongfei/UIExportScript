using System.Collections.Generic;

namespace AutoExportScriptData
{
    internal abstract class AClassFileBuilder
    {
        public abstract void CreateFile(string filePath, string ClassName,
           Dictionary<string, List<UIExportData>> dic_ClassAndVariables);
    }
}