using UnityEditor;

namespace Cosmos.Editor
{
    public class AssetBundleCommand
    {
        [MenuItem("Window/Cosmos/Command/ForceRemoveAllAssetBundleNames")]
        public static void ForceRemoveAllAssetBundleNames()
        {
            var run= EditorUtility.DisplayDialog("AssetBundleCommand", "This operation will force remove all assetBundle names , whether to continue ?", "Ok","Cancel");
            if (!run)
                return;
            var allBundleNames = AssetDatabase.GetAllAssetBundleNames();
            foreach (var bundleName in allBundleNames)
            {
                AssetDatabase.RemoveAssetBundleName(bundleName, true);
            }
            AssetDatabase.Refresh();
            EditorUtil.Debug.LogInfo("Force remove all assetBundle names done");
        }
    }
}
