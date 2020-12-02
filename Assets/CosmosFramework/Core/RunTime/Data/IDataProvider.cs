using System;
using System.Collections.Generic;
using System.Text;

namespace Cosmos.Data
{
    /// <summary>
    /// 允许多派生；
    /// 只要继承自此类，并挂载[ImplementProviderAttribute]特性，皆可在初始化时加载数据；
    /// </summary>
    public interface IDataProvider
    {
      void LoadData();
    }
}
