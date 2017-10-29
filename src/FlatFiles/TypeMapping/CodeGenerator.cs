using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FlatFiles.Resources;

namespace FlatFiles.TypeMapping
{
    internal interface ICodeGenerator
    {
        Func<object> GetFactory(Type entityType);

        Action<TEntity, object[]> GetReader<TEntity>(List<IMemberMapping> mappings);

        Func<TEntity, object[]> GetWriter<TEntity>(List<IMemberMapping> mappings);
    }

    internal sealed class ReflectionCodeGenerator : ICodeGenerator
    {
        public Func<object> GetFactory(Type entityType)
        {
            return () => Activator.CreateInstance(entityType);
        }

        public Action<TEntity, object[]> GetReader<TEntity>(List<IMemberMapping> mappings)
        {
            Action<TEntity, object[]> reader = (entity, values) =>
            {
                for (int index = 0, position = 0; index != mappings.Count; ++index)
                {
                    IMemberMapping mapping = mappings[index];
                    if (mapping.Member != null)
                    {
                        object value = values[position];
                        mapping.Member.SetValue(entity, value);
                        ++position;
                    }
                }
            };
            return reader;
        }

        public Func<TEntity, object[]> GetWriter<TEntity>(List<IMemberMapping> mappings)
        {
            Func<TEntity, object[]> writer = (entity) =>
            {
                List<object> values = new List<object>();
                for (int index = 0; index != mappings.Count; ++index)
                {
                    IMemberMapping mapping = mappings[index];
                    if (mapping.Member != null)
                    {
                        object value = mapping.Member.GetValue(entity);
                        values.Add(value);
                    }
                }
                return values.ToArray();
            };
            return writer;
        }
    }

    internal sealed class EmitCodeGenerator : ICodeGenerator
    {
        public Func<object> GetFactory(Type entityType)
        {
            var constructorInfo = entityType.GetTypeInfo().GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                throw new FlatFileException(SharedResources.NoDefaultConstructor);
            }
            DynamicMethod method = new DynamicMethod("", entityType, Type.EmptyTypes);
            var generator = method.GetILGenerator();
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            return (Func<object>)method.CreateDelegate(typeof(Func<object>));
        }

        public Action<TEntity, object[]> GetReader<TEntity>(List<IMemberMapping> mappings)
        {
            Type entityType = typeof(TEntity);
            DynamicMethod method = new DynamicMethod(
                "__FlatFiles_TypeMapping_read",
                typeof(void),
                new Type[] { entityType, typeof(object[]) },
                true);
            var generator = method.GetILGenerator();
            int position = 0;
            for (int index = 0; index != mappings.Count; ++index)
            {
                IMemberMapping mapping = mappings[index];
                if (mapping.Member == null)
                {
                    continue;
                }
                generator.Emit(OpCodes.Ldarg, 0);
                generator.Emit(OpCodes.Ldarg, 1);
                generator.Emit(OpCodes.Ldc_I4, position);
                generator.Emit(OpCodes.Ldelem_Ref);

                if (mapping.Member.MemberInfo is FieldInfo fieldInfo)
                {
                    Type fieldType = fieldInfo.FieldType;
                    generator.Emit(OpCodes.Unbox_Any, fieldType);
                    generator.Emit(OpCodes.Stfld, fieldInfo);
                }
                else if (mapping.Member.MemberInfo is PropertyInfo propertyInfo)
                {
                    MethodInfo setter = propertyInfo.GetSetMethod();
                    if (setter == null)
                    {
                        string message = String.Format(null, SharedResources.ReadOnlyProperty, propertyInfo.Name);
                        throw new FlatFileException(message);
                    }
                    Type propertyType = propertyInfo.PropertyType;
                    generator.Emit(OpCodes.Unbox_Any, propertyType);
                    generator.Emit(OpCodes.Callvirt, setter);
                }

                ++position;
            }

            generator.Emit(OpCodes.Ret);

            var result = (Action<TEntity, object[]>)method.CreateDelegate(typeof(Action<TEntity, object[]>));
            return result;
        }

        public Func<TEntity, object[]> GetWriter<TEntity>(List<IMemberMapping> mappings)
        {
            Type entityType = typeof(TEntity);
            DynamicMethod method = new DynamicMethod(
                "__FlatFiles_TypeMapping_write",
                typeof(object[]),
                new Type[] { entityType },
                true);
            var remaining = mappings.Where(m => m.Member != null).ToArray();
            var generator = method.GetILGenerator();
            generator.DeclareLocal(typeof(object[]));
            generator.Emit(OpCodes.Ldc_I4, remaining.Length);
            generator.Emit(OpCodes.Newarr, typeof(object));
            generator.Emit(OpCodes.Stloc_0);

            for (int index = 0; index != remaining.Length; ++index)
            {
                IMemberMapping mapping = remaining[index];

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldc_I4, index);

                if (mapping.Member.MemberInfo is FieldInfo fieldInfo)
                {
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, fieldInfo);
                    Type fieldType = fieldInfo.FieldType;
                    if (!fieldType.GetTypeInfo().IsClass)
                    {
                        generator.Emit(OpCodes.Box, fieldType);
                    }
                }
                else if (mapping.Member.MemberInfo is PropertyInfo propertyInfo)
                {
                    MethodInfo getter = propertyInfo.GetGetMethod();
                    if (getter == null)
                    {
                        string message = String.Format(null, SharedResources.WriteOnlyProperty, propertyInfo.Name);
                        throw new FlatFileException(message);
                    }
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Callvirt, getter);
                    Type propertyType = propertyInfo.PropertyType;
                    if (!propertyType.GetTypeInfo().IsClass)
                    {
                        generator.Emit(OpCodes.Box, propertyType);
                    }
                }

                generator.Emit(OpCodes.Stelem_Ref);
            }

            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ret);

            var result = (Func<TEntity, object[]>)method.CreateDelegate(typeof(Func<TEntity, object[]>));
            return result;
        }
    }
}
