using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.UI;

namespace Cosmos.Test {
    public class UIEntry : MonoBehaviour
    {
        [SerializeField]
        [TextArea]
        string json;
        private void Awake()
        {
            Utility.SetDebugHelper(new UnityDebugHelper());
        }
        private void Start()
        {
            var result = Facade.InitMainCanvas("UI/MainUICanvas");
            InitUtility();
            Debug.Log("UI日志输出测试---Log");
            Debug.LogError("UI日志输出测试---Error");
            Debug.LogWarning("UI日志输出测试---Warn");
            Utility.DebugLog(json);
        }
        void InitUtility()
        {
            Utility.Json.SetHelper(new JsonUtilityHelper());
        }
    }
}