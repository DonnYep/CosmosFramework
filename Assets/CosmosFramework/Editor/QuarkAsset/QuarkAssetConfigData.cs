using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.CosmosEditor
{
    public class QuarkAssetConfigData
    {
        /// <summary>
        /// 是否生成路径地址代码；
        /// </summary>
        public bool GenerateAssetPathCode;
        /// <summary>
        /// 包含的路径；
        /// </summary>
        public List<string> IncludeDirectories;
    }
}
