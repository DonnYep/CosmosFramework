using System;
using System.Collections.Generic;
using System.Text;
namespace Cosmos
{
    public static partial class Utility
    {
        public static class MessagePack
        {
            static IMessagePackHelper messagePackHelper = null;
            public static void SetHelper(IMessagePackHelper helper)
            {
                messagePackHelper = helper;
            }
            public static void ClearHelper()
            {
                messagePackHelper = null;
            }
            public static string ToJson<T>(T obj)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("messagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.ToJson(obj);
                }
                catch (Exception exception)
                {
                    throw new ArgumentNullException(Utility.Text.Format("Can not convert to JSON with exception '{0}", exception.ToString()), exception);
                }
            }
            public static byte[] ToByteArray(object obj)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("messagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.ToByteArray(obj);
                }
                catch (Exception exception)
                {
                    throw new ArgumentNullException(Utility.Text.Format("Can not convert to ByteArray with exception '{0}", exception.ToString()), exception);
                }
            }
            public static T ToObject<T>(byte[] buffer)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("messagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.ToObject<T>(buffer);
                }
                catch (Exception exception)
                {
                    throw new ArgumentNullException(Utility.Text.Format("Can not convert to ByteArray with exception '{0}", exception.ToString()), exception);
                }
            }
            public static T ToObject<T>(string json)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("messagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.ToObject<T>(json);
                }
                catch (Exception exception)
                {
                    throw new ArgumentNullException(Utility.Text.Format("Can not convert to ByteArray with exception '{0}", exception.ToString()), exception);
                }
            }
            public static object ToObject(byte[] buffer, Type objectType)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("messagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.ToObject(buffer, objectType);
                }
                catch (Exception exception)
                {
                    throw new ArgumentNullException(Utility.Text.Format("Can not convert to ByteArray with exception '{0}", exception.ToString()), exception);
                }
            }
            public static object ToObject(string json, Type objectType)
            {
                if (messagePackHelper == null)
                {
                    throw new ArgumentNullException("messagePackHelper is invalid");
                }
                try
                {
                    return messagePackHelper.ToObject(json, objectType);
                }
                catch (Exception exception)
                {
                    throw new ArgumentNullException(Utility.Text.Format("Can not convert to ByteArray with exception '{0}", exception.ToString()), exception);
                }
            }
        }
    }
}
