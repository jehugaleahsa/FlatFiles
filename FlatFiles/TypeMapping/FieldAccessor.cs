using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    internal sealed class FieldAccessor : IMemberAccessor
    {
        private readonly FieldInfo fieldInfo;

        public FieldAccessor(FieldInfo fieldInfo, IMemberAccessor? parent)
        {
            this.fieldInfo = fieldInfo;
            ParentAccessor = parent;
        }

        public MemberInfo MemberInfo => fieldInfo;

        public IMemberAccessor? ParentAccessor { get; }

        public string Name => ParentAccessor == null ? fieldInfo.Name : $"{ParentAccessor.Name}.{fieldInfo.Name}";

        public Type Type => fieldInfo.FieldType;

        public object? GetValue(object instance)
        {
            return fieldInfo.GetValue(instance);
        }

        public void SetValue(object instance, object? value)
        {
            fieldInfo.SetValue(instance, value);
        }
    }
}
