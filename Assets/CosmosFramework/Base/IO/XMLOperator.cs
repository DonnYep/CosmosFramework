using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
namespace Cosmos.IO{
    public class XMLOperator
    {
        public void ParseDefaultFileExtentionsList(ref string[] extention)
        {
            TextAsset ta = Utility.Load<TextAsset>(ApplicationConst.FileExtensionList);
            if (ta == null)
            {
                Utility.DebugError("load text asset fail ,file :" + ApplicationConst.FileExtensionList + " does not exist!\nconfigManager");
                return;
            }
          extention= ParseFileExtentionsList(ta);
        }
        string[] ParseFileExtentionsList(TextAsset ta)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(ta.text);
            XmlNodeList node = xmlDoc.SelectSingleNode(ApplicationConst._AllXmlRootName).ChildNodes;
            List<string> list = new List<string>();
            foreach (XmlElement ele in node)
            {
                foreach (XmlElement subEle in ele.ChildNodes)
                {
                    if (subEle.Name == ApplicationConst._XmlItemName)
                    {
                        list.Add(subEle.InnerText);
                    }
                }
            }
            return  list.ToArray();
        }
        /// <summary>
        /// 生成空的XML文件
        /// </summary>
        /// <param name="fullPath">完整的绝对路径</param>
        /// <param name="fileName">完整的文件名</param>
        /// <returns></returns>
        public XmlDocument CreateEmptyXmlFile(string fullPath, string fileName)
        {
            XmlDocument xml = new XmlDocument();
            XmlDeclaration xmldel = xml.CreateXmlDeclaration("1.0", "UTF-8", "");
            xml.AppendChild(xmldel);
            XmlElement root = xml.CreateElement(ApplicationConst._AllXmlRootName);
            xml.AppendChild(root);
            string fullFilePath = Utility.CombineRelativeFilePath(fileName, fullPath);
            xml.Save(fullFilePath);
            Utility.DebugLog("Config file is created, Check your path!");
            return xml;
        }
        public XmlDocument CreateEmptyXmlFile(string absolutePath)
        {
            XmlDocument xml = new XmlDocument();
            XmlDeclaration xmldel = xml.CreateXmlDeclaration("1.0", "UTF-8", "");
            xml.AppendChild(xmldel);
            XmlElement root = xml.CreateElement(ApplicationConst._AllXmlRootName);
            xml.AppendChild(root);
            xml.Save(absolutePath);
            Utility.DebugLog("Config file is created, Check your path!");
            return xml;
        }
        public void EditXmlFile(ref XmlDocument doc, string[] filePaths, string fullPath = null)
        {
            IO.FileOperator fo = new IO.FileOperator();
            int assetCount = 0;
            int index = 1;
            string lastPath = "";
            int subRootCount = -1;
            List<XmlElement> eleList = new List<XmlElement>();
            List<string> subRootName = new List<string>();
            XmlNode node = doc.SelectSingleNode(ApplicationConst._AllXmlRootName);
            for (int i = 0; i < filePaths.Length; i++)
            {
                //截取第N个包含“/”字符的作为一个xml的标签
                string[] rootPath = filePaths[i].Split('\\');
                if (! fo.IsTargetAsset(rootPath[index]))
                {
                    if (lastPath != rootPath[index])
                    {
                        subRootCount++;
                        lastPath = rootPath[index];
                        subRootName.Add(lastPath);
                        XmlElement subRoot = doc.CreateElement(subRootName[subRootCount]);
                        eleList.Add(subRoot);
                        node.AppendChild(subRoot);
                    }
                    XmlElement inner = doc.CreateElement(ApplicationConst._XmlPathName);
                    if (filePaths[i].Contains("Assets\\"))
                    {
                        inner.InnerText = filePaths[i];
                    }
                    else
                    {
                        inner.InnerText = "Assets\\" + filePaths[i];
                    }
                    eleList[subRootCount].AppendChild(inner);
                    assetCount++;
                }
            }
            //创建资源总数的标签
            XmlElement count = doc.CreateElement(ApplicationConst._XmlCountName);
            count.InnerText = assetCount.ToString();
            node.AppendChild(count);
            if (string.IsNullOrEmpty(fullPath))
                doc.Save(ApplicationConst.DefualtConfigXmlFullPath);
            else
                doc.Save(fullPath);
        }
        public void OutputXmlFile(string outputPath, string fileName,List<string> dirs)
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                Utility.DebugLog("Output default reosurce");
                if ( File.Exists(ApplicationConst.DefualtConfigXmlFullPath))
                {
                    XmlDocument doc = CreateEmptyXmlFile(ApplicationConst.DefualtConfigXmlFullPath);
                   EditXmlFile(ref doc, dirs.ToArray());
                }
                else
                {
                   File.Delete(ApplicationConst.DefualtConfigXmlFullPath);
                    XmlDocument doc = CreateEmptyXmlFile(ApplicationConst.DefualtConfigXmlFullPath);
                    EditXmlFile(ref doc, dirs.ToArray());
                }
                //使用后清空
            }
            else
            {
                string outPath = Utility.CombineAbsolutePath(outputPath);//绝对路径
                if (Directory.Exists(outPath))
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        string fullFilePath = Utility.CombineRelativeFilePath(ApplicationConst.DefaultConfigXmlName,outPath);
                        File.Delete(fullFilePath);
                        XmlDocument doc = CreateEmptyXmlFile(outPath, ApplicationConst.DefaultConfigXmlName);
                        EditXmlFile(ref doc, dirs.ToArray(), fullFilePath);
                    }
                    else
                    {
                        string fullFileName = fileName + ".xml";
                        string fullFilePath = Utility.CombineRelativeFilePath(fullFileName, outPath);
                        XmlDocument doc = CreateEmptyXmlFile(outPath, fullFileName);
                        EditXmlFile(ref doc, dirs.ToArray(), fullFilePath);
                    }
                }
                else
                {
                    //文件夹不存在，则创建文件夹
                  Directory.CreateDirectory(outPath);
                    if (string.IsNullOrEmpty(fileName))
                    {
                        string fullFilePath = Utility.CombineRelativeFilePath( outPath,ApplicationConst.DefaultConfigXmlName);
                        File.Delete(fullFilePath);
                        XmlDocument doc = CreateEmptyXmlFile(outPath, ApplicationConst.DefaultConfigXmlName);
                        EditXmlFile(ref doc, dirs.ToArray(), fullFilePath);
                    }
                    else
                    {
                        string fullFileName = fileName + ".xml";
                        //string fullPath = outPath + fullFileName;
                         string fullFilePath= Utility.CombineRelativeFilePath( fullFileName,outPath);
                        XmlDocument doc = CreateEmptyXmlFile(outPath, fullFileName);
                        EditXmlFile(ref doc, dirs.ToArray(), fullFilePath);
                    }
                }
            }
        }
        public void DeleteXmlFile(string relativePath, string fileName)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                if (string.IsNullOrEmpty(fileName))
                {
                    if (File.Exists(ApplicationConst.DefualtConfigXmlFullPath))
                      File.Delete(ApplicationConst.DefualtConfigXmlFullPath);
                    else
                      Directory.Delete(ApplicationConst.DefaultConfigXmlPath);
                }
            }
            else
            {
                string fullAbsolutePath = Utility.CombineAbsolutePath(relativePath);
                if (string.IsNullOrEmpty(fileName))
                {
                    string fullAbsoluteFilePath = Utility.CombineRelativeFilePath(ApplicationConst.DefaultConfigXmlName, fullAbsolutePath);
                    if (File.Exists(fullAbsoluteFilePath))
                        File.Delete(fullAbsoluteFilePath);
                    else
                        Directory.Delete(fullAbsolutePath);
                }
                else
                {
                    string fullFilePath = Utility.CombineRelativeFilePath(fileName + ".xml", fullAbsolutePath);
                    if (File.Exists(fullFilePath))
                        File.Delete(fullFilePath);
                    else
                        Utility.DebugError("file:   " + Utility.CombineRelativeFilePath(fileName+".xml")+ "\t dose not exist, check your path!");
                }
            }
        }
    }
}