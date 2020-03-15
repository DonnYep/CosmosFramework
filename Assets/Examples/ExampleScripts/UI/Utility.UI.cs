using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System.Text;
namespace Cosmos
{
    /// <summary>
    /// 这是一个临时工具类
    /// </summary>
    public sealed partial class Utility
    {
        public sealed class UI
        {
            static StringBuilder stringBuilderCache;
            static StringBuilder StringBuilderCache
            {
                get
                {
                    if (stringBuilderCache == null)
                        stringBuilderCache = new StringBuilder(1024);
                    return stringBuilderCache;
                }
                set { stringBuilderCache = value; }
            }
            /// <summary>
            /// 得到基于Resources下的相对完整路径
            /// </summary>
            /// <param name="panelName"></param>
            /// <returns></returns>
            public static string GetUIFullRelativePath(string panelName)
            {
                StringBuilderCache.Clear();
                StringBuilderCache.Append("UI/" + panelName);
                return StringBuilderCache.ToString();
            }
        }
    }
}