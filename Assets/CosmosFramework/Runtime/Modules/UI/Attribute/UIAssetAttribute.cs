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
        public UIAssetAttribute(string uiAssetName, string resourcePath) : base(resourcePath) 
        {
            this.UIAssetName = uiAssetName;
        }
        public UIAssetAttribute(string uiAssetName, string assetBundleName, string assetPath, string resourcePath)
            : base(assetBundleName, assetPath, resourcePath)
        {
            this.UIAssetName = uiAssetName;
        }
    }
}