using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos
{
    public interface IPreparatory
    {
        /// <summary>
        /// 模块准备工作，在OnInitialization()函数之后执行
        /// Start方法中被调用
        /// </summary>
        void OnPreparatory();
    }
}
