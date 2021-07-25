using System;
using System.Reflection;

namespace FlatFiles.TypeMapping
{
    internal interface IMemberAccessor
    {
        MemberInfo MemberInfo { get; }

        IMemberAccessor? ParentAccessor { get; }

        string Name { get; }

        Type Type { get; }

        object? GetValue(object instance);

        void SetValue(object instance, object? value);
    }
}
