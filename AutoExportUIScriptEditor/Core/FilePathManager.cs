using System.IO;

namespace AutoExportScriptData
{
    internal class FilePathManager
    {
        private static FilePathManager instance;

        public static FilePathManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new FilePathManager();
                return instance;
            }
        }

        private FilePathManager() { ReadData(); }

        //CSC编译工具的路径
        private string cscPath = null;

        //代码生成路径-放在编辑器内的路径
        private string assetsInsideScriptPath = null;

        //生成的DLL文件名称
        private string dllFileRelativeFullName = null;

        //代码生成路径-编译为DLL的脚本存放路径
        private string dllScriptPath = null;

        //依赖DLL的路径
        private string referenceLibFilePath = null;

        //模板文件路径
        public string templateFilePath = null;

        //标记图标路径
        public string starIconFilePath = null;

        private const string Section = "FilePathConfig";
        private string iniPath = UnityEngine.Application.dataPath + @"\Editor\UIScriptBuilder\UIExportScriptsConfig.ini";

        public IniFile GetIniConfig()
        {
            return new IniFile(iniPath);
        }

        public void Write()
        {
            File.Delete(iniPath);
            IniFile ini = GetIniConfig();
            ini.WriteObject(Section, this);
        }

        public void ReadData()
        {
            IniFile ini = GetIniConfig();
            ini.ReadObject(Section, this);
        }

        /// <summary>
        /// 生成文件的路径
        /// </summary>
        public string GenerateFilePath(bool isGenDllScript, string ClassName, ref bool isNeedReGenDll)
        {
            isNeedReGenDll = false;
            if (!Directory.Exists(dllScriptPath))
                Directory.CreateDirectory(dllScriptPath);

            if (!Directory.Exists(assetsInsideScriptPath))
                Directory.CreateDirectory(assetsInsideScriptPath);

            //生成脚本路径
            string filePath = isGenDllScript ? dllScriptPath : assetsInsideScriptPath;

            //删除另外一个路径存在的同名文件
            string otherFileName = (isGenDllScript ? assetsInsideScriptPath : dllScriptPath) + ClassName + ".cs";
            if (File.Exists(otherFileName))
            {
                File.Delete(otherFileName);
                if (!isGenDllScript)
                {
                    isNeedReGenDll = true;
                }
            }
            return filePath;
        }

        /// <summary>
        /// 检测是否可以编译DLL
        /// </summary>
        public bool CheckCanCompileDll(string assetPath)
        {
            string[] fileNames = Directory.GetFiles(dllScriptPath, "*.cs");
            if (fileNames == null || fileNames.Length == 0)
            {
                //没有要编译的文件，删除DLL
                string filePath = assetPath + dllFileRelativeFullName;
                File.Delete(filePath);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取编译信息
        /// </summary>
        public CompileCSharpDll.CompileFileInfo GetCompileConfigInfo()
        {
            CompileCSharpDll.CompileFileInfo info = new CompileCSharpDll.CompileFileInfo();
            info.compileFilePath = Directory.GetCurrentDirectory() + @"\" + dllScriptPath;
            info.searchPattern = "*.cs";
            info.utilFilePath = cscPath;
            info.dllOutPath = UnityEngine.Application.dataPath + dllFileRelativeFullName;

            string libPath = Directory.GetCurrentDirectory() + referenceLibFilePath;
            info.referenceDllFilePath = Directory.GetFiles(libPath, "*.dll");

            return info;
        }
    }
}