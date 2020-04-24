using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;
namespace Cosmos.Test {
    public class UIEntry : MonoBehaviour
    {
        private void Start()
        {
            var result = Facade.Instance.InitMainCanvas("UI/MainUICanvas");
            InitUtility();
        }
        void InitUtility()
        {
            Utility.Json.SetJsonWarpper(new JsonUtilityWarpper());
        }
    }
}