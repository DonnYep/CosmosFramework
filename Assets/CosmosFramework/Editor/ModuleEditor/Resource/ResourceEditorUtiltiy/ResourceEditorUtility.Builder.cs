using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Cosmos.Editor.Resource
{
    public partial class ResourceEditorUtility
    {
        public static class Builder
        {
            public static MultiColumnHeaderState CreateResourceBundleMultiColumnHeader()
            {
                var columns = new[]
                {
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("Size"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=64,
                        width=72,
                        maxWidth=128,
                        autoResize = true,
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("ObjectCount"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=36,
                        width=86,
                        maxWidth=108,
                        autoResize = true,
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("Splittable"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=48,
                        width=64,
                        maxWidth=80,
                        autoResize = false,
                        canSort=true
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("Separately"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=48,
                        width=64,
                        maxWidth=80,
                        autoResize = false,
                        canSort=true
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("Bundle"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=192,
                        width = 384,
                        maxWidth=1024,
                        autoResize = false,
                        canSort=true
                    }
               };
                var state = new MultiColumnHeaderState(columns);
                return state;
            }
            public static MultiColumnHeaderState CreateResourceObjectMultiColumnHeader()
            {
                var columns = new[]
                {
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent(GetFilterByTypeIcon()),
                        headerTextAlignment = TextAlignment.Center,
                        sortingArrowAlignment = TextAlignment.Center,
                        sortedAscending = false,
                        minWidth=28,
                        width=28,
                        maxWidth=28,
                        autoResize = true,
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("Name"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=128,
                        width = 160,
                        maxWidth=512,
                        autoResize = true,
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("Extension"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=64,
                        width=72,
                        maxWidth=128,
                        autoResize = true,
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("Valid"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=40,
                        width=40,
                        maxWidth=40,
                        autoResize = true,
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("Size"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=64,
                        width=72,
                        maxWidth=128,
                        autoResize = true,
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("AssetBundle"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=128,
                        width=256,
                        maxWidth=512,
                        autoResize = true,
                    },
                    new MultiColumnHeaderState.Column
                    {
                        headerContent = new GUIContent("AssetPath"),
                        headerTextAlignment = TextAlignment.Left,
                        sortingArrowAlignment = TextAlignment.Left,
                        sortedAscending = false,
                        minWidth=256,
                        width=1024,
                        maxWidth=1536,
                        autoResize = true,
                    }
                };
                var state = new MultiColumnHeaderState(columns);
                return state;
            }
            public static BuildAssetBundleOptions GetBuildAssetBundleOptions(AssetBundleCompressType compressType, bool disableWriteTypeTree, bool deterministicAssetBundle, bool forceRebuildAssetBundle, bool ignoreTypeTreeChanges)
            {
                BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
                switch (compressType)
                {
                    case AssetBundleCompressType.Uncompressed:
                        options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                        break;
                    case AssetBundleCompressType.StandardCompression_LZMA:
                        //None=StandardCompression_LZMA
                        break;
                    case AssetBundleCompressType.ChunkBasedCompression_LZ4:
                        options |= BuildAssetBundleOptions.ChunkBasedCompression;
                        break;
                }
                if (disableWriteTypeTree)
                    options |= BuildAssetBundleOptions.DisableWriteTypeTree;
                if (deterministicAssetBundle)
                    options |= BuildAssetBundleOptions.DeterministicAssetBundle;
                if (forceRebuildAssetBundle)
                    options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;
                if (ignoreTypeTreeChanges)
                    options |= BuildAssetBundleOptions.IgnoreTypeTreeChanges;
                return options;
            }
        }
    }
}
