using System;
using System.Reflection;
using System.Runtime.InteropServices; 
using System.Text;

namespace AutoExportScriptData
{
    /// <summary>
    /// INI文件的操作类
    /// </summary>
    internal class IniFile
    {
        private const int StrCount = 1024;
        private string Path;
        private StringBuilder temp = new StringBuilder(StrCount);

        public IniFile(string path)
        {
            this.Path = path;
        }

        public void WriteObject(string section, object data)
        {
            if (data == null)
                return;

            Type type = data.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(data);
                IniWriteValue(section, field.Name, value == null ? null : value.ToString());
            }
        }

        public T ReadObject<T>(string section)
            where T : new()
        {
            T data = new T();
            return ReadObject(section, data);
        }

        public T ReadObject<T>(string section, T data)
        {
            Type type = typeof(T);
            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo field in fields)
            {
                string value = IniReadValue(section, field.Name);
                if (string.IsNullOrEmpty(value))
                    continue;
                field.SetValue(data, System.Convert.ChangeType(IniReadValue(section, field.Name), field.FieldType));
            }
            return data;
        }

        //声明读写INI文件的API函数
        #region 声明读写INI文件的API函数 
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, Byte[] retVal, int size, string filePath);
        #endregion

        /// <summary>
        /// 写INI文件
        /// </summary>
        /// <param name="section">段落</param>
        /// <param name="key">键</param>
        /// <param name="iValue">值</param>
        public void IniWriteValue(string section, string key, string iValue)
        {
            WritePrivateProfileString(section, key, iValue, this.Path);
        }

        /// <summary>
        /// 读取INI文件（文件必须是ANSI格式）
        /// </summary>
        /// <param name="section">段落</param>
        /// <param name="key">键</param>
        /// <returns>返回的键值</returns>
        public string IniReadValue(string section, string key)
        {
            temp.Remove(0, temp.Length);

            int i = GetPrivateProfileString(section, key, null, temp, StrCount, this.Path);
            return temp.ToString();
        }

        /// <summary>
        /// 读取INI文件（文件必须是ANSI格式）
        /// </summary>
        /// <param name="Section">段，格式[]</param>
        /// <param name="Key">键</param>
        /// <returns>返回byte类型的section组或键值组</returns>
        public byte[] IniReadValues(string section, string key)
        {
            byte[] temp = new byte[StrCount];

            int i = GetPrivateProfileString(section, key, "", temp, StrCount, this.Path);
            return temp;
        }
    }
}