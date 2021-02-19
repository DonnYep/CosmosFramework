using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    /// <summary>
    /// 泛型数据传输容器
    /// 封闭继承
    /// </summary>
    public sealed class LogicEventArgs<T> : GameEventArgs
    {
        /// <summary>
        /// 泛型构造
        /// </summary>
        public LogicEventArgs(T data)
        {
            SetData(data);
        }
        public LogicEventArgs() { }
        /// <summary>
        /// 泛型数据类型
        /// </summary>
        public T Data { get; private set; }
        public LogicEventArgs<T> SetData(T data)
        {
            Data = data;
            return this;
        }
        public override void Clear()
        {
            Data = default(T);
        }
    }
}