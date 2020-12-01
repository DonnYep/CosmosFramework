using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace Cosmos
{
    public sealed partial class Utility
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
                    throw new ArgumentNullException("Type is invalid");
                }
                string typeName = type.FullName;
                return string.IsNullOrEmpty(name) ? typeName : Utility.Text.Format(typeName, name);
            }
            /// <summary>
            /// 反射工具，得到反射类的对象；
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
                    var obj = assembly.CreateInstance(typeFullName) as T;
                    return obj;
                }
                else
                {
                    throw new ArgumentNullException("Type : Assembly" + type.AssemblyQualifiedName + "Not exist!");
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
                where T : class, new()
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
            public static T GetTypeInstance<T>(string typeFullName)
                where T : class, new()
            {
                return GetTypeInstance<T>(typeof(T).Assembly, typeFullName);
            }
            /// <summary>
            /// 反射工具，得到反射类的对象；
            /// 不可反射Mono子类，被反射对象必须是具有无参公共构造
            /// 在IOS上受限，发布IOS需要谨慎
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>装箱后的对象</returns>
            public static object GetTypeInstance(Type type)
            {
                return type.Assembly.CreateInstance(type.FullName);
            }
            /// <summary>
            /// 反射工具，得到反射类的对象；
            /// 不可反射Mono子类，被反射对象必须是具有无参公共构造
            /// 在IOS上受限，发布IOS需要谨慎
            /// </summary>
            /// <param name="type">类型</param>
            /// <returns>装箱后的对象</returns>
            public static T GetTypeInstance<T>(Type type)
                where T : class, new()
            {
                return type.Assembly.CreateInstance(type.FullName) as T;
            }
            /// <summary>
            /// 获取所有派生类的实例对象
            /// </summary>
            /// <typeparam name="T">目标类型</typeparam>
            /// <param name="type">类型</param>
            /// <returns>反射生成后的对象数组</returns>
            public static T[] GetDerivedTypeInstances<T>()
                where T : class
            {
                Type type = typeof(T);
                List<T> list = new List<T>();
                var types = GetDerivedTypes(type);
                var length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    var obj = GetTypeInstance(types[i]) as T;
                    list.Add(obj);
                }
                return list.ToArray();
            }
            public static object[] GetDerivedTypeInstances(Type type)
            {
                List<object> list = new List<object>();
                var types = GetDerivedTypes(type);
                var length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    var obj = GetTypeInstance(types[i]);
                    list.Add(obj);
                }
                return list.ToArray();
            }
            /// <summary>
            /// 获取变量的名称；
            /// 参考：
            /// object dotNet=new object();
            /// Utility.Assembly.GetValueTypeName(() =>dotNet);
            /// 输出得 dotNet
            /// </summary>
            /// <typeparam name="T">任意类型的变量</typeparam>
            /// <param name="memberExperssion">变量的表达式</param>
            /// <returns>传入变量的名称</returns>
            public static string GetGetPropertyNameName<T>(Expression<Func<T>> memberExperssion)
            {
                MemberExpression me = (MemberExpression)memberExperssion.Body;
                return me.Member.Name;
            }
            /// <summary>
            /// 通过特性获取对象实体；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <param name="type">基类</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象</returns>
            public static object GetInstanceByAttribute<T>(Type type, bool inherit = false)
                where T : Attribute
            {
                object obj = default;
                var types = GetDerivedTypes(type);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes(typeof(T), inherit).Length > 0)
                    {
                        obj = GetTypeInstance(types[i]);
                        return obj;
                    }
                }
                return obj;
            }
            /// <summary>
            /// 通过特性获取对象实体；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <param name="type">基类</param>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象</returns>
            public static object GetInstanceByAttribute<T>(Type type, System.Reflection.Assembly assembly, bool inherit = false)
    where T : Attribute
            {
                object obj = default;
                var types = GetDerivedTypes(type, assembly);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes(typeof(T), inherit).Length > 0)
                    {
                        obj = GetTypeInstance(types[i]);
                        return obj;
                    }
                }
                return obj;
            }
            /// <summary>
            /// 通过特性获取对象实体；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <typeparam name="K">基类，new()约束</typeparam>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象</returns>
            public static K GetInstanceByAttribute<T, K>(bool inherit = false)
                where T : Attribute
                where K : class
            {
                K obj = default;
                var types = GetDerivedTypes(typeof(K));
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes(typeof(T), inherit).Length > 0)
                    {
                        obj = GetTypeInstance(types[i]) as K;
                        return obj;
                    }
                }
                return obj;
            }
            /// <summary>
            /// 通过特性获取对象实体；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <typeparam name="K">基类，new()约束</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象</returns>
            public static K GetInstanceByAttribute<T, K>(System.Reflection.Assembly assembly, bool inherit = false)
      where T : Attribute
      where K : class
            {
                K obj = default;
                var types = GetDerivedTypes(typeof(K), assembly);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes(typeof(T), inherit).Length > 0)
                    {
                        obj = GetTypeInstance(types[i]) as K;
                        return obj;
                    }
                }
                return obj;
            }
            /// <summary>
            /// 通过特性获取对象实体数组；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <param name="type">基类</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象数组</returns>
            public static object[] GetInstancesByAttribute<T>(Type type, bool inherit = false)
                where T : Attribute
            {
                List<object> set = new List<object>();
                var types = GetDerivedTypes(type);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes(typeof(T), inherit).Length > 0)
                    {
                        set.Add(GetTypeInstance(types[i]));
                    }
                }
                return set.ToArray();
            }
            /// <summary>
            /// 通过特性获取对象实体数组；
            /// 生成的对象必须是无参可构造；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <typeparam name="K">基类，new()约束</typeparam>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象数组</returns>
            public static K[] GetInstancesByAttribute<T, K>(bool inherit = false)
                where T : Attribute
                where K : class
            {
                List<K> set = new List<K>();
                var types = GetDerivedTypes(typeof(K));
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes<T>(inherit).Count() > 0)
                    {
                        set.Add(GetTypeInstance(types[i]) as K);
                    }
                }
                return set.ToArray();
            }
            /// <summary>
            /// 通过特性获取对象实体数组；
            /// 生成的对象必须是无参可构造；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <typeparam name="K">基类，new()约束</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns>生成的对象数组</returns>
            public static K[] GetInstancesByAttribute<T, K>(System.Reflection.Assembly assembly, bool inherit = false)
    where T : Attribute
    where K : class
            {
                List<K> set = new List<K>();
                var types = GetDerivedTypes(typeof(K), assembly);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes<T>(inherit).Count() > 0)
                    {
                        set.Add(GetTypeInstance(types[i]) as K);
                    }
                }
                return set.ToArray();
            }
            /// <summary>
            /// 通过特性获取对象实体数组；
            /// 生成的对象必须是无参可构造；
            /// </summary>
            /// <typeparam name="T">目标特性</typeparam>
            /// <param name="type">基类，new()约束</param>
            /// <param name="assembly">查询的程序集</param>
            /// <param name="inherit">是否检查基类特性</param>
            /// <returns></returns>
            public static object[] GetInstancesByAttribute<T>(Type type, System.Reflection.Assembly assembly, bool inherit = false)
    where T : Attribute
            {
                List<object> set = new List<object>();
                var types = GetDerivedTypes(type);
                int length = types.Length;
                for (int i = 0; i < length; i++)
                {
                    if (types[i].GetCustomAttributes(typeof(T), inherit).Length > 0)
                    {
                        set.Add(GetTypeInstance(types[i]));
                    }
                }
                return set.ToArray();
            }
            /// <summary>
            /// 获取某类型的所有派生类数组；
            /// </summary>
            /// <param name="type">基类</param>
            /// <returns>非抽象派生类</returns>
            public static Type[] GetDerivedTypes(Type type)
            {
                List<Type> set = new List<Type>();
                Type[] types = type.Assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (type.IsAssignableFrom(types[i]))
                    {
                        if (types[i].IsClass && !types[i].IsAbstract)
                        {
                            set.Add(types[i]);
                        }
                    }
                }
                return set.ToArray();
            }

            /// <summary>
            /// 获取某类型的所有派生类数组；
            /// </summary>
            /// <typeparam name="T">基类</typeparam>
            /// <returns>非抽象派生类</returns>
            public static Type[] GetDerivedTypes<T>()
                where T : class
            {
                Type type = typeof(T);
                List<Type> set = new List<Type>();
                Type[] types = type.Assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (type.IsAssignableFrom(types[i]))
                    {
                        if (types[i].IsClass && !types[i].IsAbstract)
                        {
                            set.Add(types[i]);
                        }
                    }
                }
                return set.ToArray();
            }
            /// <summary>
            /// 获取某类型在指定程序集的所有派生类数组；
            /// </summary>
            /// <typeparam name="T">基类</typeparam>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类</returns>
            public static Type[] GetDerivedTypes<T>(System.Reflection.Assembly assembly)
    where T : class
            {
                Type type = typeof(T);
                List<Type> set = new List<Type>();
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (type.IsAssignableFrom(types[i]))
                    {
                        if (types[i].IsClass && !types[i].IsAbstract)
                        {
                            set.Add(types[i]);
                        }
                    }
                }
                return set.ToArray();
            }
            /// <summary>
            /// 获取某类型在指定程序集的所有派生类数组；
            /// </summary>
            /// <param name="type">基类</param>
            /// <param name="assembly">查询的程序集</param>
            /// <returns>非抽象派生类</returns>
            public static Type[] GetDerivedTypes(Type type, System.Reflection.Assembly assembly)
            {
                List<Type> set = new List<Type>();
                Type[] types = assembly.GetTypes();
                for (int i = 0; i < types.Length; i++)
                {
                    if (type.IsAssignableFrom(types[i]))
                    {
                        if (types[i].IsClass && !types[i].IsAbstract)
                        {
                            set.Add(types[i]);
                        }
                    }
                }
                return set.ToArray();
            }
            /// <summary>
            /// 将一个对象上的字段值赋予到另一个对象上名字相同的字段上；
            /// 此方法可识别属性与字段，赋值时尽量将属性的索引字段也进行命名统一；
            /// </summary>
            /// <typeparam name="T">需要赋值的源类型</typeparam>
            /// <typeparam name="K">目标类型</typeparam>
            /// <param name="source">源对象</param>
            /// <param name="target">目标对象</param>
            /// <returns>被赋值后的目标对象</returns>
            public static K AssignSameFieldValue<T, K>(T source, K target)
        where T : class
        where K : class
            {
                Type srcType = typeof(T);
                Type tgType = typeof(K);
                var srcFields = srcType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                var tgFields = tgType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                var length = srcFields.Length;
                for (int i = 0; i < length; i++)
                {
                    var tgLen = tgFields.Length;
                    for (int j = 0; j < tgLen; j++)
                    {
                        if (srcFields[i].Name == tgFields[j].Name)
                        {
                            try
                            {
                                tgFields[j].SetValue(target, srcFields[i].GetValue(source));
                            }
                            catch { }
                        }
                    }
                }
                return target;
            }
            /// <summary>
            /// 遍历实例对象上的所有字段；
            /// 此方法可识别属性与字段，打印属性时候需要特别注意过滤自动属性的额外字段；
            /// </summary>
            /// <typeparam name="T">实例对象类型</typeparam>
            /// <param name="obj">实例对象</param>
            /// <returns>每个字段与字段值的数组</returns>
            public static string[] TraverseInstanceAllFiled<T>(T obj)
            {
                if (obj == null)
                    return null;
                Type type = typeof(T);
                var fields = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
                var length = fields.Length;
                List<string> filedSet = new List<string>();
                for (int i = 0; i < length; i++)
                {
                    var info = $"{fields[i].Name}:{fields[i].GetValue(obj)};";
                    filedSet.Add(info);
                }
                return filedSet.ToArray();
            }
        }
    }
}
