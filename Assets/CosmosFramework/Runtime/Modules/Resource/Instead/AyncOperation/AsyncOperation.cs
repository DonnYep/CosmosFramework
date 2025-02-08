using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Operation;
namespace Cosmos.Resource
{
    public class AsssetOperation<T> : OperationBase
        where T : UnityEngine.Object
    {
        T assetObject;
        public T AssetObject
        {
            get { return assetObject; }
        }
        internal override void InternalOnAbort()
        {
        }
        internal override void InternalOnFinish()
        {
        }
        internal override void InternalOnStart()
        {
        }
        internal override void InternalOnUpdate()
        {
        }
        public T WaitForCompletion()
        {
            var loadMode = ResourceSettings.Instance.ResourceLoadMode;
            switch (loadMode)
            {
                case ResourceLoadMode.Resource:
                    {

                    }
                    break;
                case ResourceLoadMode.AssetBundle:
                    {
                        while (!InvokeWaitForCompletion())
                        {
                        }
                    }
                    break;
                case ResourceLoadMode.AssetDatabase:
                    {

                    }
                    break;
            }
            return assetObject;
        }
        bool InvokeWaitForCompletion()
        {
            if (IsDone)
                return true;
            return IsDone;
        }
    }
}
