using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
namespace Cosmos.IO
{
    public class XmlOperator : DocumentOperator<XmlDocument>
    {
        public override XmlDocument CreateEmptyDocument(string fullPath, CFAction<XmlDocument> callBack = null)
        {
            throw new System.NotImplementedException();
        }

        public override void DeleteDocument(string fullPath, CFAction<XmlDocument> callBack = null)
        {
            throw new System.NotImplementedException();
        }
        public override XmlDocument EditDocument(XmlDocument doc, CFAction<XmlDocument> callBack = null)
        {
            throw new System.NotImplementedException();
        }

        public override void ParseDocument(TextAsset ta, CFAction<XmlDocument> callBack = null)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveDocument(XmlDocument doc, string savePath, CFAction<XmlDocument> callBack = null)
        {
            throw new System.NotImplementedException();
        }
    }
}