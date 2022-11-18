using System;
using UnityEngine;
namespace Cosmos.Entity
{
    /// <summary>
    /// 实体对象；
    /// </summary>
    public abstract class EntityObject : MonoBehaviour, IEquatable<EntityObject>
    {
        string entityName;
        public string EntityName
        {
            get { return entityName; }
            set
            {
                gameObject.name = value;
                entityName = value;
            }
        }
        public string EntityGroupName { get; private set; }
        public GameObject Handle { get { return gameObject; } }
        public int EntityObjectId { get; private set; }
        public virtual void OnInit(string entityName, int entityObjectId, string entityGroupName)
        {
            EntityName = entityName;
            EntityGroupName = entityGroupName;
            EntityObjectId = entityObjectId;
        }
        public virtual void OnShow()
        {
            gameObject.SetActive(true);
        }
        public virtual void OnHide()
        {
            gameObject.SetActive(false);
        }
        public virtual void OnRecycle() { }
        public bool Equals(EntityObject other)
        {
            return other.EntityName == this.EntityName &&
                other.EntityObjectId == this.EntityObjectId;
        }
    }
}
