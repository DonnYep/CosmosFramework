namespace Cosmos.Resource.Verify
{
    public class ResourceManifestVerifyResult
    {
        /// <summary>
        /// 校验成功的信息；
        /// </summary>
        public ResourceManifestVerifyInfo[] VerifySuccessInfos; 
        /// <summary>
        /// 校验失败的信息；
        /// </summary>
        public ResourceManifestVerifyInfo[] VerifyFailureInfos; 
    }
}
