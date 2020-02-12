using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
namespace Cosmos.IO {
    public class JsonDocOperator : DocumentOperator<JsonData>
    {
        public override JsonData CreateEmptyDocument(string fullPath)
        {
            throw new System.NotImplementedException();
        }
        public override void DeleteDocument(string fullPath)
        {
            throw new System.NotImplementedException();
        }

        public override JsonData EditDocument(JsonData doc)
        {
            throw new System.NotImplementedException();
        }

        public override void ParseDocument(TextAsset ta)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveDocument(JsonData doc, string savePath)
        {
            throw new System.NotImplementedException();
        }
    }
}