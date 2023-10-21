using UnityEditor;

namespace Cosmos.Editor.Resource
{
    public class ResourceAnalyzerWindow : ModuleWindowBase
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
            windowData = EditorUtil.SafeGetData<ResourceAnalyzerWindowData>(ResourceEditorConstants.EDITOR_CACHE_RELATIVE_PATH, windowDataName);
        }
        protected override void SaveWindowData()
        {
            EditorUtil.SaveData(ResourceEditorConstants.EDITOR_CACHE_RELATIVE_PATH, windowDataName, windowData);
        }
    }
}
