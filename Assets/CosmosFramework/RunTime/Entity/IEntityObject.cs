using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cosmos;
using UnityEngine;
namespace Cosmos.Entity
{
    public interface IEntityObject: IReference, IRefreshable, IOperable,IControllable
    {
        void SetEntity(GameObject entity);
        GameObject GetEntity();
        void OnAttach(Transform parent);
        void OnAttachChild(Transform child);
        void OnDetach();
        void OnDetachChild(Transform child);
    }
}
