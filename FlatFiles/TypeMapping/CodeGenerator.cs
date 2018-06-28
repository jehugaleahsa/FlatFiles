using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    internal interface ICodeGenerator
    {
        Func<TEntity> GetFactory<TEntity>();

        Action<IProcessMetadata, TEntity, object[]> GetReader<TEntity>(IMemberMapping[] mappings);

        Action<IProcessMetadata, TEntity, object[]> GetWriter<TEntity>(IMemberMapping[] mappings);
    }

    internal sealed class ReflectionCodeGenerator : ICodeGenerator
    {
        public Func<TEntity> GetFactory<TEntity>()
        {
            return () => (TEntity)Activator.CreateInstance(typeof(TEntity), true);
        }

        public Action<IProcessMetadata, TEntity, object[]> GetReader<TEntity>(IMemberMapping[] mappings)
        {
            void Reader(IProcessMetadata metadata, TEntity entity, object[] values)
            {
                for (int index = 0; index != mappings.Length; ++index)
                {
                    IMemberMapping mapping = mappings[index];
                    if (mapping.Member != null)
                    {
                        object value = values[mapping.LogicalIndex];
                        mapping.Member.SetValue(entity, value);
                    }
                }
            }

            return Reader;
        }

        public Action<IProcessMetadata, TEntity, object[]> GetWriter<TEntity>(IMemberMapping[] mappings)
        {
            void Writer(IProcessMetadata metadata, TEntity entity, object[] values)
            {
                for (int index = 0; index != mappings.Length; ++index)
                {
                    IMemberMapping mapping = mappings[index];
                    if (mapping.Member != null)
                    {
                        object value = mapping.Member.GetValue(entity);
                        values[mapping.LogicalIndex] = value;
                    }
                }
            }

            return Writer;
        }
    }

    internal sealed class EmitCodeGenerator : ICodeGenerator
    {
        public Func<TEntity> GetFactory<TEntity>()
        {
            Type entityType = typeof(TEntity);
            var bindingFlags = BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var constructorInfo = entityType.GetTypeInfo()
                .GetConstructors(bindingFlags)
                .SingleOrDefault(c => c.GetParameters().Length == 0);
            if (constructorInfo == null)
            {
                throw new FlatFileException(Resources.NoDefaultConstructor);
            }
            var method = new DynamicMethod("", entityType, Type.EmptyTypes, true);
            var generator = method.GetILGenerator();
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            return (Func<TEntity>)method.CreateDelegate(typeof(Func<TEntity>));
        }

        public Action<IProcessMetadata, TEntity, object[]> GetReader<TEntity>(IMemberMapping[] mappings)
        {
            Type entityType = typeof(TEntity);
            var method = new DynamicMethod(
                "__FlatFiles_TypeMapping_read",
                typeof(void),
                new[] { typeof(IProcessMetadata), entityType, typeof(object[]) },
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
                generator.Emit(OpCodes.Ldarg_2);
                generator.Emit(OpCodes.Ldc_I4, mapping.LogicalIndex);
                generator.Emit(OpCodes.Ldelem_Ref);

                if (mapping.Member.MemberInfo is FieldInfo fieldInfo)
                {
                    generator.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                    generator.Emit(OpCodes.Stfld, fieldInfo);
                }
                else if (mapping.Member.MemberInfo is PropertyInfo propertyInfo)
                {
                    MethodInfo setter = propertyInfo.GetSetMethod(true);
                    if (setter == null)
                    {
                        string message = String.Format(null, Resources.ReadOnlyProperty, propertyInfo.Name);
                        throw new FlatFileException(message);
                    }
                    generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                    generator.Emit(OpCodes.Callvirt, setter);
                }
            }
            generator.Emit(OpCodes.Ret);

            var result = (Action<IProcessMetadata, TEntity, object[]>)method.CreateDelegate(typeof(Action<IProcessMetadata, TEntity, object[]>));
            return result;
        }

        public Action<IProcessMetadata, TEntity, object[]> GetWriter<TEntity>(IMemberMapping[] mappings)
        {
            Type entityType = typeof(TEntity);
            var method = new DynamicMethod(
                "__FlatFiles_TypeMapping_write",
                null,
                new[] { typeof(IProcessMetadata), entityType, typeof(object[]) },
                true);
            var generator = method.GetILGenerator();

            for (int index = 0; index != mappings.Length; ++index)
            {
                IMemberMapping mapping = mappings[index];
                if (mapping.Member == null)
                {
                    continue;
                }

                generator.Emit(OpCodes.Ldarg_2);
                generator.Emit(OpCodes.Ldc_I4, mapping.LogicalIndex);
                generator.Emit(OpCodes.Ldarg_1);

                if (mapping.Member.MemberInfo is FieldInfo fieldInfo)
                {
                    generator.Emit(OpCodes.Ldfld, fieldInfo);
                    Type fieldType = fieldInfo.FieldType;
                    if (!fieldType.GetTypeInfo().IsClass)
                    {
                        generator.Emit(OpCodes.Box, fieldType);
                    }
                }
                else if (mapping.Member.MemberInfo is PropertyInfo propertyInfo)
                {
                    MethodInfo getter = propertyInfo.GetGetMethod(true);
                    if (getter == null)
                    {
                        string message = String.Format(null, Resources.WriteOnlyProperty, propertyInfo.Name);
                        throw new FlatFileException(message);
                    }
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

            var result = (Action<IProcessMetadata, TEntity, object[]>)method.CreateDelegate(typeof(Action<IProcessMetadata, TEntity, object[]>));
            return result;
        }
    }
}
