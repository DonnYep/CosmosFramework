using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Cosmos.DataTable
{
    /// <summary>
    /// 反射绑定计划：把「列名 → 成员（字段/属性）」的解析只做一次，之后每行只做取值 + 赋值。
    /// <para>旧示例是**每行**调用一次反射（GetFields + 构建字典 + SetFieldValue），
    /// 一万行 × 六列就是六万次反射；这里只在读到表头时反射一次。</para>
    /// <para>想要零反射：让行实现 <see cref="IRawDataRow"/>，解析器会走行自己的实现。</para>
    /// </summary>
    /// <typeparam name="T">行类型</typeparam>
    internal sealed class DataRowBinder<T> where T : class, IDataRow, new()
    {
        sealed class ColumnBinding
        {
            public MemberInfo Member;
            public Type MemberType;
            public int ColumnIndex;
        }
        readonly ColumnBinding[] bindings;
        readonly string missingIdError;
        DataRowBinder(ColumnBinding[] bindings, string missingIdError)
        {
            this.bindings = bindings;
            this.missingIdError = missingIdError;
        }
        /// <summary>
        /// 按表头构建绑定计划
        /// </summary>
        /// <param name="header">列名 → 列序号</param>
        /// <param name="idColumnName">主键列名</param>
        /// <returns>绑定计划</returns>
        public static DataRowBinder<T> Create(IReadOnlyDictionary<string, int> header, string idColumnName)
        {
            var members = CollectMembers();
            var bindingList = new List<ColumnBinding>();
            var hasId = false;
            foreach (var pair in header)
            {
                MemberInfo member;
                if (!members.TryGetValue(pair.Key, out member))
                    continue;   //表里多出来的列直接忽略（允许表结构演进）
                if (member.Name == idColumnName)
                    hasId = true;
                bindingList.Add(new ColumnBinding()
                {
                    Member = member,
                    MemberType = GetMemberType(member),
                    ColumnIndex = pair.Value,
                });
            }
            var missing = hasId ? null : $"表头缺少主键列 \"{idColumnName}\"（或行类型没有可写的 {idColumnName} 成员）";
            return new DataRowBinder<T>(bindingList.ToArray(), missing);
        }
        /// <summary>
        /// 用一行字段填充行对象
        /// </summary>
        /// <param name="row">行对象</param>
        /// <param name="fields">字段</param>
        public void Bind(T row, string[] fields)
        {
            if (missingIdError != null)
                throw new InvalidOperationException(missingIdError);
            for (int i = 0; i < bindings.Length; i++)
            {
                var binding = bindings[i];
                var raw = binding.ColumnIndex < fields.Length ? fields[binding.ColumnIndex] : null;
                var value = ConvertValue(binding.MemberType, raw);
                var field = binding.Member as FieldInfo;
                if (field != null)
                    field.SetValue(row, value);
                else
                    ((PropertyInfo)binding.Member).SetValue(row, value, null);
            }
        }
        static Dictionary<string, MemberInfo> CollectMembers()
        {
            var members = new Dictionary<string, MemberInfo>(StringComparer.Ordinal);
            var type = typeof(T);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; i++)
                members[fields[i].Name] = fields[i];
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                if (property.CanWrite && property.GetSetMethod() != null)
                    members[property.Name] = property;
            }
            return members;
        }
        static Type GetMemberType(MemberInfo member)
        {
            var field = member as FieldInfo;
            return field != null ? field.FieldType : ((PropertyInfo)member).PropertyType;
        }
        /// <summary>
        /// 字符串 → 成员类型；空串取默认值，非法值抛 FormatException（由解析器补行号/列名）
        /// </summary>
        public static object ConvertValue(Type type, string raw)
        {
            if (type == typeof(string))
                return raw ?? string.Empty;
            var text = raw == null ? null : raw.Trim();
            if (string.IsNullOrEmpty(text))
                return type.IsValueType ? Activator.CreateInstance(type) : null;
            var culture = CultureInfo.InvariantCulture;
            if (type == typeof(int))
                return int.Parse(text, NumberStyles.Integer, culture);
            if (type == typeof(long))
                return long.Parse(text, NumberStyles.Integer, culture);
            if (type == typeof(short))
                return short.Parse(text, NumberStyles.Integer, culture);
            if (type == typeof(byte))
                return byte.Parse(text, NumberStyles.Integer, culture);
            if (type == typeof(uint))
                return uint.Parse(text, NumberStyles.Integer, culture);
            if (type == typeof(ulong))
                return ulong.Parse(text, NumberStyles.Integer, culture);
            if (type == typeof(float))
                return float.Parse(text, NumberStyles.Float, culture);
            if (type == typeof(double))
                return double.Parse(text, NumberStyles.Float, culture);
            if (type == typeof(decimal))
                return decimal.Parse(text, NumberStyles.Number, culture);
            if (type == typeof(bool))
            {
                if (text == "1") return true;
                if (text == "0") return false;
                return bool.Parse(text);
            }
            if (type.IsEnum)
            {
                int numeric;
                if (int.TryParse(text, NumberStyles.Integer, culture, out numeric))
                    return Enum.ToObject(type, numeric);
                return Enum.Parse(type, text, true);
            }
            return Convert.ChangeType(text, type, culture);
        }
    }
}
