using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    public sealed partial class ApplicationConst
    {
        /// <summary>
        /// PlayerPrefs持久化前缀
        /// </summary>
        public const string APPPERFIX = "Cosmos";
        /// <summary>
        /// Unity的路径
        /// </summary>
        public static string ApplicationDataPath { get { return Application.dataPath; } }
    }
}