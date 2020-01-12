using UnityEngine;
using System.Collections;
using System;
namespace Cosmos.Resource
{
    public enum ResourceType:int
    {

        PREFAB=0,
        LEVEL=1,
    }
    public class ResourceUnit : IDisposable
    {
        string path;
        object asset;

        public void Dispose(){ }
        public void Json()
        {
        }
    }
}