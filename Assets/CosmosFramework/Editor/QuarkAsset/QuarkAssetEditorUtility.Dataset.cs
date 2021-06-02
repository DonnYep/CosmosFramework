using Cosmos.Quark;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
namespace Cosmos.CosmosEditor
{
    public static partial class QuarkAssetEditorUtility
    {
        public static class Dataset
        {
            const string folderPath = "Assets/QuarkAsset";
            const string datasetPath = "Assets/QuarkAsset/QuarkAssetDataset.asset";
            static QuarkAssetDataset quarkAssetDataset;
            public static QuarkAssetDataset QuarkAssetDatasetInstance
            {
                get
                {
                    if (quarkAssetDataset == null)
                    {
                        var so = AssetDatabase.LoadAssetAtPath<QuarkAssetDataset>(datasetPath);
                        if (so == null)
                        {
                            if (!AssetDatabase.IsValidFolder(folderPath))
                            {
                                AssetDatabase.CreateFolder("Assets", "QuarkAsset");
                            }
                            so = ScriptableObject.CreateInstance<QuarkAssetDataset>();
                            so.hideFlags = HideFlags.NotEditable;
                            AssetDatabase.CreateAsset(so, datasetPath);
                            EditorUtility.SetDirty(so);
                            AssetDatabase.SaveAssets();
                            AssetDatabase.Refresh();
                        }
                        quarkAssetDataset = so;
                    }
                    return quarkAssetDataset;
                }
            }
        }
    }
}
