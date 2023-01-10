namespace Cosmos.Resource.Verifiy
{
    public class ResourceManifestVerifierResult
    {
        /// <summary>
        /// 校验成功的信息；
        /// </summary>
        public ResourceManifestVerifiyInfo[] VerifiySuccessInfos; 
        /// <summary>
        /// 校验失败的信息；
        /// </summary>
        public ResourceManifestVerifiyInfo[] VerifiyFailureInfos; 
    }
}
