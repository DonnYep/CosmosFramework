using System;
namespace Cosmos.Editor.Hotfix
{
    [Serializable]
    public class HotfixWindowData
    {
        /// <summary>
        /// 编译完成的程序集路径；
        /// </summary>
        public string CompiledAssemblyPath { get; set; }
        /// <summary>
        /// 拷贝到工程的相对路径；
        /// </summary>
        public string AssetsRelativePath{ get; set; }
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
        public bool AutoLoadHotfixCode { get; set; }
        public HotfixWindowData()
        {
            AutoLoadHotfixCode = false;
            AppendExtension = true;
            Extension = ".bytes";
        }
        public void Reset()
        {
            CompiledAssemblyPath = string.Empty;
            AssetsRelativePath = string.Empty;
            AutoLoadHotfixCode = false;
            AppendExtension = true;
            Extension = ".bytes";
        }
    }
}
