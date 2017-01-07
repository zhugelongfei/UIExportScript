using AutoExportScriptData;
using System.IO;

public class ToolsConfigManager
{
    #region 单例
    private static ToolsConfigManager instance;
    public static ToolsConfigManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ToolsConfigManager();
            return instance;
        }
    }

    private ToolsConfigManager()
    {
        IniFile ini = FilePathManager.Instance.GetIniConfig();
        //ini.WriteObject(Section, data);


        ini.ReadObject(Section, data);
    }

    #endregion

    private ToolsConfigData data = new ToolsConfigData();

    public bool IsShowUIProgramDataHierarchyIcon
    {
        get { return data.isShowUIProgramDataHierarchyIcon; }
        set
        {
            if (data.isShowUIProgramDataHierarchyIcon == value)
                return;

            data.isShowUIProgramDataHierarchyIcon = value;
            if (data.isShowUIProgramDataHierarchyIcon)
            {
                ComponentHierarchyIcon<UIProgramData>.Open();
            }
            else
            {
                ComponentHierarchyIcon<UIProgramData>.Close();
            }
        }
    }

    private const string Section = "ToolsConfig";
    private string iniPath = UnityEngine.Application.dataPath + @"\Editor\UIScriptBuilder\UIExportScriptsConfig.ini";

    public void Write()
    {
        File.Delete(iniPath);
        IniFile ini = new IniFile(iniPath);
        ini.WriteObject(Section, this);
    }

    public void ReadData()
    {
        IniFile ini = FilePathManager.Instance.GetIniConfig();
        ini.ReadObject(Section, data);
    }

}