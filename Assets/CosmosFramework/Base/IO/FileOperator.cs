using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
namespace Cosmos.IO
{
    public class FileOperator
    {
        public static string[] FileExtensions { get { return fileExtensions; } }
        static string[] fileExtensions;
        /// <summary>
        /// 文件相对路径的存储
        /// </summary>
        List<string> dirs = new List<string>();
        #region IO operate
        /// <summary>
        ///删除文件 
        /// </summary>
        public static  void DeleteFile(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            else
                Utility.DebugError(Utility.Unity.DecomposeAppAbsolutePath(path) + "\t" + "does not exist!");
        }
        /// <summary>
        ///文件是否存在 
        /// </summary>
        public static  bool ExistFile(string path)
        {
            if (File.Exists(path))
                return true;
            else
                return false;
        }
        /// <summary>
        ///文件夹是否存在 
        /// </summary>
        public static  bool ExistDirectory(string path)
        {
            if (Directory.Exists(path))
                return true;
            else
                return false;
        }
        /// <summary>
        ///创建文件夹 
        /// </summary>
        public static  void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        public static  void DeleteFloder(string path)
        {
            if (Directory.Exists(path))
                Directory.Delete(path);
            else
                Utility.DebugError("floder does not exist!\t" + path);
        }
        #endregion
        /// <summary>
        /// 获取指定路径下的asset的相对路径
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public void GetAssetRelativePath(string dirPath ,ref List<string> path)
        {
            DirectoryInfo di = new DirectoryInfo(dirPath);
            FileSystemInfo[] fsi = di.GetFileSystemInfos();
            foreach ( FileSystemInfo item in fsi)
            {
                if(item is DirectoryInfo)
                {
                    GetAssetRelativePath(item.FullName,ref path);
                }
                else
                {
                    if (IsTargetAsset(Path.GetExtension(item.FullName)))
                    {
                        string[] pathArray = item.FullName.Split(new string[] { "Assets\\" }, StringSplitOptions.RemoveEmptyEntries);//根据数组裁切字段
                        string subPath = "Assets\\" + pathArray[1];
                        path.Add(subPath);
                    }
                }
            }
        }
        public void LoadRelativePathAsset(string sourcePath)
        {
            if (string.IsNullOrEmpty(sourcePath))
            {
                if (Directory.Exists(ApplicationConst.ApplicationDataPath))
                  GetAssetRelativePath(ApplicationConst.ApplicationDataPath,ref dirs);
            }
            else
            {
                string filePath = Utility.Unity.CombineAppAbsolutePath(sourcePath);
                //不区分地址大小写
                if (Directory.Exists(filePath))
                {
                    GetAssetRelativePath(filePath,ref dirs);
                }
                else
                {
                    Utility.DebugError("Load file does not exist,check your path!");
                }
            }
        }
        public bool IsTargetAsset(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            for (int i = 0; i < FileExtensions.Length; i++)
            {
                if (path.Contains(FileExtensions[i]))
                    return true;
            }
            return false;
        }
        #region XmlFileOperate
       XMLFileOperator xo = new XMLFileOperator();
        public void ParseDefaultFileExtentionsList()
        {
            xo.ParseDefaultFileExtentionsList(ref fileExtensions);
        }
        public void DeleteXmlFile(string filePath, string fileName)
        {
            xo.DeleteXmlFile(filePath, fileName);
        }
        public void OutputXmlFile(string outputPath, string fileName)
        {
            xo.OutputXmlFile(outputPath, fileName, dirs);
            dirs.Clear();
        }
        /// <summary>
        ///留着，也是获取子物体、孙物体的方法 
        /// </summary>
        public void GetDir(string dirPath, ref List<string> dirs)
        {
            foreach (string path in Directory.GetFiles(dirPath))
            {
                if (IsTargetAsset(Path.GetExtension(path)))
                {
                    string subPath = path.Substring(path.IndexOf("Assets\\", 7));
                    //删除Assets/这个字段，直接从Assets 根目录读取文件
                    subPath = subPath.Remove(0, 7);
                    dirs.Add(subPath);
                }
            }
            if (Directory.GetDirectories(dirPath).Length > 0)
            {
                foreach (string path in Directory.GetDirectories(dirPath))
                {
                    GetDir(path, ref dirs);
                }
            }
        }
        #endregion
        #region JsonFileOperate
        JsonFileOperator jo = new JsonFileOperator();
        public void CreateEmptyJsonFile(string filePath,string fileName)
        {
            string fullPath = filePath + fileName;

            jo.CreateEmptyJson(fullPath);
        }
        public void CreateEmptyJsonFile(string fullPath)
        {
            jo.CreateEmptyJson(fullPath);
        }
        #endregion
    }
}