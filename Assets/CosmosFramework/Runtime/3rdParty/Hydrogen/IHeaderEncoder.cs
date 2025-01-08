namespace Hydrogen
{
    public interface IHeaderEncoder
    {
        /// <summary>
        /// [header][message]，获取header的长度
        /// </summary>
        /// <returns>header的长度，通常返回2或4</returns>
        int GetHeaderLength();
        byte[] EncodeHeader(ushort length);
        ushort DecodeHeader(byte[] header);
    }
}
