using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public static partial class Utility
    {
        public static class Process
        {
            /// <summary>
            /// 通过cmd启动一个.netcore应用；
            /// </summary>
            /// <param name="workingDirectory">工作的文件夹地址，dll存放的文件夹路径</param>
            /// <param name="dllName">启动的dll名</param>
            public static void StartNetCoreProcess(string workingDirectory, string dllName)
            {
                var process = new ProcessStartInfo();
                process.UseShellExecute = true;
                process.WorkingDirectory = workingDirectory;
                process.Arguments = dllName;
                process.FileName = "dotnet.exe";
                System.Diagnostics.Process.Start(process);
            }
            /// <summary>
            /// 启动一个进程；
            /// </summary>
            /// <param name="filePath">文件地址</param>
            public static void StartProcess(string filePath)
            {
                System.Diagnostics.Process.Start(filePath);
            }
        }
    }
}
