namespace AutoExportScriptData
{
    internal class ToolsConfigManager
    {
        private class ToolsConfigData
        {
            /// <summary>
            /// 是否在Hierarchy面板的物体上显示UIProgramData的特殊标识
            /// </summary>
            public bool isShowUIProgramDataHierarchyIcon = false;

            /// <summary>
            /// 是否开启生成GameObject的引用功能
            /// </summary>
            public bool openGenerateGameObjectRef = true;

            /// <summary>
            /// 是否开启生成引用数组的功能
            /// </summary>
            public bool openGenerateArrayRef = true;

            /// <summary>
            /// Get属性内的安全监测
            /// </summary>
            public bool dataSafeCheck = false;

            /// <summary>
            /// 使用Get属性，保证数据只读性
            /// </summary>
            public bool useGetAttribute = false;

        }

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
            ini.ReadObject(Section, data);
        }

        #endregion

        private const string Section = "ToolsConfig";
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
                    UIProgramDataHierarchyIcon.Open();
                }
                else
                {
                    UIProgramDataHierarchyIcon.Close();
                }
            }
        }

        public bool OpenGenerateGameObjectRef
        {
            get { return data.openGenerateGameObjectRef; }
            set
            {
                if (data.openGenerateGameObjectRef == value)
                    return;

                data.openGenerateGameObjectRef = value;
            }
        }

        public bool OpenGenerateArrayRef
        {
            get { return data.openGenerateArrayRef; }
            set
            {
                if (data.openGenerateArrayRef == value)
                    return;

                data.openGenerateArrayRef = value;
            }
        }

        public bool UseGetAttribute
        {
            get { return data.useGetAttribute; }
            set
            {
                if (data.useGetAttribute == value)
                    return;

                data.useGetAttribute = value;
            }
        }

        public bool DataSafeCheck
        {
            get { return data.dataSafeCheck; }
            set
            {
                if (data.dataSafeCheck == value)
                    return;

                data.dataSafeCheck = value;
            }
        }

        public void SaveDataToIni()
        {
            IniFile ini = FilePathManager.Instance.GetIniConfig();
            ini.WriteObject(Section, data);
        }

    }
}