using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AutoExportScriptData
{
    internal class ExportScriptTools
    {
        private const string regex = "^[_a-zA-Z]+[_0-9a-zA-Z]*$";

        // 生成的主类名(等于文件名)
        private string ClassName;
        private UIExportScript export;

        public ExportScriptTools(UIExportScript export)
        {
            this.export = export;
            ClassName = export.ClassName;
        }

        public void ExportScript(string filePath)
        {
            //1：类名检测
            if (!CheckVariableOrClassName(ClassName))
            {
                throw new UIExportDataException("ClassName is null or illegal !");
            }
            Dictionary<string, List<UIExportData>> dic_ClassAndVariables = new Dictionary<string, List<UIExportData>>();
            dic_ClassAndVariables.Add(ClassName, new List<UIExportData>());

            //2：获取所有导出数据
            UIProgramData[] ProgramDatas = export.GetComponentsInChildren<UIProgramData>(true);
            if (null == ProgramDatas || ProgramDatas.Length == 0)
            {
                throw new UIExportDataException("Can't find the UIProgramData from all game object !");
            }

            //过滤不导出的数据
            for (int i = 0; i < ProgramDatas.Length; i++)
            {
                if (ProgramDatas[i].notExport)
                {
                    ProgramDatas[i] = null;
                }
            }

            //3：寻找所有创建的其他类
            FindAllClassNameSaveToDic(ProgramDatas, dic_ClassAndVariables);

            //4：根据导出数据的类名分类
            FindAllVariablesSaveToDic(ProgramDatas, dic_ClassAndVariables);

            AClassFileBuilder gcf = new TemplateClassBuilder();
            gcf.CreateFile(ClassName, filePath, dic_ClassAndVariables);
        }

        /// <summary>
        /// 找出所有需要创建的类，把名字存储到字典的键中
        /// </summary>
        private void FindAllClassNameSaveToDic(UIProgramData[] ProgramDatas, Dictionary<string, List<UIExportData>> dic_ClassAndVariables)
        {
            foreach (UIProgramData data in ProgramDatas)
            {
                if (data == null) continue;

                if (!string.IsNullOrEmpty(data.CreateClassName))
                {
                    if (!dic_ClassAndVariables.ContainsKey(data.CreateClassName))
                    {
                        //证明类名不存在字典中，可以加入
                        dic_ClassAndVariables.Add(data.CreateClassName, new List<UIExportData>());
                    }
                    else
                    {
                        //证明字典中已经有这个类名，查找到类名所在的物体，抛出异常
                        if (data.CreateClassName.Equals(ClassName))
                        {
                            //证明这个类的创建名字是和主类相同
                            throw new UIExportDataException(string.Format("This game object has the same create class name with main and class name is {0}!",
                                data.CreateClassName), data);
                        }
                        else
                        {
                            UIProgramData sameNameObj = null;
                            foreach (var item in ProgramDatas)
                            {
                                if (item != null && item != data && item.CreateClassName.Equals(data.CreateClassName))
                                {
                                    sameNameObj = item;
                                    break;
                                }
                            }
                            throw new UIExportDataException(string.Format("There are two game object has the same create class name and class name is {0}!",
                                data.CreateClassName), data, sameNameObj);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 找出所有类包含的变量，存储到对应的值中
        /// </summary>
        private void FindAllVariablesSaveToDic(UIProgramData[] ProgramDatas, Dictionary<string, List<UIExportData>> dic_ClassAndVariables)
        {
            foreach (UIProgramData data in ProgramDatas)
            {
                if (data == null) continue;

                if (!string.IsNullOrEmpty(data.CreateClassName))
                {
                    //证明这个UIProgramData需要创建一个其他类
                    AddContainerToVariablesList(data.CreateClassName, data, ProgramDatas, dic_ClassAndVariables);
                }

                if (string.IsNullOrEmpty(data.LocalClassName))
                {
                    //证明这个变量是主类中的变量
                    AddContainerToVariablesList(ClassName, data, ProgramDatas, dic_ClassAndVariables);
                }
                else if (!data.CreateClassName.Equals(data.LocalClassName))
                {
                    //证明这个变量是一个新增类的变量
                    AddContainerToVariablesList(data.LocalClassName, data, ProgramDatas, dic_ClassAndVariables);
                }
            }
        }

        /// <summary>
        /// 将UIProgramData中的变量添加到数组中
        /// </summary>
        private void AddContainerToVariablesList(string key, UIProgramData pData, UIProgramData[] ProgramDatas, Dictionary<string, List<UIExportData>> dic_ClassAndVariables)
        {
            //1：检测变量数据是否合法
            CheckVariableData(pData);

            if (dic_ClassAndVariables.ContainsKey(key))
            {
                List<UIExportData> dataList = dic_ClassAndVariables[key];
                foreach (var data in pData.ExportData)
                {
                    UIExportData sameData = dataList.Find(d => d.VariableName.Equals(data.VariableName));
                    if (sameData != null)
                    {
                        //证明已存在相同变量名，那就找出这个物体，抛异常
                        UIProgramData hasObj = null;
                        foreach (var item in ProgramDatas)
                        {
                            if (item == pData) continue;
                            foreach (var exportData in item.ExportData)
                            {
                                if (exportData == sameData)
                                {
                                    hasObj = item;
                                    goto throwLine;
                                }
                            }
                        }
                    throwLine:
                        throw new UIExportDataException(string.Format("The variables has same and variable name is {0} !",
                            data.VariableName), pData, hasObj);
                    }
                    dataList.Add(data);
                }
            }
            else
            {
                throw new UIExportDataException(string.Format("Don't has this class and class name is {0}", key), pData);
            }
        }

        /// <summary>
        /// 检测变量数据是否合法
        /// </summary>
        private void CheckVariableData(UIProgramData pData)
        {
            for (int i = 0; i < pData.ExportData.Length; i++)
            {
                UIExportData data = pData.ExportData[i];
                if (data == null) continue;

                //检测数据
                if (data.isArrayData)
                {
                    //检测数组是否为Null或者数组内有Null
                    bool isErrorData = false;
                    if (data.CompReferenceArray == null)
                    {
                        isErrorData = true;
                    }
                    else
                    {
                        foreach (var item in data.CompReferenceArray)
                        {
                            if (item == null)
                            {
                                isErrorData = true;
                                break;
                            }
                        }
                    }
                    if (isErrorData)
                    {
                        throw new UIExportDataException(string.Format("Variable data error.It is array data,but array is null or element is null. Variable name is {0} !",
                            data.VariableName), pData);
                    }
                }
                else
                {
                    //非数组，检测引用是否为Null
                    if (data.CompReference == null)
                    {
                        throw new UIExportDataException(string.Format("Variable data error.It is null. Variable name is {0} !",
                            data.VariableName), pData);
                    }
                }

                //检测变量名
                if (!CheckVariableOrClassName(data.VariableName))
                {
                    throw new UIExportDataException(string.Format("Variable name error.It is null or has blank space and variable name is {0} !",
                        data.VariableName), pData);
                }
            }
        }

        /// <summary>
        /// 检测命名是否合法
        /// </summary>
        /// <param name="name">命名</param>
        /// <returns>是否合法</returns>
        private bool CheckVariableOrClassName(string name)
        {
            return !string.IsNullOrEmpty(ClassName) && Regex.IsMatch(name, regex);
        }
    }
}