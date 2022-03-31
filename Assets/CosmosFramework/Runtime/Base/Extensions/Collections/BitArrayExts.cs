using System.Collections;
namespace Cosmos
{
    public static class BitArrayExts
    {
        public static byte[] ToByteArray(this BitArray @this)
        {
            var result = new byte[@this.GetArrayLength(8)];
            @this.CopyTo(result, 0);
            return result;
        }
        public static int[] ToInt32Array(this BitArray @this)
        {
            var result = new int[@this.GetArrayLength(32)];
            @this.CopyTo(result, 0);
            return result;
        }
        public static int GetArrayLength(this BitArray @this, int div)
        {
            return GetArrayLength(@this.Length, div);
        }
        private static int GetArrayLength(int n, int div)
        {
            return n > 0 ? (n - 1) / div + 1 : 0;
        }
    }
}
