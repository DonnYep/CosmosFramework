using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    public class UIAssetAttribute : PrefabAssetAttribute
    {
        public string UIAssetName { get; private set; }
        public string UIGroupName { get; private set; }
        public UIAssetAttribute(string uiAssetName, string resourcePath) : base(resourcePath) 
        {
            this.UIAssetName = uiAssetName;
        }
        public UIAssetAttribute(string uiAssetName, string uiGroupName,string resourcePath) : base(resourcePath)
        {
            this.UIAssetName = uiAssetName;
            this.UIGroupName = uiGroupName;
        }
        public UIAssetAttribute(string uiAssetName, string assetBundleName, string assetPath, string resourcePath)
            : base(assetBundleName, assetPath)
        {
            this.UIAssetName = uiAssetName;
        }
    }
}