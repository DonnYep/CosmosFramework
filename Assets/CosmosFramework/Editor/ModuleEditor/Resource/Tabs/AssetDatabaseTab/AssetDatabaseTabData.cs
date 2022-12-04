using System;
using System.Collections.Generic;

namespace Cosmos.Editor.Resource
{
    [Serializable]
    public class AssetDatabaseTabData
    {
        List<int> selectedBundleIds;
        /// <summary>
        /// 当前被选择的bundleId集合
        /// </summary>
        public List<int> SelectedBundleIds
        {
            get
            {
                if (selectedBundleIds == null)
                    selectedBundleIds = new List<int>();
                return selectedBundleIds;
            }
            set
            {
                selectedBundleIds = value;
                if (selectedBundleIds == null)
                    selectedBundleIds = new List<int>();
            }
        }
    }
}
