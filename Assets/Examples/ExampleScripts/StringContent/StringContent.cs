using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    [CreateAssetMenu(fileName ="NewStringContent",menuName ="CosmosFramework/StringContent")]
    public class StringContent: DatasetBase
    {
        //[SerializeField]
        public string[] content;
        public string[] Content { get { return content; }
            set
            {
                content = value;
                if (content.Length >= 32)
                    Array.Resize(ref content, 32);
            }
        }
        public override void Dispose()
        {
            content = new string[]{ };
        }
    }
}