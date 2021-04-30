using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public abstract class ActorBase
    {
        /// <summary>
        /// 被持有的Actor的Type类型
        /// </summary>
        public abstract Type OwnerType { get; }
        /// <summary>
        /// 当前Actor是否被激活
        /// </summary>
        public abstract bool IsActivated { get; }
        /// <summary>
        /// Actor枚举约束的类型;
        /// </summary>
        public abstract byte ActorType { get;protected set; }
        /// <summary>
        ///系统分配的ID
        /// </summary>
        public abstract int ActorID { get; protected set; }
    }
}
