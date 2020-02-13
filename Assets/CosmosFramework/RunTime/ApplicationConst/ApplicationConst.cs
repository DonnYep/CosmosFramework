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
        public const string _AllXmlRootName= "ResourceRoot";
        public const string _XmlPathName= "path";
        public const string _XmlItemName= "item";
        public const string _XmlCountName= "assetCount";
        public const string _MainUICanvansPath= "UI/MainUICanvans";
        public static string DefaultConfigXmlName { get { return "ResourcesConfig.xml"; } }
        //完整路径
        public static string DefaultConfigXmlPath { get { return Application.dataPath + "/Resources/"; } }
        //完整路径
        public static string DefualtConfigXmlFullPath { get { return DefaultConfigXmlPath + "/" + DefaultConfigXmlName; } }
        //默认的InputEventKey。 后面的字符我也不知道什么意思，但是能确保乱码不会让事件重复
        public const string _InputEventKey = "CF_InputEventKey_5%csfijok";
        //默认的ControllerEventKey。使用设备，例如Vehicle、Device时所使用的Key
        public const string _ControllerEventKey = "CF_UseingDevice_%418asdf";
        //Unity的路径
        public static string ApplicationDataPath { get { return Application.dataPath; } }
    }
}