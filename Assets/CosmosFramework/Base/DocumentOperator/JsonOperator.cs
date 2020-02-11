using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;
namespace Cosmos.IO {
    public class JsonDocOperator : DocumentOperator<JsonData>
    {
        public override JsonData CreateEmptyDocument(string fullPath, CFAction<JsonData> callBack = null)
        {
            throw new System.NotImplementedException();
        }

        public override void DeleteDocument(string fullPath, CFAction<JsonData> callBack = null)
        {
            throw new System.NotImplementedException();
        }

        public override JsonData EditDocument(JsonData doc, CFAction<JsonData> callBack = null)
        {
            throw new System.NotImplementedException();
        }

        public override void ParseDocument(TextAsset ta, CFAction<JsonData> callBack = null)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveDocument(JsonData doc, string savePath, CFAction<JsonData> callBack = null)
        {
            throw new System.NotImplementedException();
        }
    }
}