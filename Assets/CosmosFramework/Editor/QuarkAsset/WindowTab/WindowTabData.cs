namespace CosmosEditor.Quark
{
    internal class WindowTabData
    {
        /// <summary>
        /// 目标为Asset下的所有目录
        /// </summary>
        public bool UnderAssetsDirectory { get; set; }
        /// <summary>
        /// 是否生成路径地址代码；
        /// </summary>
        public bool GenerateAssetPathCode { get; set; }
        /// <summary>
        /// QuarkAssetDataset对象在Assets目录下的相对路径；
        /// </summary>
        public string QuarkAssetDatasetPath { get; set; }
    }
}
