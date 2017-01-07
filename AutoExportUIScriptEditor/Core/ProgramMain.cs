using UnityEditor;

[InitializeOnLoad]
public class ProgramMain
{
    /// <summary>
    /// 静态构造函数，初始化作用
    /// </summary>
    static ProgramMain()
    {
        ComponentHierarchyIcon<AutoExportScriptData.UIProgramData>.Init();
    }
}