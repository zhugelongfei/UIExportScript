using UnityEditor;

namespace AutoExportScriptData
{
    [InitializeOnLoad]
    internal class ProgramMain
    {
        /// <summary>
        /// 静态构造函数，初始化作用
        /// </summary>
        static ProgramMain()
        {
            ComponentHierarchyIcon<UIProgramData>.Init();
        }
    }
}