using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    /// <summary>
    /// 自动构造接口；
    /// 凡是继承自单例（Singleton）的对象都会在创建完成之后，有单例对象自动调用
    /// </summary>
    public interface IConstruction
    {
        /// <summary>
        /// 对象被构造完成后调用的函数
        /// </summary>
        void OnConstruction();
    }
}
