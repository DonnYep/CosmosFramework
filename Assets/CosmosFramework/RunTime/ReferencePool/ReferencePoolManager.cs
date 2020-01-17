using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.Reference
{
    public class ReferencePoolManager : Module<ReferencePoolManager>
    {
        /// <summary>
        /// 单个引用池上线
        /// </summary>
        public const short _ReferencePoolCapcity= 100;
        Dictionary<object, ReferenceSpawnPool> referencePool = new Dictionary<object, ReferenceSpawnPool>();
        protected override void InitModule()
        {
            RegisterModule(CFModule.Reference);
        }
    }
}