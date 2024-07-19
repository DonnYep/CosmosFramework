using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cosmos.Resource
{
    public class CosmosResouces
    {
        private static bool isInitialize = false;

        private static readonly List<ResourcePackage> resourcePackageList = new List<ResourcePackage>();
        /// <summary>
        /// 是否初始化
        /// </summary>
        public static bool Initialized
        {
            get { return isInitialize; }
        }
    }
}
