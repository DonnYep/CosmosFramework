using System;
namespace Cosmos.Editor.Hotfix
{
    [Serializable]
    public class HotUpdateWindowData
    {
        /// <summary>
        /// 编译完成的程序集路径；
        /// </summary>
        public string CompiledDllPath { get; set; }
        /// <summary>
        /// 拷贝到工程的相对路径；
        /// </summary>
        public string DllPastePath { get; set; }
        /// <summary>
        /// 绝对路径；
        /// </summary>
        public string FullDllPastePath { get; set; }
        /// <summary>
        /// 追加后缀名；
        /// </summary>
        public bool AppendExtension { get; set; }
        /// <summary>
        /// 追加的后缀名；
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 开启自动加载；
        /// </summary>
        public bool AutoLoadHotUpdateCode { get; set; }
        public HotUpdateWindowData()
        {
            AutoLoadHotUpdateCode = false;
            AppendExtension = true;
            Extension = ".bytes";
        }
        public void Reset()
        {
            CompiledDllPath = string.Empty;
            DllPastePath= string.Empty;
            AutoLoadHotUpdateCode = false;
            AppendExtension = true;
            Extension = ".bytes";
        }
    }
}
