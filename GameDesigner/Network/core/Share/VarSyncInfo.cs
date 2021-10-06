using System;
using System.Reflection;

namespace Net.Share
{
    public class VarSyncInfo
    {
        internal object value;
        internal ushort id;
        internal Type type;
        internal bool passive;
        public virtual object GetValue()
        {
            return null;
        }
        public virtual void SetValue(object value)
        {
        }
    }
    public class VarSyncFieldInfo : VarSyncInfo
    {
        public object target;
        public FieldInfo fieldInfo;
        public override object GetValue()
        {
            return fieldInfo.GetValue(target);
        }
        public override void SetValue(object value)
        {
            fieldInfo.SetValue(target, value);
        }
    }
    public class VarSyncPropertyInfo : VarSyncInfo
    {
        public object target;
        public PropertyInfo propertyInfo;
        public override object GetValue()
        {
            return propertyInfo.GetValue(target);
        }
        public override void SetValue(object value)
        {
            propertyInfo.SetValue(target, value);
        }
    }
}
