using System;
using System.Collections.Generic;
using NVelocity;
using NVelocity.App;
using NVelocity.Runtime;
using System.IO;

namespace AutoExportScriptData
{
    internal class VelocityEngineHandle
    {
        private VelocityContext context = null;
        private Template template = null;
        private Dictionary<string, object> keyValueDic = new Dictionary<string, object>();

        /// <summary>
        /// 初始化模板引擎
        /// </summary>
        public void Init()
        {
            try
            {
                VelocityEngine templateEngine = new VelocityEngine();
                templateEngine.SetProperty(RuntimeConstants.RESOURCE_LOADER, "file");

                templateEngine.SetProperty(RuntimeConstants.INPUT_ENCODING, "utf-8");
                templateEngine.SetProperty(RuntimeConstants.OUTPUT_ENCODING, "utf-8");

                //如果设置了FILE_RESOURCE_LOADER_PATH属性，那么模板文件的基础路径就是基于这个设置的目录，
                //否则默认当前运行目录
                templateEngine.SetProperty(RuntimeConstants.FILE_RESOURCE_LOADER_PATH,
                    UnityEngine.Application.dataPath);

                templateEngine.Init();

                template = templateEngine.GetTemplate(FilePathManager.Instance.templateFilePath);
            }
            catch (NVelocity.Exception.ResourceNotFoundException re)
            {
                string error = string.Format("Cannot find template " + "");

                Console.WriteLine(error);
                throw new Exception(error, re);
            }
            catch (NVelocity.Exception.ParseErrorException pee)
            {
                string error = string.Format("Syntax error in template " + "" + ":" + pee.StackTrace);
                Console.WriteLine(error);
                throw new Exception(error, pee);
            }
        }

        /// <summary>
        /// 向上下文中添加键值对
        /// </summary>
        public void AddValue(string key, object value)
        {
            if (keyValueDic.ContainsKey(key))
                return;
            keyValueDic.Add(key, value);
        }

        /// <summary>
        /// 初始化上下文的内容
        /// </summary>
        private void InitContext()
        {
            context = new VelocityContext();
            foreach (string key in keyValueDic.Keys)
            {
                context.Put(key, keyValueDic[key]);
            }
        }

        /// <summary>
        /// 根据模板创建输出的文件,并返回生成的文件路径
        /// </summary>
        /// <param name="outputFilePath">路径</param>
        /// <param name="outputFileName">文件名</param>
        /// <param name="outputFileExtension">文件后缀</param>
        public void ExecuteFile(string outputFilePath, string outputFileName, string outputFileExtension)
        {
            string fileName = "";
            if (template != null)
            {
                fileName = outputFilePath + outputFileName + outputFileExtension;

                if (!string.IsNullOrEmpty(outputFilePath) && !Directory.Exists(outputFilePath))
                {
                    Directory.CreateDirectory(outputFilePath);
                }

                InitContext();
                using (StreamWriter writer = new StreamWriter(fileName, false, System.Text.Encoding.UTF8))
                {
                    template.Merge(context, writer);
                }
            }
        }

    }
}