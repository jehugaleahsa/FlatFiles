using System.Reflection;

namespace FlatFiles.TypeMapping
{
    internal interface IMemberAccessor
    {
        MemberInfo MemberInfo { get; }

        string Name { get; }

        object GetValue(object instance);

        void SetValue(object instance, object value);
    }

    internal class FieldAccessor : IMemberAccessor
    {
        private readonly FieldInfo fieldInfo;

        public FieldAccessor(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
        }

        public MemberInfo MemberInfo
        {
            get { return fieldInfo; }
        }

        public string Name
        {
            get { return fieldInfo.Name; }
        }

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

        public PropertyAccessor(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;
        }

        public MemberInfo MemberInfo
        {
            get { return propertyInfo; }
        }

        public string Name
        {
            get { return propertyInfo.Name; }
        }

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
