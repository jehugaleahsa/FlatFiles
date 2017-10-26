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

        Action<TEntity, object[]> GetReader<TEntity>(List<IPropertyMapping> mappings);

        Func<TEntity, object[]> GetWriter<TEntity>(List<IPropertyMapping> mappings);
    }

    internal sealed class ReflectionCodeGenerator : ICodeGenerator
    {
        public Func<object> GetFactory(Type entityType)
        {
            return () => Activator.CreateInstance(entityType);
        }

        public Action<TEntity, object[]> GetReader<TEntity>(List<IPropertyMapping> mappings)
        {
            Action<TEntity, object[]> reader = (entity, values) =>
            {
                int position = 0;
                for (int index = 0; index != mappings.Count; ++index)
                {
                    IPropertyMapping mapping = mappings[index];
                    if (mapping.Property == null)
                    {
                        continue;
                    }
                    object value = values[position];
                    mapping.Property.SetValue(entity, value, null);
                    ++position;
                }
            };
            return reader;
        }

        public Func<TEntity, object[]> GetWriter<TEntity>(List<IPropertyMapping> mappings)
        {
            Func<TEntity, object[]> writer = (entity) =>
            {
                var values = from mapping in mappings
                             where mapping.Property != null
                             select mapping.Property.GetValue(entity, null);
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

        public Action<TEntity, object[]> GetReader<TEntity>(List<IPropertyMapping> mappings)
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
                IPropertyMapping mapping = mappings[index];
                if (mapping.Property == null)
                {
                    continue;
                }

                MethodInfo setter = mapping.Property.GetSetMethod();
                if (setter == null)
                {
                    string message = String.Format(null, SharedResources.ReadOnlyProperty, mapping.Property.Name);
                    throw new FlatFileException(message);
                }
                generator.Emit(OpCodes.Ldarg, 0);

                generator.Emit(OpCodes.Ldarg, 1);
                generator.Emit(OpCodes.Ldc_I4, position);
                generator.Emit(OpCodes.Ldelem_Ref);

                Type propertyType = mapping.Property.PropertyType;
                generator.Emit(OpCodes.Unbox_Any, propertyType);

                generator.Emit(OpCodes.Callvirt, setter);

                ++position;
            }

            generator.Emit(OpCodes.Ret);

            var result = (Action<TEntity, object[]>)method.CreateDelegate(typeof(Action<TEntity, object[]>));
            return result;
        }

        public Func<TEntity, object[]> GetWriter<TEntity>(List<IPropertyMapping> mappings)
        {
            Type entityType = typeof(TEntity);
            DynamicMethod method = new DynamicMethod(
                "__FlatFiles_TypeMapping_write",
                typeof(object[]),
                new Type[] { entityType },
                true);
            var remaining = mappings.Where(m => m.Property != null).ToArray();
            var generator = method.GetILGenerator();
            generator.DeclareLocal(typeof(object[]));
            generator.Emit(OpCodes.Ldc_I4, remaining.Length);
            generator.Emit(OpCodes.Newarr, typeof(object));
            generator.Emit(OpCodes.Stloc_0);

            for (int index = 0; index != remaining.Length; ++index)
            {
                IPropertyMapping mapping = remaining[index];

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldc_I4, index);

                MethodInfo getter = mapping.Property.GetGetMethod();
                if (getter == null)
                {
                    string message = String.Format(null, SharedResources.WriteOnlyProperty, mapping.Property.Name);
                    throw new FlatFileException(message);
                }
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Callvirt, getter);
                Type propertyType = mapping.Property.PropertyType;
                if (!propertyType.GetTypeInfo().IsClass)
                {
                    generator.Emit(OpCodes.Box, propertyType);
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
