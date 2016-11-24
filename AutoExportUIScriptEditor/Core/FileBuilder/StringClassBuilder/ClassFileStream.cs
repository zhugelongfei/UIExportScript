using System.IO;
using System.Text;

namespace AutoExportScriptData
{
    internal class ClassFileStream
    {
        private const string BraceLeft = "{";
        private const string BraceRight = "}";
        private const string Tab = "    ";

        private StringBuilder strBuilder = new StringBuilder(100);
        private string strTab = "";
        private StreamWriter writer = null;

        //tab键的数量，每写入一个左大括号，就会增加一次，每写入一个右大括号，就会减少一次
        private int tCount = 0;
        private int tabCount
        {
            get { return tCount; }
            set
            {
                tCount = System.Math.Max(0, value);
                RebuildTabStr();
            }
        }

        public ClassFileStream(StreamWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// 写入一行空行
        /// </summary>
        public void WriteLine()
        {
            writer.WriteLine();
        }

        /// <summary>
        /// 写入一行文本
        /// </summary>
        /// <param name="tabAmendment">修正Tab长度（精简if 和 case 使用）</param>
        public void WriteLine(int tabAmendment, params string[] values)
        {
            if (tabAmendment != 0)
            {
                for (int i = 0; i < tabAmendment; i++)
                {
                    strBuilder.Append(Tab);
                }
            }
            WriteLine(values);
        }

        /// <summary>
        /// 写入一行文本
        /// </summary>
        public void WriteLine(params string[] values)
        {
            //加入行Tab长度
            strBuilder.Append(strTab);

            //追加输入字符串
            foreach (string str in values)
            {
                strBuilder.Append(str);
            }
            writer.WriteLine(strBuilder.ToString());

            //清空字符串
            ClearStringBuilder();
        }

        /// <summary>
        /// 写入左大括号
        /// </summary>
        public void WriteBraceLeft()
        {
            WriteLine(BraceLeft);
            tabCount++;
        }

        /// <summary>
        /// 写入右大括号
        /// </summary>
        public void WriteLineBraceRight()
        {
            tabCount--;
            WriteLine(BraceRight);
        }

        /// <summary>
        /// 重新计算并设置tab的长度
        /// </summary>
        private void RebuildTabStr()
        {
            for (int i = 0; i < tabCount; i++)
            {
                strBuilder.Append(Tab);
            }
            strTab = strBuilder.ToString();
            ClearStringBuilder();
        }

        /// <summary>
        /// 清空StringBuilder
        /// </summary>
        private void ClearStringBuilder()
        {
            strBuilder.Remove(0, strBuilder.Length);
        }

        public void Flush()
        {
            writer.Flush();
        }

        public void Close()
        {
            writer.Close();
            writer.Dispose();
            writer = null;
        }
    }
}