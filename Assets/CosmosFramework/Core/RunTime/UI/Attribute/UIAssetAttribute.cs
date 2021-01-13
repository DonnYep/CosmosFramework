using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    public class UIAssetAttribute : PrefabAssetAttribute
    {
        public UIAssetAttribute(string resourcePath) : base(resourcePath) { }
        public UIAssetAttribute(string assetBundleName, string assetPath, string resourcePath)
            : base(assetBundleName, assetPath, resourcePath) { }
    }
}