using UnityEditor;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public abstract class ResourceBuilderWindowTabBase
    {
        public EditorWindow ParentWindow { get; protected set; }
        public ResourceBuilderWindowTabBase(EditorWindow parentWindow)
        {
            this.ParentWindow = parentWindow;
        }
        public abstract void OnEnable();
        public abstract void OnDisable();
        public abstract void OnGUI(Rect rect);
        public virtual void OnDatasetAssign() { }
        public virtual void OnDatasetRefresh() { }
        public virtual void OnDatasetUnassign() { }
    }
}
