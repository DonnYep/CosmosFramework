using UnityEngine;
namespace Cosmos.Entity
{
    /// <summary>
    /// 实体对象；
    /// </summary>
    public abstract class EntityObject : MonoBehaviour
    {
        Entity entity;
        public virtual string EntityObjectName
        {
            get { return gameObject.name; }
            set { gameObject.name = value; }
        }
        public Entity Entity { get { return entity; } }
        public abstract void OnInit();
    }
}
