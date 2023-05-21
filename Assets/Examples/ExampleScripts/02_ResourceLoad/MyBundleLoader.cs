using UnityEngine;
using Cosmos;
public class MyBundleLoader : MonoBehaviour
{
    string[] prefabNames = new string[]
    {
        "AudioSource","Capsule","Cube","Cylinder","Sphere","YBot_Idle","ResPrefab_Capsule","ResPrefab_Cube","ResPrefab_Sphere"
    };
    void Start()
    {
        for (int i = 0; i < prefabNames.Length; i++)
        {
            CosmosEntry.ResourceManager.LoadPrefabAsync(prefabNames[i], null, null, true);
        }
    }
}
