using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System.Text;

public class EncryptionTester : MonoBehaviour
{
    string aesKey = "1416515516116166";
    void Start()
    {
        var key = Encoding.UTF8.GetBytes(aesKey);
        string str = Utility.Encryption.AESEncryptStringToString("尤格索托斯", key);
        Utility.Debug.LogInfo(str);
        Utility.Debug.LogInfo(Utility.Encryption.AESDecryptStringToString(str, key));
    }
}
