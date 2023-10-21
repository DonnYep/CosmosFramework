﻿using System;
namespace Cosmos.Editor.Resource
{
    [Serializable]
    public class ResourceBuilderWindowData
    {
        /// <summary>
        /// Assets目录下，ResourceDataset的文件地址；
        /// </summary>
        public string ResourceDatasetPath;
        /// <summary>
        /// 被选择的标签序号；
        /// </summary>
        public int SelectedTabIndex;

        public ResourceBuilderWindowData()
        {
            ResourceDatasetPath = ResourceEditorConstants.DEFAULT_DATASET_PATH;
        }
    }
}
