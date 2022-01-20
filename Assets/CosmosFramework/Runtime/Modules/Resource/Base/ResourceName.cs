﻿using System;

namespace Cosmos
{
    /// <summary>
    /// 资源名称。
    /// </summary>
    public struct ResourceName : IComparable, IComparable<ResourceName>, IEquatable<ResourceName>
    {
        private readonly string m_Name;
        private readonly string m_Variant;
        private readonly string m_Extension;
        private string m_CachedFullName;
        /// <summary>
        /// 获取资源名称。
        /// </summary>
        public string Name { get { return m_Name; } }
        /// <summary>
        /// 获取变体名称。
        /// </summary>
        public string Variant { get { return m_Variant; } }
        /// <summary>
        /// 获取扩展名称。
        /// </summary>
        public string Extension { get { return m_Extension; } }
        /// <summary>
        /// 初始化资源名称的新实例。
        /// </summary>
        /// <param name="name">资源名称。</param>
        /// <param name="variant">变体名称。</param>
        /// <param name="extension">扩展名称。</param>
        public ResourceName(string name, string variant, string extension)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("Resource name is invalid.");
            }

            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentNullException("Resource extension is invalid.");
            }
            m_Name = name;
            m_Variant = variant;
            m_Extension = extension;
            m_CachedFullName = null;
        }
        public string FullName
        {
            get
            {
                if (m_CachedFullName == null)
                {
                    m_CachedFullName = m_Variant != null ? Utility.Text.Format($"{m_Name}.{m_Variant}.{m_Extension}") : Utility.Text.Format($"{m_Name}.{m_Extension}");
                }
                return m_CachedFullName;
            }
        }
        public override string ToString()
        {
            return FullName;
        }
        public override int GetHashCode()
        {
            if (m_Variant == null)
            {
                return m_Name.GetHashCode() ^ m_Extension.GetHashCode();
            }
            return m_Name.GetHashCode() ^ m_Variant.GetHashCode() ^ m_Extension.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return (obj is ResourceName) && Equals((ResourceName)obj);
        }
        public bool Equals(ResourceName value)
        {
            return string.Equals(m_Name, value.m_Name, StringComparison.Ordinal) && string.Equals(m_Variant, value.m_Variant, StringComparison.Ordinal) && string.Equals(m_Extension, value.m_Extension, StringComparison.Ordinal);
        }

        public static bool operator ==(ResourceName a, ResourceName b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(ResourceName a, ResourceName b)
        {
            return !(a == b);
        }
        public int CompareTo(object value)
        {
            if (value == null)
            {
                return 1;
            }
            if (!(value is ResourceName))
            {
                throw new NotImplementedException("Type of value is invalid.");
            }
            return CompareTo((ResourceName)value);
        }
        public int CompareTo(ResourceName resourceName)
        {
            int result = string.CompareOrdinal(m_Name, resourceName.m_Name);
            if (result != 0)
            {
                return result;
            }
            result = string.CompareOrdinal(m_Variant, resourceName.m_Variant);
            if (result != 0)
            {
                return result;
            }
            return string.CompareOrdinal(m_Extension, resourceName.m_Extension);
        }
    }
}
