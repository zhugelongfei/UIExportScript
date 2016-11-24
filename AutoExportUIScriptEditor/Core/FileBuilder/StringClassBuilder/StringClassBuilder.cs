using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AutoExportScriptData
{
    internal class StringClassBuilder : AClassFileBuilder
    {
        //生成的代码所在的命名空间
        private const string nameSpace = "AutoExportScriptData";

        //引用命名空间
        private string[] HeaderArray = new string[] { "using UnityEngine;" };

        //拼接字符串时的缓存StringBuilder
        private StringBuilder strCache = new StringBuilder();

        //类文件写入流
        private ClassFileStream classWriter = null;

        /// <summary>
        /// 生成类文件
        /// </summary>
        public override void CreateFile(string ClassName, string filePath,
            Dictionary<string, List<UIExportData>> dic_ClassAndVariables)
        {
            string fileName = JointString(filePath, ClassName, ".cs");
            classWriter = new ClassFileStream(new StreamWriter(new FileStream(fileName, FileMode.Create), Encoding.UTF8));

            //写入引用命名空间
            foreach (var header in HeaderArray)
            {
                classWriter.WriteLine(header);
            }

            classWriter.WriteLine();

            //写入自己所在命名空间
            classWriter.WriteLine("namespace " + nameSpace);
            classWriter.WriteBraceLeft();

            //写入所有类
            foreach (var item in dic_ClassAndVariables)
            {
                CreateClass(item.Key, item.Value);
                classWriter.WriteLine();
            }

            classWriter.WriteLineBraceRight();

            classWriter.Flush();
            classWriter.Close();
            classWriter = null;
        }

        /// <summary>
        /// 生成单个类模块
        /// </summary>
        private void CreateClass(string className, List<UIExportData> datas)
        {
            classWriter.WriteLine("public partial class ", className);
            classWriter.WriteBraceLeft();

            #region 变量区

            classWriter.WriteLine("public GameObject thisObjRoot;");

            for (int i = 0; i < datas.Count; i++)
            {
                UIExportData data = datas[i];

                WriteVariable(data, className);
            }

            #endregion

            classWriter.WriteLine();

            #region 析构函数区

            classWriter.WriteLine("~", className, "()");
            classWriter.WriteBraceLeft();

            classWriter.WriteLine("Clear();");

            classWriter.WriteLineBraceRight();
            #endregion

            classWriter.WriteLine();

            #region Init方法区

            classWriter.WriteLine("public void Init(GameObject objRoot)");
            classWriter.WriteBraceLeft();//Start of init function
            {
                classWriter.WriteLine("if (objRoot == null)");
                classWriter.WriteLine(1, "return;");

                classWriter.WriteLine();

                classWriter.WriteLine("thisObjRoot = objRoot;");
                classWriter.WriteLine("thisObjRoot.SetActive(true);");

                classWriter.WriteLine();

                classWriter.WriteLine("UIProgramData[] allData = thisObjRoot.GetComponentsInChildren<UIProgramData>(true);");
                classWriter.WriteLine("if (null == allData || allData.Length <= 0)");
                classWriter.WriteLine(1, "return;");

                classWriter.WriteLine();

                #region 遍历UIProgramData，并为当前脚本的引用赋值

                classWriter.WriteLine("for (int iLoop = 0; iLoop < allData.Length; iLoop++)");
                classWriter.WriteBraceLeft();//Start of for UIProgramData
                {
                    classWriter.WriteLine("UIProgramData programData = allData[iLoop];");

                    classWriter.WriteLine("if (null != programData.ExportData && programData.ExportData.Length > 0)");
                    classWriter.WriteBraceLeft();//Start of if programData.ExportData
                    {
                        #region 遍历UIExportData

                        classWriter.WriteLine("for (int iIndex = 0; iIndex < programData.ExportData.Length; iIndex++)");
                        classWriter.WriteBraceLeft();
                        {
                            classWriter.WriteLine("UIExportData exportData = programData.ExportData[iIndex];");
                            classWriter.WriteLine("switch (exportData.VariableName)");
                            classWriter.WriteBraceLeft();
                            {
                                for (int i = 0; i < datas.Count; i++)
                                {
                                    UIExportData data = datas[i];
                                    classWriter.WriteLine("case ", "_", data.VariableName, "_Name", ":");
                                    classWriter.WriteLine(1, "_", data.VariableName, " = ", (data.CompReference == null ? "programData.gameObject;" : JointString("(", data.CompReference.GetType().Name, ")", "exportData.CompReference;")));
                                    classWriter.WriteLine(1, "break;");
                                }
                            }
                            classWriter.WriteLineBraceRight();
                        }
                        classWriter.WriteLineBraceRight();

                        #endregion
                    }
                    classWriter.WriteLineBraceRight();//End of if programData.ExportData
                }
                classWriter.WriteLineBraceRight();//End of for UIProgramData

                #endregion
            }
            classWriter.WriteLineBraceRight();//End of function

            #endregion

            classWriter.WriteLine();

            #region Clear方法区

            classWriter.WriteLine("public void Clear()");
            classWriter.WriteBraceLeft();//Start of clear function
            {
                //置空Root
                classWriter.WriteLine("thisObjRoot = null;");

                //置空变量
                for (int i = 0; i < datas.Count; i++)
                {
                    UIExportData data = datas[i];
                    string variableName = JointString("_", data.VariableName);
                    classWriter.WriteLine(variableName + " = null;");
                }

                //删除这个对象创建的GameObject
                classWriter.WriteLine("DefaultObjForUI.DestroyGameObjectByObjGen(this);");
            }
            classWriter.WriteLineBraceRight();//End of clear function
            #endregion

            classWriter.WriteLineBraceRight();
        }

        /// <summary>
        /// 生成一个变量（变量标识名，变量字段，变量属性）
        /// </summary>
        private void WriteVariable(UIExportData data, string className)
        {
            string type = GetVariableType(data);
            string constStrVaiableName = JointString("_", data.VariableName, "_Name");
            string privateVariableName = JointString("_", data.VariableName);

            classWriter.WriteLine();

            classWriter.WriteLine("#region ", type, " ", data.VariableName);

            //物体的引用变量名
            classWriter.WriteLine("private const string ", constStrVaiableName, " = \"", data.VariableName, "\";");
            //物体的引用
            classWriter.WriteLine("private ", type, " ", privateVariableName, ";");
            //物体的Get属性
            classWriter.WriteLine("public ", type, " ", data.VariableName);
            classWriter.WriteBraceLeft();
            {
                classWriter.WriteLine("get");
                classWriter.WriteBraceLeft();
                {
                    classWriter.WriteLine("if (null == ", privateVariableName, ")");
                    if (data.CompReference == null)
                    {
                        //证明是GameObject
                        classWriter.WriteLine(1, privateVariableName, " = DefaultObjForUI.DefaultGameObject(this, \"ClassName_",
                            className, " VariableName_", data.VariableName, "\");");
                    }
                    else
                    {
                        //证明是组件
                        classWriter.WriteLine(1, privateVariableName, " = DefaultObjForUI.DefaultComponent<", type,
                            ">(this, \"ClassName_", className, " VariableName_", data.VariableName, "\");");
                    }
                    classWriter.WriteLine("return ", privateVariableName, ";");
                }
                classWriter.WriteLineBraceRight();
            }
            classWriter.WriteLineBraceRight();

            classWriter.WriteLine("#endregion");
        }

        /// <summary>
        /// 获取变量的简称类型
        /// </summary>
        private string GetVariableType(UIExportData data)
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

        /// <summary>
        /// 拼接字符串
        /// </summary>
        private string JointString(params string[] strs)
        {
            strCache.Remove(0, strCache.Length);
            for (int i = 0; i < strs.Length; i++)
            {
                strCache.Append(strs[i]);
            }
            return strCache.ToString();
        }

    }
}