using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Cosmos.Quark.Loader
{
    public class QuarkAssetDatabaseLoader : IQuarkAssetLoader
    {
        QuarkAssetDataset quarkAssetData;
        /// <summary>
        /// AssetDataBase模式下资源的映射字典；
        /// Key : AssetName---Value :  Lnk [QuarkAssetObject]
        /// </summary>
        Dictionary<string, LinkedList<QuarkAssetDatabaseObject>> assetDatabaseMap
            = new Dictionary<string, LinkedList<QuarkAssetDatabaseObject>>();

        public void Initiate(string url, string localPath)
        {

        }
        public void SetLoaderData(object customeData)
        {
            SetAssetDatabaseModeData(customeData as QuarkAssetDataset);
        }
        public T LoadAsset<T>(string assetName, bool instantiate = false) where T : UnityEngine.Object
        {
            return LoadAsset<T>(assetName, string.Empty, instantiate);
        }
        public T LoadAsset<T>(string assetName, string assetExtension, bool instantiate = false) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(assetName))
                throw new ArgumentNullException("Asset name is invalid!");
            QuarkAssetDatabaseObject quarkAssetDatabaseObject = new QuarkAssetDatabaseObject();
            if (assetDatabaseMap == null)
                throw new Exception("QuarkAsset 未执行 build 操作！");
            if (assetDatabaseMap.TryGetValue(assetName, out var lnk))
                quarkAssetDatabaseObject = GetAssetDatabaseObject<T>(lnk, assetExtension);
            if (!string.IsNullOrEmpty(quarkAssetDatabaseObject.AssetGuid))
            {
                var guid2path = UnityEditor.AssetDatabase.GUIDToAssetPath(quarkAssetDatabaseObject.AssetGuid);
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(guid2path);
                if (instantiate)
                    return GameObject.Instantiate(asset);
                return asset;
            }
            else
                return null;
#else
                return null;
#endif
        }
        public Coroutine LoadAssetAsync<T>(string assetName, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, string.Empty, callback, instantiate));
        }
        public Coroutine LoadAssetAsync<T>(string assetName, string assetExtension, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object
        {
            return QuarkUtility.Unity.StartCoroutine(EnumLoadAsssetAsync(assetName, assetExtension, callback, instantiate));
        }
        public void UnLoadAllAssetBundle(bool unloadAllLoadedObjects = false)
        {
            Utility.Debug.LogInfo("AssetDatabase Mode UnLoadAllAsset");
        }
        public void UnLoadAssetBundle(string assetBundleName, bool unloadAllLoadedObjects = false)
        {
            Utility.Debug.LogInfo("AssetDatabase Mode UnLoadAllAsset");
        }
        IEnumerator EnumLoadAsssetAsync<T>(string assetName, string assetExtension, Action<T> callback, bool instantiate = false) where T : UnityEngine.Object
        {
            var asset = LoadAsset<T>(assetName, assetExtension, instantiate);
            yield return null;
            callback?.Invoke(asset);
        }
        QuarkAssetDatabaseObject GetAssetDatabaseObject<T>(LinkedList<QuarkAssetDatabaseObject> lnk, string assetExtension = null)
where T : UnityEngine.Object
        {
            var assetType = typeof(T).ToString();
            QuarkAssetDatabaseObject quarkAssetObject = new QuarkAssetDatabaseObject();
            var tempObj = lnk.First.Value;
            if (tempObj.AssetType != assetType)
            {
                foreach (var assetObj in lnk)
                {
                    if (assetObj.AssetType == assetType)
                    {
                        if (!string.IsNullOrEmpty(assetExtension))
                        {
                            if (assetObj.AssetExtension == assetExtension)
                            {
                                quarkAssetObject = assetObj;
                                break;
                            }
                        }
                        else
                        {
                            quarkAssetObject = assetObj;
                        }
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(assetExtension))
                {
                    quarkAssetObject = tempObj.AssetExtension == assetExtension == true ? tempObj : QuarkAssetDatabaseObject.None;
                }
                else
                {
                    quarkAssetObject = tempObj;
                }
            }
            return quarkAssetObject;
        }
        /// <summary>
        /// 对QuarkAssetDataset进行编码
        /// </summary>
        /// <param name="assetData">QuarkAssetDataset对象</param>
        void SetAssetDatabaseModeData(QuarkAssetDataset assetData)
        {
            var lnkDict = new Dictionary<string, LinkedList<QuarkAssetDatabaseObject>>();
            var length = assetData.QuarkAssetObjectList.Count;
            for (int i = 0; i < length; i++)
            {
                var name = assetData.QuarkAssetObjectList[i].AssetName;
                if (!lnkDict.TryGetValue(name, out var lnkList))
                {
                    var lnk = new LinkedList<QuarkAssetDatabaseObject>();
                    lnk.AddLast(assetData.QuarkAssetObjectList[i]);
                    lnkDict.Add(name, lnk);
                }
                else
                {
                    lnkList.AddLast(assetData.QuarkAssetObjectList[i]);
                }
            }
            quarkAssetData = assetData;
            assetDatabaseMap = lnkDict;
        }
    }
}
