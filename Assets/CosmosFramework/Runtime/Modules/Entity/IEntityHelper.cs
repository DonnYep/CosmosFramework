using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos.Entity;
namespace Cosmos
{
    public interface IEntityHelper
    {
        /// <summary>
        /// 实体对象的根节点；
        /// </summary>
        object EntityRoot { get; }
        /// <summary>
        /// 实例化实体；
        /// </summary>
        /// <param name="entityAsset">实体资源</param>
        /// <returns>实体对象</returns>
        object  SpanwEntityInstance(object entityAsset); 
        void DespawnEntityInstance(object  entityInstance);
        void Attach(IEntity childEntity,IEntity parent);
        void Deatch(IEntity childEntity,IEntity parent);
    }
}
