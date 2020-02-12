using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.IO
{
    public interface IDocumentOperator
    {
       void ParseDocument(TextAsset ta);
    }
}