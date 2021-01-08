using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
namespace Cosmos.Test {
    public class UIEntry : MonoBehaviour
    {
        private void Start()
        {
            CosmosEntry.UIManager.SetHelper(new DefaultUIFormHelper());
            var mianUICanvas = GameManager.GetModule<IResourceManager>().LoadPrefabAsync(new AssetInfo(null, null, "UI/MainUICanvas"),(go=> 
            {
                GameManager.GetModule<IUIManager>().SetUIRoot(go);
            }),null,true);
            //InitUtility();
        }
        void InitUtility()
        {
            Utility.Json.SetHelper(new JsonUtilityHelper());
        }
    }
}