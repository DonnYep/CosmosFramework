using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
public class EncryptionTester : MonoBehaviour
{
    void Start()
    {
        var iv = Utility.Encryption.GenerateIV("尤格索托斯");
        string str = Utility.Encryption.DESEncrypt("尤格索托斯", "12345678",iv);
        Utility.Debug.LogInfo(str);
        Utility.Debug.LogInfo(Utility.Encryption.DESDecrypt(str, "12345678",iv));
        var result = Utility.Encryption.GenerateIV("尤格索托斯");
        for (int i = 0; i < result.Length; i++)
        {
            Utility.Debug.LogInfo(result[i].ToString("X2"),MessageColor.PURPLE);
        }
    }
}
