using System;
using System.IO;

namespace Cosmos.Editor.Hotfix
{
    [Serializable]
    public class HotUpdateWindowData
    {
        /// <summary>
        /// 编译完成的程序集路径；
        /// </summary>
        public string AssembliesPath { get; set; }
        /// <summary>
        /// 拷贝到工程的相对路径；
        /// </summary>
        public string AssembliesPastePath { get; set; }
        /// <summary>
        /// 绝对路径；
        /// </summary>
        public string FullAssembliesPastePath { get; set; }
        /// <summary>
        /// 追加后缀名；
        /// </summary>
        public bool AppendExtension { get; set; }
        /// <summary>
        /// 追加的后缀名；
        /// </summary>
        public string Extension { get; set; }
        public HotUpdateWindowData()
        {
            AssembliesPath = Utility.IO.WebPathCombine(Directory.GetCurrentDirectory());
            AppendExtension = true;
            Extension = HotUpdateWindowConstants.AppendExtension;
            AssembliesPastePath = "HotUpdate";
        }
        public void Reset()
        {
            AssembliesPath = Utility.IO.WebPathCombine(Directory.GetCurrentDirectory());
            AssembliesPastePath = "HotUpdate";
            AppendExtension = true;
            Extension = HotUpdateWindowConstants.AppendExtension;
        }
    }
}
