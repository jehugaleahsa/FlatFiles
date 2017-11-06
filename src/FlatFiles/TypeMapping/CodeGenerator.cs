using System;
using System.Reflection;
using System.Reflection.Emit;
using FlatFiles.Resources;

namespace FlatFiles.TypeMapping
{
    internal interface ICodeGenerator
    {
        Func<TEntity> GetFactory<TEntity>();

        Action<TEntity, object[]> GetReader<TEntity>(IMemberMapping[] mappings);

        Action<TEntity, object[]> GetWriter<TEntity>(IMemberMapping[] mappings);
    }

    internal sealed class ReflectionCodeGenerator : ICodeGenerator
    {
        public Func<TEntity> GetFactory<TEntity>()
        {
            return Activator.CreateInstance<TEntity>;
        }

        public Action<TEntity, object[]> GetReader<TEntity>(IMemberMapping[] mappings)
        {
            Action<TEntity, object[]> reader = (entity, values) =>
            {
                for (int index = 0; index != mappings.Length; ++index)
                {
                    IMemberMapping mapping = mappings[index];
                    if (mapping.Member != null)
                    {
                        object value = values[mapping.WorkIndex];
                        mapping.Member.SetValue(entity, value);
                    }
                }
            };
            return reader;
        }

        public Action<TEntity, object[]> GetWriter<TEntity>(IMemberMapping[] mappings)
        {
            Action<TEntity, object[]> writer = (entity, values) =>
            {
                for (int index = 0; index != mappings.Length; ++index)
                {
                    IMemberMapping mapping = mappings[index];
                    if (mapping.Member != null)
                    {
                        object value = mapping.Member.GetValue(entity);
                        values[mapping.WorkIndex] = value;
                    }
                }
            };
            return writer;
        }
    }

    internal sealed class EmitCodeGenerator : ICodeGenerator
    {
        public Func<TEntity> GetFactory<TEntity>()
        {
            Type entityType = typeof(TEntity);
            var constructorInfo = entityType.GetTypeInfo().GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                throw new FlatFileException(SharedResources.NoDefaultConstructor);
            }
            DynamicMethod method = new DynamicMethod("", entityType, Type.EmptyTypes);
            var generator = method.GetILGenerator();
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            return (Func<TEntity>)method.CreateDelegate(typeof(Func<TEntity>));
        }

        public Action<TEntity, object[]> GetReader<TEntity>(IMemberMapping[] mappings)
        {
            Type entityType = typeof(TEntity);
            DynamicMethod method = new DynamicMethod(
                "__FlatFiles_TypeMapping_read",
                typeof(void),
                new Type[] { entityType, typeof(object[]) },
                true);
            var generator = method.GetILGenerator();
            for (int index = 0; index != mappings.Length; ++index)
            {
                IMemberMapping mapping = mappings[index];
                if (mapping.Member == null)
                {
                    continue;
                }
                generator.Emit(OpCodes.Ldarg, 0);
                generator.Emit(OpCodes.Ldarg, 1);
                generator.Emit(OpCodes.Ldc_I4, mapping.WorkIndex);
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
            }

            generator.Emit(OpCodes.Ret);

            var result = (Action<TEntity, object[]>)method.CreateDelegate(typeof(Action<TEntity, object[]>));
            return result;
        }

        public Action<TEntity, object[]> GetWriter<TEntity>(IMemberMapping[] mappings)
        {
            Type entityType = typeof(TEntity);
            DynamicMethod method = new DynamicMethod(
                "__FlatFiles_TypeMapping_write",
                null,
                new Type[] { entityType, typeof(object[]) },
                true);
            var generator = method.GetILGenerator();

            for (int index = 0; index != mappings.Length; ++index)
            {
                IMemberMapping mapping = mappings[index];
                if (mapping.Member == null)
                {
                    continue;
                }

                generator.Emit(OpCodes.Ldarg_1);
                generator.Emit(OpCodes.Ldc_I4, mapping.WorkIndex);

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
            
            generator.Emit(OpCodes.Ret);

            var result = (Action<TEntity, object[]>)method.CreateDelegate(typeof(Action<TEntity, object[]>));
            return result;
        }
    }
}
