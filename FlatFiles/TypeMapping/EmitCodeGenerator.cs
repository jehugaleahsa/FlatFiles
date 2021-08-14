using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;
using FlatFiles.Properties;

namespace FlatFiles.TypeMapping
{
    internal sealed class EmitCodeGenerator : ICodeGenerator
    {
        private readonly ConcurrentDictionary<string, int> nameLookup = new();
        private readonly AssemblyBuilder assemblyBuilder;
        private readonly ModuleBuilder moduleBuilder;

        public EmitCodeGenerator()
        {
            var assemblyName = new AssemblyName("FlatFiles.DynamicAssembly");
            var flatFilesAssembly = typeof(ICodeGenerator).GetTypeInfo().Assembly;
            var flatFilesAssemblyName = flatFilesAssembly.GetName();
            var publicKey = flatFilesAssemblyName.GetPublicKey();
            assemblyName.SetPublicKey(publicKey);
            var publicKeyToken = flatFilesAssemblyName.GetPublicKeyToken();
            assemblyName.SetPublicKeyToken(publicKeyToken);
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
            moduleBuilder = assemblyBuilder.DefineDynamicModule("FlatFiles_DynamicModule");
        }

        private string GetUniqueTypeName(string name)
        {
            int id = nameLookup.AddOrUpdate(name, 0, (_, old) => old + 1);
            return $"{name}_{id}";
        }

        public Func<TEntity> GetFactory<TEntity>()
        {
            var entityType = typeof(TEntity);
            var constructorInfo = MemberAccessorBuilder.GetConstructor<TEntity>(Type.EmptyTypes);
            if (constructorInfo == null)
            {
                throw new FlatFileException(Resources.NoDefaultConstructor);
            }
            var typeName = GetUniqueTypeName($"{entityType.Name}Factory");
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed);
            var methodBuilder = typeBuilder.DefineMethod("Create", MethodAttributes.Public | MethodAttributes.Static, entityType, Type.EmptyTypes);
            var generator = methodBuilder.GetILGenerator();
            generator.Emit(OpCodes.Newobj, constructorInfo);
            generator.Emit(OpCodes.Ret);
            var typeInfo = typeBuilder.CreateTypeInfo()!;
            var createInfo = typeInfo.GetMethod(methodBuilder.Name)!;
            return (Func<TEntity>)createInfo.CreateDelegate(typeof(Func<TEntity>))!;
        }

        public Action<IRecordContext, TEntity, object?[]> GetReader<TEntity>(IMemberMapping[] mappings)
        {
            var entityType = typeof(TEntity);
            var typeName = GetUniqueTypeName($"{entityType.Name}Reader");
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Sealed);
            var fieldBuilder = typeBuilder.DefineField("mappings", typeof(IMemberMapping[]), FieldAttributes.Private);
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IMemberMapping[]) });
            var ctorGenerator = ctorBuilder.GetILGenerator();
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldarg_1);
            ctorGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            ctorGenerator.Emit(OpCodes.Ret);

            var methodBuilder = typeBuilder.DefineMethod("Read", MethodAttributes.Public, null, new[] { typeof(IRecordContext), entityType, typeof(object[]) });
            var methodGenerator = methodBuilder.GetILGenerator();
            for (int index = 0; index != mappings.Length; ++index)
            {
                var mapping = mappings[index];
                if (mapping.Member != null)
                {
                    EmitMemberRead(methodGenerator, mapping.Member, mapping.LogicalIndex);
                }
                else if (mapping.Reader != null)
                {
                    EmitCustomRead(methodGenerator, fieldBuilder, index);
                }
            }
            methodGenerator.Emit(OpCodes.Ret);

            var typeInfo = typeBuilder.CreateTypeInfo()!;
            var instance = Activator.CreateInstance(typeInfo.AsType(), (object)mappings);
            var readInfo = typeInfo.GetMethod(methodBuilder.Name)!;
            return (Action<IRecordContext, TEntity, object?[]>)readInfo.CreateDelegate(typeof(Action<IRecordContext, TEntity, object?[]>), instance);
        }

        private static void EmitMemberRead(ILGenerator generator, IMemberAccessor member, int logicalIndex)
        {
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Ldarg_3);
            generator.Emit(OpCodes.Ldc_I4, logicalIndex);
            generator.Emit(OpCodes.Ldelem_Ref);

            if (member.MemberInfo is FieldInfo fieldInfo)
            {
                generator.Emit(OpCodes.Unbox_Any, fieldInfo.FieldType);
                generator.Emit(OpCodes.Stfld, fieldInfo);
            }
            else if (member.MemberInfo is PropertyInfo propertyInfo)
            {
                MethodInfo? setter = propertyInfo.GetSetMethod(true);
                if (setter == null)
                {
                    string message = string.Format(null, Resources.ReadOnlyProperty, propertyInfo.Name);
                    throw new FlatFileException(message);
                }
                generator.Emit(OpCodes.Unbox_Any, propertyInfo.PropertyType);
                generator.Emit(OpCodes.Callvirt, setter);
            }
        }

        private static void EmitCustomRead(ILGenerator generator, FieldInfo fieldInfo, int mappingIndex)
        {
            var contextBuilder = generator.DeclareLocal(typeof(ColumnContext));
            var mappingBuilder = generator.DeclareLocal(typeof(IMemberMapping));
            var indexBuilder = generator.DeclareLocal(typeof(int));

            // Gets the mapping being processed
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, fieldInfo);
            generator.Emit(OpCodes.Ldc_I4, mappingIndex);
            generator.Emit(OpCodes.Ldelem_Ref);
            generator.Emit(OpCodes.Stloc, mappingBuilder);

            // Create the Context, passing in RecordContext, physical index and logical index
            generator.Emit(OpCodes.Ldarg_1);

            generator.Emit(OpCodes.Ldloc, mappingBuilder);
            var physicalIndexGetInfo = MemberAccessorBuilder.GetProperty<IMemberMapping, int>(x => x.PhysicalIndex)!;
            var physicalIndexGetter = physicalIndexGetInfo.GetGetMethod()!;
            generator.Emit(OpCodes.Callvirt, physicalIndexGetter);

            // Set ColumnContext.LogicalIndex
            generator.Emit(OpCodes.Ldloc, mappingBuilder);
            var logicalIndexGetInfo = MemberAccessorBuilder.GetProperty<IMemberMapping, int>(x => x.LogicalIndex)!;
            var logicalIndexGetter = logicalIndexGetInfo.GetGetMethod()!;
            generator.Emit(OpCodes.Callvirt, logicalIndexGetter);
            generator.Emit(OpCodes.Dup);
            generator.Emit(OpCodes.Stloc, indexBuilder);

            var contextCtorInfo = MemberAccessorBuilder.GetConstructor<ColumnContext>(typeof(IRecordContext), typeof(int), typeof(int))!;
            generator.Emit(OpCodes.Newobj, contextCtorInfo);
            generator.Emit(OpCodes.Stloc, contextBuilder);

            // Get the reader
            generator.Emit(OpCodes.Ldloc, mappingBuilder);
            var readerGetInfo = MemberAccessorBuilder.GetProperty<IMemberMapping, Action<IColumnContext?, object?, object?>?>(x => x.Reader)!;
            var readerGetter = readerGetInfo.GetGetMethod()!;
            generator.Emit(OpCodes.Callvirt, readerGetter);

            // Load the parameters
            generator.Emit(OpCodes.Ldloc, contextBuilder);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Ldarg_3);
            generator.Emit(OpCodes.Ldloc, indexBuilder);
            generator.Emit(OpCodes.Ldelem_Ref);

            // Invoke the reader
            var invokeInfo = MemberAccessorBuilder.GetMethod<Action<IColumnContext?, object?, object?>>(x => x.Invoke(null, null, null))!;
            generator.Emit(OpCodes.Callvirt, invokeInfo);
        }

        public Action<IRecordContext, TEntity, object?[]> GetWriter<TEntity>(IMemberMapping[] mappings)
        {
            var entityType = typeof(TEntity);
            var typeName = GetUniqueTypeName($"{entityType.Name}Writer");
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Sealed);
            var fieldBuilder = typeBuilder.DefineField("mappings", typeof(IMemberMapping[]), FieldAttributes.Private);
            var ctorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IMemberMapping[]) });
            var ctorGenerator = ctorBuilder.GetILGenerator();
            ctorGenerator.Emit(OpCodes.Ldarg_0);
            ctorGenerator.Emit(OpCodes.Ldarg_1);
            ctorGenerator.Emit(OpCodes.Stfld, fieldBuilder);
            ctorGenerator.Emit(OpCodes.Ret);

            var methodBuilder = typeBuilder.DefineMethod("Write", MethodAttributes.Public, null, new[] { typeof(IRecordContext), entityType, typeof(object[]) });
            var methodGenerator = methodBuilder.GetILGenerator();
            for (int index = 0; index != mappings.Length; ++index)
            {
                var mapping = mappings[index];
                if (mapping.Member != null)
                {
                    EmitMemberWrite(methodGenerator, mapping.Member, mapping.LogicalIndex);
                }
                else if (mapping.Writer != null)
                {
                    EmitCustomWrite<TEntity>(methodGenerator, fieldBuilder, index);
                }
            }
            methodGenerator.Emit(OpCodes.Ret);

            var typeInfo = typeBuilder.CreateTypeInfo()!;
            var instance = Activator.CreateInstance(typeInfo.AsType(), (object)mappings);
            var writeMethodInfo = typeInfo.GetMethod(methodBuilder.Name)!;
            return (Action<IRecordContext, TEntity, object?[]>)writeMethodInfo.CreateDelegate(typeof(Action<IRecordContext, TEntity, object?[]>), instance);
        }

        private static void EmitMemberWrite(ILGenerator generator, IMemberAccessor member, int logicalIndex)
        {
            generator.Emit(OpCodes.Ldarg_3);
            generator.Emit(OpCodes.Ldc_I4, logicalIndex);
            generator.Emit(OpCodes.Ldarg_2);

            if (member.MemberInfo is FieldInfo fieldInfo)
            {
                generator.Emit(OpCodes.Ldfld, fieldInfo);
                var fieldType = fieldInfo.FieldType;
                if (!fieldType.GetTypeInfo().IsClass)
                {
                    generator.Emit(OpCodes.Box, fieldType);
                }
            }
            else if (member.MemberInfo is PropertyInfo propertyInfo)
            {
                var getter = propertyInfo.GetGetMethod(true);
                if (getter == null)
                {
                    string message = string.Format(null, Resources.WriteOnlyProperty, propertyInfo.Name);
                    throw new FlatFileException(message);
                }
                generator.Emit(OpCodes.Callvirt, getter);
                var propertyType = propertyInfo.PropertyType;
                if (!propertyType.GetTypeInfo().IsClass)
                {
                    generator.Emit(OpCodes.Box, propertyType);
                }
            }
            generator.Emit(OpCodes.Stelem_Ref);
        }

        private static void EmitCustomWrite<TEntity>(ILGenerator generator, FieldInfo fieldInfo, int mappingIndex)
        {
            var contextBuilder = generator.DeclareLocal(typeof(ColumnContext));
            var mappingBuilder = generator.DeclareLocal(typeof(IMemberMapping));

            // Gets the mapping being processed
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, fieldInfo);
            generator.Emit(OpCodes.Ldc_I4, mappingIndex);
            generator.Emit(OpCodes.Ldelem_Ref);
            generator.Emit(OpCodes.Stloc, mappingBuilder);

            // Create the ColumnContext, passing RecordContext, physical index and logical index
            generator.Emit(OpCodes.Ldarg_1);

            // Set ColumnContext.PhysicalIndex
            generator.Emit(OpCodes.Ldloc, mappingBuilder);
            var physicalIndexGetInfo = MemberAccessorBuilder.GetProperty<IMemberMapping, int>(x => x.PhysicalIndex)!;
            var physicalIndexGetter = physicalIndexGetInfo.GetGetMethod()!;
            generator.Emit(OpCodes.Callvirt, physicalIndexGetter);

            // Set ColumnContext.LogicalIndex
            generator.Emit(OpCodes.Ldloc, mappingBuilder);
            var logicalIndexGetInfo = MemberAccessorBuilder.GetProperty<IMemberMapping, int>(x => x.LogicalIndex)!;
            var logicalIndexGetter = logicalIndexGetInfo.GetGetMethod()!;
            generator.Emit(OpCodes.Callvirt, logicalIndexGetter);

            var contextCtorInfo = MemberAccessorBuilder.GetConstructor<ColumnContext>(typeof(IRecordContext), typeof(int), typeof(int))!;
            generator.Emit(OpCodes.Newobj, contextCtorInfo);
            generator.Emit(OpCodes.Stloc, contextBuilder);

            // Get the writer
            generator.Emit(OpCodes.Ldloc, mappingBuilder);
            var writerInfo = MemberAccessorBuilder.GetProperty<IMemberMapping, Action<IColumnContext?, object?, object?[]>?>(x => x.Writer)!;
            var writerGetter = writerInfo.GetGetMethod()!;
            generator.Emit(OpCodes.Callvirt, writerGetter);

            // Load the parameters
            generator.Emit(OpCodes.Ldloc, contextBuilder);
            generator.Emit(OpCodes.Ldarg_2);
            generator.Emit(OpCodes.Ldarg_3);

            // Invoke the writer
            var invokeInfo = MemberAccessorBuilder.GetMethod<Action<IColumnContext?, object?, object?>>(x => x.Invoke(null, null, null))!;
            generator.Emit(OpCodes.Callvirt, invokeInfo);
        }
    }
}
