using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos
{
    public class ApplicationConst
    {
        /// <summary>
        /// 前缀
        /// </summary>
        public const string _AppPerfix = "Cosmos";
        //默认文件扩展名表，“FileExtensionList”是这个表Path
        static string fileExtensionList = "FileExtensionList";
        public static string FileExtensionList { get { return fileExtensionList; } set { fileExtensionList = value; } }
        public const string ALL_XMLROOT_NAME= "ResourceRoot";
        public const string XML_PATH_NAME= "path";
        public const string XML_ITEM_NAME= "item";
        public const string XML_COUNT_NAME= "assetCount";
        public const string MAIN_UICANVANS_PATH= "UI/MainUICanvans";
        public static string DefaultConfigXmlName { get { return "ResourcesConfig.xml"; } }
        //完整路径
        public static string DefaultConfigXmlPath { get { return Application.dataPath + "/Resources/"; } }
        //完整路径
        public static string DefualtConfigXmlFullPath { get { return DefaultConfigXmlPath + "/" + DefaultConfigXmlName; } }
        //Unity的路径
        public static string ApplicationDataPath { get { return Application.dataPath; } }
    }
}