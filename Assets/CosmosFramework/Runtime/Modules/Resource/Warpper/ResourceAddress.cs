using System.Collections.Generic;
using System.IO;

namespace Cosmos.Resource
{
    //================================================
    /*
     * 1、AssetBundle支持 assetname，assetname.ext， assets/../assetname.ext
     *  等忽略大小写的加载方式。
     *  
     *  2、此资源寻址方案根据AssetBundle加载特性，采用完整地址方式加载。
     *  
     *  3、加载时也支持通过assetname与assetname.ext加载。此二种主要用于
     *  寻址到资源的完整路径，最终还是通过完整路径对资源进行加载。
     *  
     *  4、初始化时自动填充所有资源地址。
    */
    //================================================
    public class ResourceAddress
    {
        /// <summary>
        /// assetName===[Lnk===ResourceObject]
        /// </summary>
        readonly Dictionary<string, LinkedList<ResourceObject>> resourceObjectLnkDict;
        public ResourceAddress()
        {
            resourceObjectLnkDict = new Dictionary<string, LinkedList<ResourceObject>>();
        }
        public void AddResourceObjects(IEnumerable<ResourceObject> resourceObjects)
        {
            foreach (var resourceObject in resourceObjects)
            {
                if (!resourceObjectLnkDict.TryGetValue(resourceObject.AssetName, out var lnk))
                {
                    lnk = new LinkedList<ResourceObject>();
                    resourceObjectLnkDict.Add(resourceObject.AssetName, lnk);
                }
                lnk.AddLast(resourceObject);
            }
        }
        public void RemoveResourceObjects(IEnumerable<ResourceObject> resourceObjects)
        {
            foreach (var resourceObject in resourceObjects)
            {
                if (resourceObjectLnkDict.TryGetValue(resourceObject.AssetName, out var lnk))
                {
                    lnk.Remove(resourceObject);
                    if (lnk.Count == 0)
                        resourceObjectLnkDict.Remove(resourceObject.AssetName);
                }
            }
        }
        public bool PeekAssetPath(string assetName, out string assetPath)
        {
            assetPath = string.Empty;
            if (assetName.StartsWith("Assets/"))
            {
                //若以Assets/开头 ，则表示为完整路径
                assetPath = assetName;
                return true;
            }
            else
            {
                var ext = Path.GetExtension(assetName);
                if (string.IsNullOrEmpty(ext))
                {
                    //若文件后缀名为空
                    if (resourceObjectLnkDict.TryGetValue(assetName, out var lnk))
                    {
                        //默认返回第一个
                        assetPath = lnk.First.Value.AssetPath;
                    }
                }
                else
                {
                    //若文件后缀名存在
                    //获取无后的文件名
                    var nameWithoutExt = Path.GetFileNameWithoutExtension(assetName);
                    if (resourceObjectLnkDict.TryGetValue(nameWithoutExt, out var lnk))
                    {
                        foreach (var resourceObject in lnk)
                        {
                            //返回后缀名匹配的
                            if (resourceObject.Extension == ext)
                            {
                                assetPath = resourceObject.AssetPath;
                                break;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public void Clear()
        {
            resourceObjectLnkDict.Clear();
        }
    }
}
