## 3.0.0 (2018-06-29)
**Summary** - Introducing custom mapping support and more contextual information.

### Enhancements
* The new type mapper method, `CustomMapping`, grants full control over the way values are mapped between raw `object[]` values and entities.
* All exceptions, events and custom mapping features now provide access to column, record and/or execution context.
* Fewer restrictions on the number of environments FlatFiles can run.

### Breaking Changes
* The `CustomProperty` and `WriteOnlyProperty` methods have been superseded by the `CustomMapping` method, so they have been removed.
* The `IProcessMetadata` interface has been replaced by the `IRecordContext` interface.
* The `RecordNumber` property of `RecordProcessingException` has been replaced with the `Context` property.
* The `ProcessingErrorEventArgs` class has been replaced by the `ExecutionErrorEventArgs` class.
* The `IncludeFilteredRecords` property of `RecordNumberColumn` has been renamed to `IncludeSkippedRecords`.
* The `IColumnDefinition` interface methods `Parse` and `Format` now accept `IColumnContext` objects.
* The `IMetadataColumn` interface no longer has the `GetValue` method. Use the `MetadataColumn` base class instead. 

Most significantly of all, previous versions of FlatFiles used `DynamicMethod` to generate code at runtime. A `DynamicMethod` can be configured to allow the generated code to access non-public classes and members from other assemblies. However, this additional access requires the code to be running in a trusted environment, meaning FlatFiles could not be used in a sandboxed environment.

The new custom mapping functionality required the creation of types at runtime, so `DynamicMethod` was no longer an option. However, there is no means of granting dynamic types access to non-public classes/members in other assemblies. Going forward, FlatFiles will only be able to access public classes and members in your projects. If you need FlatFiles to access internal classes/members, you can add this line to you `Assembly.cs` file:

```csharp
[assembly: InternalsVisibleTo("FlatFiles.DynamicAssembly,PublicKey=00240000048000009400000006020000002400005253413100040000010001009b9e44f637b293021ec4d8625071e5fe1682eeb167c233b46314cca79bf2769606285d5d1225cba8ce1e75be9e8ab7251d17eaf2c3b00fde5eac50a0f7dc7fec2f70279ff71c72341ad2738661babfdc6792479f14fd64d841285644d5c09c2902e9467f574e0d369161caee632087c5d819c3c36f76622306b09a4f868230c1")]
```

Otherwise, you can disable runtime optimization by calling `OptimizeMapping(false)` on your mapping, which will cause FlatFiles to fallback on reflection which can access private members at the cost of runtime overhead. Another alternative is to pass a delegate that accesses the internal member to the `CustomMapping` method.

Forcing users to add the `[InternalsVisibleTo]` attribute is in-line with what other .NET libraries involving runtime generation of types are doing (e.g., Moq and Castle.DynamicProxy). While this is may be inconvenient to some users, it makes the library more portable. It also mean, FlatFiles no longer depends on the (System.Reflection.Emit.LightWeight)[https://www.nuget.org/packages/System.Reflection.Emit.Lightweight/] NuGet package which is now considered [obsolete](https://github.com/dotnet/source-build/issues/532). 

## 2.1.3 (2018-06-16)
**Summary** - Use `ConfigureAwait(false)` for all async operations.

### Enhancements
* Using `ConfigureAwait(false)` consistently can improve async/await performance and avoid deadlocks in some environments. For more information, read [this article](https://msdn.microsoft.com/en-us/magazine/jj991977.aspx).

## 2.1.2 (2018-06-16)
**Summary** - Code modernization and bug fixes.

### Bug Fixes
* Skipping the last record in a fixed-length file causes an error.

### Enhancements
* Cleaned up the code to use latest C# 7.3 features
* Added enum generic-constraint for EnumColumn and type mapping methods.

## 2.1.1 (2018-06-11)
**Summary** - Remove unneeded references to .NET Standard projects when targeting .NET 4.5.1. Updated resource file to be recognized by project.

## 2.1.0 (2018-06-05)
**Summary** - Write files with multiple schemas.

### New Features
I wanted to make sure flat files consisting of multiple schemas could be generated similar to the way they are read. Parallel to the "selectors" used to read files, there are now injectors for writing files. This release introduces the `SeparatedValueSchemaInjector`, `FixedLengthSchemaInjector`, `SeparatedValueTypeMapperInjector` and the `FixedLengthTypeMapperInjector` classes.

### Enhancements
* Several minor performance enhancements for the `SeparatedValueWriter`  and `FixedLengthWriter` classes.

## 2.0.0 (2018-06-05)
**Summary** - Read files with multiple schemas.

### New Features
Several requests were made to support files containing multiple schemas. Especially in fixed-length files, the use of data blocks with header and footer records is normal. The introduction of the `SeparatedValueSchemaSelector`, `FixedLengthSchemaSelector`, `SeparatedValueTypeMapperSelector` and `FixedLengthTypeMapperSelector` make it possible take existing schemas and type mappers, respectively, and combine them so the appropriate schema is used to parse a record.

### Breaking Changes
In previous versions, records could be skipped using `Func` properties on the options object. A similar property existed for handling errors while processing files. However, the naming and usage was not obvious. Going forward, readers will expose events for registering callback methods which can be used to skip records. For example:

```csharp
var mapper = new SeparatedValueTypeMapper<Person>(() => new Person());
// ...configure the type mapper
var reader = mapper.GetReader(stringReader, options);
// Register a handler that fires any time a record is extracted
reader.RecordRead += (sender, e) =>
{
    e.IsSkipped = e.Values.Length > 0 && e.Values[0] == "#Comment";
};
// Read all the records, skipping records starting with '#Comment'.
var results = reader.ReadAll().ToArray();
```

The properties on the options object have been removed in favor of the new events. Read the [README](https://github.com/jehugaleahsa/FlatFiles/blob/master/README.md#skipping-records) for more details.
