using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FlatFiles.TypeMapping
{
    internal static class CodeGenerator
    {
        public static Action<TEntity, object[]> GetReader<TEntity>(
            Dictionary<string, IPropertyMapping> mappings,
            Dictionary<string, int> indexes)
        {
            Type entityType = typeof(TEntity);
            DynamicMethod method = new DynamicMethod(
                "__FixedLengthTypeMapper_read",
                typeof(void),
                new Type[] { entityType, typeof(object[]) },
                true);
            var generator = method.GetILGenerator();
            foreach (string propertyName in mappings.Keys)
            {
                IPropertyMapping mapping = mappings[propertyName];
                int index = indexes[propertyName];
                MethodInfo setter = mapping.Property.GetSetMethod();
                generator.Emit(OpCodes.Ldarg, 0);

                generator.Emit(OpCodes.Ldarg, 1);
                generator.Emit(OpCodes.Ldc_I4, index);
                generator.Emit(OpCodes.Ldelem_Ref);

                Type propertyType = mapping.Property.PropertyType;
                generator.Emit(OpCodes.Unbox_Any, propertyType);

                generator.Emit(OpCodes.Callvirt, setter);
            }

            generator.Emit(OpCodes.Ret);

            var result = (Action<TEntity, object[]>)method.CreateDelegate(typeof(Action<TEntity, object[]>));
            return result;
        }

        public static Func<TEntity, object[]> GetWriter<TEntity>(
            Dictionary<string, IPropertyMapping> mappings,
            Dictionary<string, int> indexes)
        {
            Type entityType = typeof(TEntity);
            DynamicMethod method = new DynamicMethod(
                "__FixedLengthTypeMapper_write",
                typeof(object[]),
                new Type[] { entityType },
                true);
            var generator = method.GetILGenerator();
            generator.DeclareLocal(typeof(object[]));
            generator.Emit(OpCodes.Ldc_I4, mappings.Count);
            generator.Emit(OpCodes.Newarr, typeof(object));
            generator.Emit(OpCodes.Stloc_0);

            foreach (string propertyName in mappings.Keys)
            {
                IPropertyMapping mapping = mappings[propertyName];
                int index = indexes[propertyName];
                MethodInfo getter = mapping.Property.GetGetMethod();

                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldc_I4, index);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Callvirt, getter);
                Type propertyType = mapping.Property.PropertyType;
                if (!propertyType.IsClass)
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

        public static Action<TEntity, object[]> GetSlowReader<TEntity>(
            Dictionary<string, IPropertyMapping> mappings,
            Dictionary<string, int> indexes)
        {
            Action<TEntity, object[]> reader = (entity, values) =>
            {
                foreach (string propertyName in mappings.Keys)
                {
                    IPropertyMapping mapping = mappings[propertyName];
                    int index = indexes[propertyName];
                    object value = values[index];
                    mapping.Property.SetValue(entity, value, null);
                }
            };
            return reader;
        }

        public static Func<TEntity, object[]> GetSlowWriter<TEntity>(
            Dictionary<string, IPropertyMapping> mappings,
            Dictionary<string, int> indexes)
        {
            Func<TEntity, object[]> writer = (entity) =>
            {
                object[] values = new object[mappings.Count];
                foreach (string propertyName in mappings.Keys)
                {
                    IPropertyMapping mapping = mappings[propertyName];
                    int index = indexes[propertyName];
                    values[index] = mapping.Property.GetValue(entity, null);
                }
                return values;
            };
            return writer;
        }
    }
}
