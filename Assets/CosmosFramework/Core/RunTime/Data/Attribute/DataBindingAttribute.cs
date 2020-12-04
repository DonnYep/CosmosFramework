using System;
namespace Cosmos
{
    /// <summary>
    ///  数据绑定标记 
    /// </summary>
    [AttributeUsage(AttributeTargets.Field|AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DataBindingAttribute : Attribute
    {
        public string Target { get; private set; }
        public DataBindingAttribute(string target)
        {
            Target = target;
        }
    }
}