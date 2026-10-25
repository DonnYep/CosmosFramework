using System;
using UnityEngine;
namespace Cosmos.Entity
{
    /// <summary>
    /// 持有 GameObject 的实体实例基类；
    /// <para>由 EntityManager 统一生成 / 显示 / 隐藏 / 回收，刷新也由管理器集中派发：</para>
    /// <para>需要逐帧刷新的实体实现 <see cref="IEntityTickable"/>（固定帧用
    /// <see cref="IEntityFixedTickable"/>，帧末用 <see cref="IEntityLateTickable"/>）；</para>
    /// <para>不要用 MonoBehaviour.Update 做同一件事，否则会绕过实体暂停与统一时序。</para>
    /// </summary>
    public abstract class EntityObject : MonoBehaviour, IEquatable<EntityObject>, IEntityObject
    {
        EntityPauseState pauseState;
        string entityName;
        /// <summary>
        /// 实体名；赋值时同步修改 GameObject 的名称。
        /// </summary>
        public string EntityName
        {
            get { return entityName; }
            set
            {
                gameObject.name = value;
                entityName = value;
            }
        }
        /// <summary>
        /// 实体组名。
        /// </summary>
        public string EntityGroupName { get; private set; }
        /// <summary>
        /// 实体持有的 GameObject。
        /// </summary>
        public GameObject Handle { get { return gameObject; } }
        /// <summary>
        /// 实体实例编号；在同一实体（同名注册）内部唯一。
        /// </summary>
        public int EntityObjectId { get; private set; }
        /// <summary>
        /// 是否暂停刷新；组冻结会叠加到个体状态上。
        /// </summary>
        public bool Paused
        {
            get { return pauseState.Paused; }
            set { pauseState.Paused = value; }
        }
        /// <inheritdoc/>
        public void SetGroupPaused(bool paused)
        {
            pauseState.SetGroupPaused(paused);
        }
        /// <summary>
        /// 实体初始化；仅在生成新实例时调用一次。
        /// </summary>
        public virtual void OnInit(string entityName, int entityObjectId, string entityGroupName)
        {
            EntityName = entityName;
            EntityGroupName = entityGroupName;
            EntityObjectId = entityObjectId;
        }
        /// <summary>
        /// 实体显示。
        /// </summary>
        public virtual void OnShow()
        {
            gameObject.SetActive(true);
        }
        /// <summary>
        /// 实体隐藏。
        /// </summary>
        public virtual void OnHide()
        {
            gameObject.SetActive(false);
        }
        /// <summary>
        /// 实体回收（进入池）。
        /// </summary>
        public virtual void OnRecycle()
        {
            pauseState.Reset();
        }
        /// <inheritdoc/>
        public bool Equals(EntityObject other)
        {
            if (other == null)
                return false;
            return other.EntityName == this.EntityName &&
                other.EntityObjectId == this.EntityObjectId;
        }
    }
}
