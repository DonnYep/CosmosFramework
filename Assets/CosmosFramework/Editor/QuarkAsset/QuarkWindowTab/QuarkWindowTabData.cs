namespace Quark.Editor
{
    internal class QuarkWindowTabData
    {
        /// <summary>
        /// 是否生成路径地址代码；
        /// </summary>
        public bool GenerateAssetPathCode { get; set; }
        /// <summary>
        /// 排序资源文件夹
        /// </summary>
        public bool SortAssetBundle{ get; set; }
        /// <summary>
        /// 排序资源；
        /// </summary>
        public bool SortAssetObjectList{ get; set; }
        /// <summary>
        /// QuarkAssetDataset对象在Assets目录下的相对路径；
        /// </summary>
        public string QuarkAssetDatasetPath { get; set; }
    }
}
