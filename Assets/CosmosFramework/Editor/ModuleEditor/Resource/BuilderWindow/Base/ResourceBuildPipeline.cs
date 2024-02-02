using Cosmos.Resource;
using UnityEditor;

namespace Cosmos.Editor.Resource
{
    public class ResourceBuildPipeline
    {
        static ResourceDataset defaultResourceDataset;
        static AssetBundleBuildProfile defaultAssetBundleBuildProfile;
        /// <summary>
        /// 默认ResourceDataset
        /// <para><see cref="ResourceEditorConstants.DEFAULT_DATASET_PATH"/></para>
        /// </summary>
        public static ResourceDataset DefaultResourceDataset
        {
            get
            {
                if (defaultResourceDataset == null)
                    defaultResourceDataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>(ResourceEditorConstants.DEFAULT_DATASET_PATH);
                return defaultResourceDataset;
            }
        }
        /// <summary>
        /// 默认AssetBundleBuildProfile
        /// <para><see cref="ResourceEditorConstants.DEFAULT_BUILD_PROFILE_PATH"/></para>
        /// </summary>
        public static AssetBundleBuildProfile DefaultAssetBundleBuildProfile
        {
            get
            {
                if (defaultAssetBundleBuildProfile == null)
                    defaultAssetBundleBuildProfile = AssetDatabase.LoadAssetAtPath<AssetBundleBuildProfile>(ResourceEditorConstants.DEFAULT_BUILD_PROFILE_PATH);
                return defaultAssetBundleBuildProfile;
            }
        }
        /// <summary>
        /// 通过默认预设构建资源
        /// <para>默认ResourceDatase地址：<see cref="ResourceEditorConstants.DEFAULT_DATASET_PATH"/></para>
        /// <para>默认AssetBundleBuildProfile地址：<see cref="ResourceEditorConstants.DEFAULT_BUILD_PROFILE_PATH"/></para>
        /// </summary>
        [MenuItem("Window/Cosmos/Build/Resource/BuildAssetBundleByDefaultProfile")]
        public static void BuildAssetBundleByDefaultProfile()
        {
            if (DefaultResourceDataset == null)
            {
                EditorUtil.Debug.LogError($"ResourceDataset : {ResourceEditorConstants.DEFAULT_DATASET_PATH} not exist !");
                return;
            }
            if (DefaultAssetBundleBuildProfile == null)
            {
                EditorUtil.Debug.LogError($"AssetBundleBuildProfile : {ResourceEditorConstants.DEFAULT_BUILD_PROFILE_PATH} not exist !");
                return;
            }
            var buildParams = DefaultAssetBundleBuildProfile.GetBuildParams();
            ResourceBuildController.BuildAssetBundle(defaultResourceDataset, buildParams);
        }
        /// <summary>
        /// 通过预设构建资源
        /// <para>ResourceDatase参考地址：<see cref="ResourceEditorConstants.DEFAULT_DATASET_PATH"/></para>
        /// <para>AssetBundleBuildProfile参考地址：<see cref="ResourceEditorConstants.DEFAULT_BUILD_PROFILE_PATH"/></para>
        /// </summary>
        /// <param name="datasetPath">ResourceDataset预设的地址</param>
        /// <param name="buildProfilePath">AssetBundleBuildProfile预设的地址</param>
        public static void BuildAssetBundleByProfile(string datasetPath, string buildProfilePath)
        {
            var dataset = AssetDatabase.LoadAssetAtPath<ResourceDataset>(datasetPath);
            if (dataset == null)
            {
                EditorUtil.Debug.LogError($"ResourceDataset : {datasetPath} not exist !");
                return;
            }
            var buildProfile = AssetDatabase.LoadAssetAtPath<AssetBundleBuildProfile>(buildProfilePath);
            if (buildProfile == null)
            {
                EditorUtil.Debug.LogError($"AssetBundleBuildProfile : {buildProfilePath} not exist !");
                return;
            }
            var buildParams = buildProfile.GetBuildParams();
            ResourceBuildController.BuildAssetBundle(dataset, buildParams);
        }
    }
}