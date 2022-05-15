using Quark.Asset;
namespace Quark.Editor
{
    public class QuarkEditorDataProxy
    {
        public static QuarkAssetDataset  QuarkAssetDataset{ get; set; }
        internal static QuarkAssetDatabaseTabData QuarkAssetDatabaseTabData { get; set; }
        internal static QuarkAssetBundleTabData QuarkAssetBundleTabData { get; set; }
    }
}
