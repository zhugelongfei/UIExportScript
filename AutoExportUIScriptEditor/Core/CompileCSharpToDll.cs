using System.Diagnostics;
using System.Text;

namespace AutoExportScriptData
{
    internal class CompileCSharpDll
    {
        public delegate void Logger(string log);
        public static bool isFinished = false;
        private Logger log;

        public CompileCSharpDll(Logger log)
        {
            this.log = log;
        }

        public class CompileFileInfo
        {
            //编译工具路径
            public string utilFilePath;

            //编译的文件路径
            public string compileFilePath;

            //在编译文件路径下，寻找脚本的通配符
            public string searchPattern;

            //引用的DLL路径
            public string[] referenceDllFilePath;

            //DLL输出路径
            public string dllOutPath;
        }

        public void Compile(CompileFileInfo compInfo)
        {
            if (compInfo == null)
            {
                UnityEngine.Debug.LogError("Error: compInfo is null");
                return;
            }
            StringBuilder strCache = new StringBuilder(300);
            //编译类型
            strCache.Append("/t:library ");
            //DLL输出路径
            strCache.Append("/out:");
            strCache.Append(compInfo.dllOutPath);
            strCache.Append(" ");
            //依赖DLL
            if (compInfo.referenceDllFilePath != null && compInfo.referenceDllFilePath.Length > 0)
            {
                foreach (string path in compInfo.referenceDllFilePath)
                {
                    strCache.Append(@"/reference:");
                    strCache.Append(path);
                    strCache.Append(" ");
                }
            }
            //编译文件路径
            strCache.Append(compInfo.compileFilePath + compInfo.searchPattern);

            Process p = new Process();
            p.StartInfo.FileName = compInfo.utilFilePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = false;

            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.StandardOutputEncoding = Encoding.Default;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += ReceiveOutPut;

            if (log != null)
                log(strCache.ToString());

            p.StartInfo.Arguments = strCache.ToString();
            p.EnableRaisingEvents = true;
            p.Exited += CSCExit;

            if (!p.Start())
            {
                if (log != null)
                    log("CSC 启动失败");
            }

            p.BeginOutputReadLine();

        }

        private void ReceiveOutPut(object sender, DataReceivedEventArgs e)
        {
            //刷新缓冲区，防止缓冲区满了，导致程序等待数据读取，而Process等待程序退出的 死锁问题
            Process p = (Process)sender;
            p.CancelOutputRead();
            if (log != null && !string.IsNullOrEmpty(e.Data))
                log("CSC: " + e.Data);
            p.BeginOutputReadLine();
        }

        private void CSCExit(object sender, System.EventArgs e)
        {
            Process p = (Process)sender;

            string outputStr = p.StandardError.ReadToEnd();

            //等待异步输出关闭
            p.WaitForExit();

            //输出此次结果
            if (log != null)
            {
                if (p.ExitCode != 0)
                {
                    log("编译失败");
                }
                else
                {
                    log("编译完成");
                }
                if (!string.IsNullOrEmpty(outputStr))
                    log("Error : " + outputStr);
            }

            //对外标识结束
            isFinished = true;

            //必须注销函数指针，才能析构此对象
            //不明白为什么Process对象被释放后，还不能释放函数指针
            //难道Process对象根本就没有释放？
            p.OutputDataReceived -= ReceiveOutPut;
            p.Exited -= CSCExit;

            //释放资源
            p.Close();
            p.Dispose();
            log = null;
        }
    }
}