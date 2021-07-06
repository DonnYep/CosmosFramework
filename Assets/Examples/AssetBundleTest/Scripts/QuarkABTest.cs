using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos.Quark;
using Cosmos;
using System;
using System.IO;
using System.Text;
using System.Net;

public class QuarkABTest : MonoBehaviour
{
    [SerializeField] string remoteUrl;
    [SerializeField] string localUrl;
    QuarkABLoader quarkAssetABLoader;
    Dictionary<int, string> testDict = new Dictionary<int, string>();
    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            testDict.Add(i, i.ToString());
        }

        //foreach (var  v in testDict)
        //{
        //    Utility.Debug.LogInfo(v.Value);
        //}

        var enumerator = testDict.GetEnumerator();
        Utility.Debug.LogInfo(enumerator.Current.Value);
        while (enumerator.MoveNext())
        {
            Utility.Debug.LogInfo(enumerator.Current.Value);
        }
    }
}
