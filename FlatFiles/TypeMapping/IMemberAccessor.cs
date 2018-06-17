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
        private readonly FieldInfo _fieldInfo;

        public FieldAccessor(FieldInfo fieldInfo, IMemberAccessor parent)
        {
            _fieldInfo = fieldInfo;
            ParentAccessor = parent;
        }

        public MemberInfo MemberInfo => _fieldInfo;

        public IMemberAccessor ParentAccessor { get; }

        public string Name => ParentAccessor == null ? _fieldInfo.Name : $"{ParentAccessor.Name}.{_fieldInfo.Name}";

        public Type Type => _fieldInfo.FieldType;

        public object GetValue(object instance)
        {
            return _fieldInfo.GetValue(instance);
        }

        public void SetValue(object instance, object value)
        {
            _fieldInfo.SetValue(instance, value);
        }
    }

    internal class PropertyAccessor : IMemberAccessor
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyAccessor(PropertyInfo propertyInfo, IMemberAccessor parent)
        {
            _propertyInfo = propertyInfo;
            ParentAccessor = parent;
        }

        public MemberInfo MemberInfo => _propertyInfo;

        public IMemberAccessor ParentAccessor { get; }

        public string Name => ParentAccessor == null ? _propertyInfo.Name : $"{ParentAccessor.Name}.{_propertyInfo.Name}";

        public Type Type => _propertyInfo.PropertyType;

        public object GetValue(object instance)
        {
            return _propertyInfo.GetValue(instance);
        }

        public void SetValue(object instance, object value)
        {
            _propertyInfo.SetValue(instance, value);
        }
    }
}
