using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.QuarkAsset
{
    /// <summary>
    /// QuarkAssetDataset用于在Editor Runtime快速开发时使用；
    /// build之后需配合AB资源使用；
    /// </summary>
    public class QuarkAssetDataset : ScriptableObject
    {
        const string datasetConfigName = "QuarkAssetDataset";
        const string stringPath = "Assets/QuarkAssetDataset.asset";
        static QuarkAssetDataset instance;
        public static QuarkAssetDataset Instance
        {
            get
            {
                if (instance == null)
                {
                    var so= UnityEditor.AssetDatabase.LoadAssetAtPath<QuarkAssetDataset>(stringPath);
                    if(so==null)
                    {
                        so = ScriptableObject.CreateInstance<QuarkAssetDataset>();
                        UnityEditor.AssetDatabase.CreateAsset(so, stringPath);
                        UnityEditor.AssetDatabase.SaveAssets();
                        UnityEditor.AssetDatabase.Refresh();
                        instance= UnityEditor.AssetDatabase.LoadAssetAtPath<QuarkAssetDataset>(stringPath);
                    }
                }
                return instance;
            }
        }
        public QuarkAssetData QuarkAssetData;
        public override string ToString()
        {
            return datasetConfigName+ " ToString";
        }
    }
}