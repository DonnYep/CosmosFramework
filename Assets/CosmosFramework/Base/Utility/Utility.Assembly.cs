using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cosmos
{
    public  static partial class Utility
    {
        public static class Assembly
        {
            public static string GetTypeFullName<T>()
            {
                return typeof(T).ToString();
            }
            public static string GetTypeFullName<T>(T arg)
            {
                return arg.ToString();
            }
            public static string GetTypeFullName<T>(string name)
                where T : class
            {
                return GetTypeFullName(typeof(T), name);
            }
            public static string GetTypeFullName(Type type, string name)
            {
                if (type == null)
                {
                    DebugError("Type is invalid.无效类");
                    return null;
                }
                string typeName = type.FullName;
                return string.IsNullOrEmpty(name) ? typeName : Utility.Text.Format(typeName, name);
            }
            /// <summary>
            ///   /// 反射工具，得到反射类的对象；
            /// 不可反射Mono子类，被反射对象必须是具有无参公共构造
            /// 在IOS上受限，发布IOS需要谨慎
            /// </summary>
            /// <typeparam name="T">类型目标</typeparam>
            /// <param name="assembly">目标程序集</param>
            /// <param name="typeFullName">完全限定名</param>
            /// <returns>返回T类型的目标类型对象</returns>
            public static T GetTypeInstance<T>(System.Reflection.Assembly assembly, string typeFullName)
     where T : class
            {
                Type type = assembly.GetType(typeFullName);
                if (type != null)
                {
                    //        var obj = Activator.CreateInstance(type) as T;
                    //TODO反射失败，需要解决
                    var obj = assembly.CreateInstance(typeFullName) as T;
                    return obj;
                }
                else
                {
                    throw new CFrameworkException("Type : Assembly" + type.AssemblyQualifiedName + "Not exist!");
                }
            }
            /// <summary>
            ///   /// 反射工具，得到反射类的对象；
            /// 不可反射Mono子类，被反射对象必须是具有无参公共构造
            /// 在IOS上受限，发布IOS需要谨慎
            /// </summary>
            /// <typeparam name="T">类型目标</typeparam>
            /// <param name="type">目标对象程序集中的某个对象类型:GetType()</param>
            /// <param name="typeFullName">完全限定名</param>
            /// <returns>返回T类型的目标类型对象</returns>
            public static T GetTypeInstance<T>(Type type, string typeFullName)
                where T : class
            {
                return GetTypeInstance<T>(type.Assembly, typeFullName);
            }
            /// <summary>
            /// 反射工具，得到反射类的对象；
            /// 不可反射Mono子类，被反射对象必须是具有无参公共构造
            /// 在IOS上受限，发布IOS需要谨慎
            /// <typeparam name="T">目标类型</typeparam>
            /// <param name="arg">目标类型的对象</param>
            /// <param name="typeFullName">完全限定名</param>
            /// <returns>返回T类型的目标类型对象</returns>
            public static T GetTypeInstance<T>(T arg, string typeFullName)
                where T : class
            {
                return GetTypeInstance<T>(typeof(T).Assembly, typeFullName);
            }
        }
    }
}
