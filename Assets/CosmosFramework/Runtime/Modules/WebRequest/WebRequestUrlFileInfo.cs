using System.Runtime.InteropServices;
namespace Cosmos.WebRequest
{
    /// <summary>
    /// 文件信息
    /// <para>假设url=http://myftp/myfolder/subfoler/myfile.txt </para>
    /// <para><see cref="URL"/>= http://myftp/myfolder/subfoler/myfile.txt </para>
    /// <para><see cref="FileName"/>= myfile.txt </para>
    /// <para><see cref="RelativeUrl"/>= /myfolder/subfoler </para>
    /// </summary>
    [StructLayout(LayoutKind.Auto)]
    public struct WebRequestUrlFileInfo
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName;
        /// <summary>
        /// 文件地址。 
        /// </summary>
        public string URL;
        /// <summary>
        /// 相对地址。 <see cref="WebRequestUrlFileInfo"/>
        /// </summary>
        public string RelativeUrl;
    }
}
