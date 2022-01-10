using System;

namespace Cosmos
{
    public static partial class Utility
    {
        public static class Bson
        {
            public interface IBsonHelper
            {
                byte[] ToBson<T>(T obj);
                T FromBson<T>(byte[] bson);
            }
            static IBsonHelper  bsonHelper= null;
            public static void SetHelper(IBsonHelper helper)
            {
                bsonHelper= helper;
            }
            public static void ClearHelper()
            {
                bsonHelper= null;
            }
            public static byte[] ToBson<T>(T obj)
            {
                if (bsonHelper == null)
                    throw new ArgumentNullException("Bson helper is invalid");
                try
                {
                    return bsonHelper.ToBson<T>(obj);
                }
                catch (Exception exception)
                {
                    throw new ArgumentException($"Can not convert to Bson with exception {exception}");
                }
            }
            public static string ToBsonString<T>(T obj)
            {
                if (bsonHelper == null)
                    throw new ArgumentNullException("Bson helper is invalid");
                try
                {
                    var bosn= bsonHelper.ToBson<T>(obj);
                    return Convert.ToBase64String(bosn);
                }
                catch (Exception exception)
                {
                    throw new ArgumentException($"Can not convert to Bson with exception {exception}");
                }
            }
            public static T FromBson<T>(byte[] bson)
            {
                if (bsonHelper == null)
                    throw new ArgumentNullException("Bson helper is invalid");
                try
                {
                    return bsonHelper.FromBson<T>(bson);
                }
                catch (Exception exception)
                {
                    throw new ArgumentException($"Can not convert to Bson with exception {exception}");
                }
            }
            public static T FromBson<T>(string base64data)
            {
                byte[] bson= Convert.FromBase64String(base64data);
                return FromBson<T>(bson);
            }
        }
    }
}
