namespace Cosmos.Resource
{
    public struct ResourceIntegrityInfo
    {
        public long RecordedBundleSize { get; private set; }
        public long DetectedBundleSize { get; private set; }
        public string BundleKey { get; private set; }
        public string BundleName { get; private set; }
        public ResourceIntegrityInfo(long recordedBundleSize, long detectedBundleSize, string bundleKey, string bundleName)
        {
            RecordedBundleSize = recordedBundleSize;
            DetectedBundleSize = detectedBundleSize;
            BundleKey = bundleKey;
            BundleName = bundleName;
        }
    }
}
