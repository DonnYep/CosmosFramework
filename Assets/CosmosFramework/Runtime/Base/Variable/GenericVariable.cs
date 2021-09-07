using System;
using System.Collections;
using System.Collections.Generic;
namespace Cosmos {
    /// <summary>
    ///泛型变量；
    /// </summary>
    public abstract class Variable <T>:Variable
    {
        T value;
        protected Variable(){value = default(T);}
        protected Variable(T var){value = var;}
        public override Type Type { get { return typeof(T); } }
        public T Value { get { return this.value; } set { this.value = value; } }
        public override void SetValue(object value)
        {
            this.value = (T)value;
        }
        public override object GetValue()
        {
            return value;
        }
        public override void Release()
        {
            value = default(T);
        }
        public override string ToString()
        {
            return (value != null) ? value.ToString() : "<Null>";
        }
    }
}