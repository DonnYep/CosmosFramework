using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos{
    [CreateAssetMenu(fileName ="NewStringContent",menuName ="CosmosFramework/StringContent")]
    public class StringContent: CFDataSet
    {
        [SerializeField]
        string[] content;
        public string[] Content { get { return content; } }
        public override void Reset()
        {
            content = null;
        }
    }
}