using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System.Text;
namespace Cosmos
{
    
    public sealed partial class Utility
    {
        /// <summary>
        /// 这是一个临时UI工具类，正式项目不建议使用。
        /// </summary>
        public static class UI
        {
            /// <summary>
            /// 得到基于Resources下的相对完整路径
            /// </summary>
            /// <param name="panelName"></param>
            /// <returns></returns>
            public static string GetUIFullRelativePath(string panelName)
            {
              return  Utility.Text.Format("UI/" + panelName);
            }
        }
    }
}