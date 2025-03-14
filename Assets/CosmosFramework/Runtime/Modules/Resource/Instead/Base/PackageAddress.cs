using System.Collections.Generic;
using System.IO;

namespace Cosmos.Resource
{
    /// <summary>
    /// 资源包内寻址类
    /// </summary>
    internal class PackageAddress
    {
        /// <summary>
        /// assetName===[Lnk===AssetInfo]
        /// </summary>
        readonly Dictionary<string, LinkedList<AssetInfo>> assetInfoLnkMap = new Dictionary<string, LinkedList<AssetInfo>>();
        /// <summary>
        /// assetPath===[ AssetInfo ]
        /// </summary>
        readonly Dictionary<string, AssetInfo> assetInfoPathMap = new Dictionary<string, AssetInfo>();
        /// <summary>
        /// 初始化寻址文件
        /// </summary>
        /// <param name="assetInfos">资产信息</param>
        public void Init(IEnumerable<AssetInfo> assetInfos)
        {
            assetInfoLnkMap.Clear();
            foreach (var assetInfo in assetInfos)
            {
                if (!assetInfoLnkMap.TryGetValue(assetInfo.Name, out var lnk))
                {
                    lnk = new LinkedList<AssetInfo>();
                    assetInfoLnkMap.Add(assetInfo.Name, lnk);
                }
                lnk.AddLast(assetInfo);
                assetInfoPathMap[assetInfo.AssetPath] = assetInfo;
            }
        }
        /// <summary>
        /// 重置寻址文件
        /// </summary>
        public void Reset()
        {
            assetInfoLnkMap.Clear();
        }
        /// <summary>
        /// 查询寻址对象
        /// <para>assetKey能够识别以下几种类型：assetName，assetName.ext，assetPath</para>
        /// <para>assetName: 资产文件名，e.g. myText</para>
        /// <para>assetName.ext: 资产文件名加上后缀，e.g. myText.txt</para>
        /// <para>assetPath: 资产路径，e.g. Assets/Text/myText.txt</para>
        /// </summary>
        /// <param name="assetKey">资产识别标识</param>
        /// <param name="assetInfo">资产信息</param>
        /// <returns>是否查询成功</returns>
        public bool PeekAssetInfo(string assetKey, out AssetInfo assetInfo)
        {
            assetInfo = null;
            if (assetKey.StartsWith("Assets/"))
            {
                //Assets/开头，表示assetKey值是完整路径
                var has = assetInfoPathMap.TryGetValue(assetKey, out var _assetInfo);
                assetInfo = _assetInfo;
                return has;
            }
            else
            {
                var ext = Path.GetExtension(assetKey);
                if (string.IsNullOrEmpty(ext))
                {
                    //文件后缀名为空
                    if (assetInfoLnkMap.TryGetValue(assetKey, out var lnk))
                    {
                        //默认返回第一个
                        assetInfo = lnk.First.Value;
                        return true;
                    }
                }
                else
                {
                    //若文件后缀名存在
                    //获取无后的文件名
                    var nameWithoutExt = Path.GetFileNameWithoutExtension(assetKey);
                    var lowerExt = ext.ToLower();
                    if (assetInfoLnkMap.TryGetValue(nameWithoutExt, out var lnk))
                    {
                        foreach (var asset in lnk)
                        {
                            //返回后缀名匹配的
                            if (asset.Extension == lowerExt)
                            {
                                assetInfo = asset;
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

    }
}
