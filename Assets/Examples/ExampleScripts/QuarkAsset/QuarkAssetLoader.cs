using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cosmos.QuarkAsset;
namespace Cosmos.Test
{
    public class QuarkAssetLoader : MonoBehaviour
    {
        void Start()
        {
            var go = QuarkUtility.Instantiate<GameObject>("YBot_LM_Local");
            if (go == null)
                Debug.LogError("go null");
            Debug.Log(QuarkAssetDataset.Instance.ToString());
        }
    }
}