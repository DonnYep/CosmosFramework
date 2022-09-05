using System;

namespace Cosmos.DataNode
{
    public abstract class DataVariable<T> : IDataVariable
    {
        protected T value;
        public T Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
        /// <inheritdoc/>
        public virtual Type Type
        {
            get { return typeof(T); }
        }
        /// <inheritdoc/>
        public virtual object GetValue()
        {
            return value;
        }
        /// <inheritdoc/>
        public virtual void SetValue(object value)
        {
            this.value = (T)value;
        }
        public virtual void Release()
        {
            value = default(T);
        }
        public override string ToString()
        {
            return (value != null) ? value.ToString() : "<NULL>";
        }
    }
}
