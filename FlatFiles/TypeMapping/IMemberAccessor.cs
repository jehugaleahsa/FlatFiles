using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    internal interface IMemberAccessor
    {
        MemberInfo MemberInfo { get; }

        IMemberAccessor ParentAccessor { get; }

        string Name { get; }

        Type Type { get; }

        object GetValue(object instance);

        void SetValue(object instance, object value);
    }

    internal class FieldAccessor : IMemberAccessor
    {
        private readonly FieldInfo fieldInfo;

        public FieldAccessor(FieldInfo fieldInfo, IMemberAccessor parent)
        {
            this.fieldInfo = fieldInfo;
            ParentAccessor = parent;
        }

        public MemberInfo MemberInfo => fieldInfo;

        public IMemberAccessor ParentAccessor { get; private set;  }

        public string Name => ParentAccessor == null ? fieldInfo.Name : $"{ParentAccessor.Name}.{fieldInfo.Name}";

        public Type Type => fieldInfo.FieldType;

        public object GetValue(object instance)
        {
            return fieldInfo.GetValue(instance);
        }

        public void SetValue(object instance, object value)
        {
            fieldInfo.SetValue(instance, value);
        }
    }

    internal class PropertyAccessor : IMemberAccessor
    {
        private readonly PropertyInfo propertyInfo;

        public PropertyAccessor(PropertyInfo propertyInfo, IMemberAccessor parent)
        {
            this.propertyInfo = propertyInfo;
            ParentAccessor = parent;
        }

        public MemberInfo MemberInfo => propertyInfo;

        public IMemberAccessor ParentAccessor { get; private set; }

        public string Name => ParentAccessor == null ? propertyInfo.Name : $"{ParentAccessor.Name}.{propertyInfo.Name}";

        public Type Type => propertyInfo.PropertyType;

        public object GetValue(object instance)
        {
            return propertyInfo.GetValue(instance);
        }

        public void SetValue(object instance, object value)
        {
            propertyInfo.SetValue(instance, value);
        }
    }
}
