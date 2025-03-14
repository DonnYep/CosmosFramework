using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Cosmos.Resource
{
    public abstract class InitializationOperation : OperationBase
    {
        public string Version { get; protected set; }
    }
    /// <summary>
    /// 编辑器加载资源寻址信息
    /// </summary>
    internal class EditorLoadAssetOperation : InitializationOperation
    {
        internal override void InternalOnStart()
        {

        }
        internal override void InternalOnUpdate()
        {

        }
    }
}
