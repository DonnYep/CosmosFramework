using UnityEditor;
namespace Cosmos.Editor
{
    public class ModuleWindowBase : EditorWindow
    {
        protected virtual void OnEnable()
        {
            GetWindowData();
        }
        protected virtual void OnGUI()
        {
        }
        protected virtual void OnDisable()
        {
            SaveWindowData();
        }
        protected virtual void GetWindowData()
        {

        }
        protected virtual void SaveWindowData()
        {

        }
    }
}
