using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
namespace Cosmos.IO
{
    public class XmlOperator : DocumentOperator<XmlDocument>
    {
        public override XmlDocument CreateEmptyDocument(string fullPath)
        {
            throw new System.NotImplementedException();
        }

        public override void DeleteDocument(string fullPath)
        {
            throw new System.NotImplementedException();
        }

        public override XmlDocument EditDocument(XmlDocument doc)
        {
            throw new System.NotImplementedException();
        }

        public override void ParseDocument(TextAsset ta)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveDocument(XmlDocument doc, string savePath)
        {
            throw new System.NotImplementedException();
        }
    }
}