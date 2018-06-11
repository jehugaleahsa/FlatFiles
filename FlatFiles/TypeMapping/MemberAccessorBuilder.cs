using System;
using System.Linq.Expressions;
using System.Reflection;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    internal static class MemberAccessorBuilder
    {
        public static IMemberAccessor GetMember<TEntity, TProp>(string memberName)
        {
            return GetMember<TEntity>(typeof(TProp), memberName);
        }

        public static IMemberAccessor GetMember<TEntity>(Type propertyType, string memberName)
        {
            string[] memberNames = memberName.Split('.');
            var member = getMember(typeof(TEntity), memberNames, 0, null);
            if (propertyType != null && member.Type != propertyType && member.Type != Nullable.GetUnderlyingType(propertyType))
            {
                throw new ArgumentException(Resources.WrongPropertyType);
            }
            return member;
        }

        public static IMemberAccessor getMember(Type entityType, string[] memberNames, int nameIndex, IMemberAccessor parent)
        {
            if (nameIndex == memberNames.Length)
            {
                return parent;
            }
            string memberName = memberNames[nameIndex];
            var propertyInfo = getProperty(entityType, memberName);
            if (propertyInfo != null)
            {
                var accessor = new PropertyAccessor(propertyInfo, parent);
                return getMember(propertyInfo.PropertyType, memberNames, nameIndex + 1, accessor);
            }
            var fieldInfo = getField(entityType, memberName);
            if (fieldInfo != null)
            {
                var accessor = new FieldAccessor(fieldInfo, parent);
                return getMember(fieldInfo.FieldType, memberNames, nameIndex + 1, accessor);
            }
            throw new ArgumentException(Resources.BadPropertySelector, nameof(memberName));
        }

        private static PropertyInfo getProperty(Type type, string propertyName)
        {
            var bindingFlags = BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            return type.GetTypeInfo().GetProperty(propertyName, bindingFlags);
        }

        private static FieldInfo getField(Type type, string fieldName)
        {
            var bindingFlags = BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            return type.GetTypeInfo().GetField(fieldName, bindingFlags);
        }

        public static IMemberAccessor GetMember<TEntity, TProp>(Expression<Func<TEntity, TProp>> accessor)
        {
            if (accessor == null)
            {
                throw new ArgumentNullException(nameof(accessor));
            }
            return getMember<TEntity>(accessor.Body);
        }

        private static IMemberAccessor getMember<TEntity>(Expression expression)
        {
            MemberExpression member = expression as MemberExpression;
            if (member == null)
            {
                throw new ArgumentException(Resources.BadPropertySelector, nameof(expression));
            }
            if (member.Member is PropertyInfo propertyInfo)
            {
                if (propertyInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
                {
                    return new PropertyAccessor(propertyInfo, null);
                }
                else
                {
                    IMemberAccessor parentAccessor = getMember<TEntity>(member.Expression);
                    return new PropertyAccessor(propertyInfo, parentAccessor);
                }
            }
            else if (member.Member is FieldInfo fieldInfo)
            {
                if (fieldInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(typeof(TEntity)))
                {
                    return new FieldAccessor(fieldInfo, null);
                }
                else
                {
                    IMemberAccessor parentAccessor = getMember<TEntity>(member.Expression);
                    return new FieldAccessor(fieldInfo, parentAccessor);
                }
            }
            else
            {
                throw new ArgumentException(Resources.BadPropertySelector, nameof(expression));
            }
        }
    }
}
