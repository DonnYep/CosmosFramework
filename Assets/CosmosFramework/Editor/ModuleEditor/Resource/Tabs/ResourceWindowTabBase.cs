using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public abstract class ResourceWindowTabBase
    {
        public abstract void OnEnable();
        public abstract void OnDisable();
        public abstract void OnGUI(Rect rect);
        public virtual void OnDatasetAssign() { }
        public virtual void OnDatasetRefresh() { }
        public virtual void OnDatasetUnassign() { }
    }
}
