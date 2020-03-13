using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Cosmos
{
    [CreateAssetMenu(fileName ="NewStringContent",menuName ="CosmosFramework/StringContent")]
    public class StringContent: CFDataSet
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
        public override void Reset()
        {
            content = new string[]{ };
        }
    }
}