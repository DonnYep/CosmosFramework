using UnityEditor;

namespace Cosmos.Editor.Resource
{
    public class ResourceAnalyzerWindow: ModuleWindowBase
    {
        readonly string windowDataName = "ResourceAnalyzerWindowData.json";
        ResourceAnalyzerWindowData windowData;
        [MenuItem("Window/Cosmos/Module/Resource/ResourceAnalyzer")]
        public static void OpenWindow()
        {
            var window = GetWindow<ResourceAnalyzerWindow>("ResourceAnalyzer", true);
            window.minSize = EditorUtil.DevWinSize;
        }
        protected override void GetWindowData()
        {
            try
            {
                windowData = EditorUtil.GetData<ResourceAnalyzerWindowData>(ResourceEditorConstants.CACHE_RELATIVE_PATH, windowDataName);
            }
            catch
            {
                windowData = new ResourceAnalyzerWindowData();
                EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, windowDataName, windowData);
            }
        }
        protected override void SaveWindowData()
        {
            EditorUtil.SaveData(ResourceEditorConstants.CACHE_RELATIVE_PATH, windowDataName, windowData);
        }
    }
}
