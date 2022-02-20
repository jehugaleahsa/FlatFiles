## 5.0.0 (2022-02-20)
**Summary** - Rename classes/methods/etc. from "separated value" to "delimited". Break out each class into its own file. Introduce nullable references compile setting and improve null safety.

For the past several years, my classes being named `SeparatedValueReader` (and the like) has been a thorn in my side. Pretty much universally other libraries, and people in general, use the term "delimited". Delimited is also a smaller word, which makes typing it over and over less of a pain. While it will cause long-time users of the library some discomfort, the rename shouldn't take too long (I just did it 10,000 times for this version so I feel your pain).

Coming back to this code base after years of minimal maintenance, I had no idea where to find half my classes. I think I was afraid of the sheer number of files I'd end up with should I break everything out; however, a little more scrolling is better than guessing the correct file. This is mostly noticeable when looking through source control, not in the IDE. Almost every public interface and class (and many internal classes) are in their own files now.

I wanted to enable nullable reference types for a while. Immediately after enabling it, I had thousands of compile warnings. In fixing the warnings, I actually improved the code for processing and filtering records in the parsers, fixed my overall event broadcasting mechanism, and fixed several subtle bugs I wouldn't have caught otherwise. Some of these changes resulted in minor interface changes, hence the 5.0.0.

Ultimately, this is not the 5.0.0 release I was hoping for. I was hoping to include several other major refactorings before letting this loose on the world. My long-term plan, assuming there's a continued interest, is to separate out my delimited parser into a separate, stupid set of classes. These classes would use the latest .NET features to streamline that parsing process and give you, the library consumer, direct access to the low-level performance benefits if you needed it. I want to then separate out the schema definitions and make it possible to parse values coming from anywhere. I want to revisit the code generation happening in the type mappers. I don't have much of an appetite for those things, yet. But... I want to make sure the enhancements I made several months back get out there, as well as make sure bug fixes aren't targeting some older version.

## 4.16.0 (2021-06-27)
**Summary** - Several years ago, I was looking into the use of `IAsyncEnumerable`. I actually had some code already that would work exclusively for .NET Core App 3.0+. Now that `IAsyncEnumerable` is part is .NET Standard 2.1, I can support for additional runtimes. I officially added a .NET Standard 2.1 build. I then added additional extension methods for the `TypedReader` and `TypedWriter` classes. I also added methods to the type mapper classes that use these extension methods internally. I found various other places where I could use the extension methods, so reimplemented them where I could. This sort of code duplication actually led to some bugs recently, so I think it benefits me to consolidate as much as I can.

## 4.15.0 (2021-05-10)
**Summary** - Recent changes to write out headers when writing multiple records only applied to the extension method `WriteAll`. However, the mapper class's `Write` methods have a similar semantic and behaved the same as before. This commit actually changes the mapper methods to call `WriteAll` under the hood, so now the same behavior will be exhibited.

I discovered a bug I introduced with the previous version where I wrote the schema out no matter what. I only want to do this if the writer is configured to write out the schema, which is `false` by default.

## 4.14.0 (2021-05-01)
**Summary** - The behavior of `TypedWriter.WriteAll` is somewhat unintuitive when called with no records. The expectation is that the header is written when performing this bulk operation; otherwise, the caller has to explicitly call `WriteSchema` beforehand. This slightly changes the behavior of the code, such that it might result in headers/schema being written in cases where the file was blank before. However, when `IsFirstRecordSchema` is `true`, it is extremely unlikely consumers would expect a blank file to be generated.

During my testing, I also discovered a bug where the schema was getting set, then unset, when the header/schema record was the only record in the file. You should be able to try to `Read` the first record of an empty file and get `false` back, then get the schema via `GetSchema`; however, my code was throwing an `InvalidOperationException` or, worse, a `NullReferenceException`.

## 4.13.0 (2020-12-03)
**Summary** - This change allows the original text making up a record to be viewed while parsing a file. The raw record contents will be accessible via the `IRecordContext` interface, which is available within the event args.

I originally had some concerns regarding memory usage or performance impacts, but after profiling and benchmarking, no significant performance issues were detected.

## 4.12.0 (2020-10-21)
**Summary** - `FlatFileDataReader` not correctly ignoring ignored columns in several places.

The ADO.NET classes didn't receive the same level of love that the rest of the library received when introducing ignored columns. When getting the column names, their ordinal positions, etc., the `FlatFileDataReader` was returning information for ignored columns. This caused the `DataTable` extensions to see too many column names and yet receive too few record values in the table (with the data shifted to the left for each missing column). Fixing the `FlatFileDataReader` fixed the `DataTable` problems, as well.

Technically this is a breaking change that might warrant a major version change; however, as the previous behavior could not possibly be desired and few people actually use the ADO.NET classes, I am going to include this in the next minor version, treating it as just a bug fix.

## 4.11.0 (2020-10-09)
**Summary** - Allow handling unrecognized rows when using schema selectors.

Previously, if a record was encountered that could be handled by any of the configured schemas, the selector would throw a generic `FlatFilesException`. Now, a `RecordProcessingException`is thrown instead, which can be ignored causing the record to be skipped.

## 4.10.0 (2020-10-06)
**Summary** - Add the ability to explicitly write the schema using typed writers.

I never added support for writing the schema using typed writers. I never added `WriteSchema` and `WriteSchemaAsync` to the `IWriter` interface either. I don't see why not, so I added them.

## 4.9.0 (2020-09-26)
**Summary** - Make OnParsing, OnParsed, OnFormatting, OnFormatted events available to type mappings.

When I introduced the `OnParsing`, `OnParsed`, `OnFormatting` and `OnFormatted` delegates, I marked `Preprocessing` as deprecated but then did not mark it deprecated on the `ColumnDefinition` class or in the column property mapping classes.  Furthermore, I did not add methods to the property mapping classes to allow you to utilize the new delegates. While working on this, I also realized that whenever the `NullFormatter` or `DefaultValue` classes were being used, I was not executing the `OnParsed` and `OnFormatted` delegates with the output of these classes. So, now, I have marked all references to `Preprocessing` as deprecated, added methods to register the delegates on property mappings and now call `OnParsed` and `OnFormatted` regardless of whether the value being parsed/formatted is considered `null`.

One of the primary motivations of these changes was to allow inspecting the values found in ignored columns. For example, you might want to ignore a block of text within a file, but also perform some sanity checks to ensure that the ignored value corresponds to your expectations. For example, you might be working on a fixed-width file and you want to ignore pipe (`|`) characters appearing between values; as a sanity check, you additionally want to ensure the extracted string is in fact a pipe. If you saw something else, you could then assume something was wrong with the input text, such as instead of truncating a string to fit within the fixed-width column, the record got shifted over and the values no longer fit within the expected windows. I updated the `Parse` and `Format` methods to call the `OnParsing`, `OnParsed`, `OnFormatting` and `OnFormatted` delegates just like the other column types and updated the property mappings as well.

An interesting side-effect of these changes is that for `IgnoredColumn`s, spitting out a placeholder value can be achieved either via a `NullFormatter` or using the `OnFormatted` delegate. This is different than the other column types because they are not equipped to handle `null`s.

## 4.8.0 (2020-09-17)
**Summary** - Avoid memory leaks by creating new dynamic assemblies each time.

I was not sure how much of a runtime impact creating a new dynamic assembly would be and so I was trying to optimize the code by storing the `AssemblyBuilder`/`ModuleBuilder` in a static variable. However, I had no mechanism in place to prevent generating duplicate types/methods each time a reader/writer was created, so over time the same emitted code was being added to the dynamic assembly over and over again; a.k.a., a memory leak. At first I tried to implement some sort of caching but then realize that it was almost impossible to uniquely identify a type/column mapping configuration without looking at every property on every mapping. I decided to try creating new assemblies each and every time and that I ran a benchmark. There was no discernable difference in performance, so I think eliminating the premature optimization is the right approach.

## 4.7.0 (2020-02-15)
**Summary** - Support capturing trailing text after last fixed-length window.

For fixed-length files, it may be desirable to capture any extra data trailing after the last configured window. For example, a fixed-length format may be extended in the future with the additional information appended to the end of each record. In that example, in order to be backward compatible, the parser needs to only expect the original set of values (windows) but can perform additional operations on the extra information, if found. Another common practice is to place arbitrary-length text columns, such as comments or descriptions, at the end of records so a specific max length is not required; otherwise, the file format must include that many characters on each record even if most records only use a small portion of the alloted amount. This results in unnecessarily large files consisting of mostly whitespace.

In order to support these types of files, a new `Window.Trailing` indicator was added, which can be passed to a `FixedLengthSchema` or `FixedLengthTypeMapper` where a `Window` is normally expected. FlatFiles will detect this special marker `Window` and configure the reader to grab any trailing text (after the last regular `Window`). When using the `FixedLengthTypeMapper`, you specify the property you want the extra information written to. When working directly with the `FixedLengthReader`, the trailing text appears at the end of the returned arrays. Typically, you want to use a `string` property or `StringColumn` for trailing text, but you can technically use any column type.

You can technically specify the trailing window at any time during configuration; it doesn't matter if you define other windows before or afterwards. If you try to configure multiple trailing windows, the latest configuration is used and is *not* considered an error.

## 4.6.0 (2019-06-03)
**Summary** - Support replacing `DBNull` with `null` when calling `GetValues` on an `IDataRecord`.

### New Features
When working with ADO.NET classes, you often have to deal with `null`s by checking for instances of `DBNull`. Most of the time, this involves calling `IDataRecord.IsDBNull()` or comparing values to `DBNull.Value`. The `DataRecordExtensions` class provides a lot of convenience methods, like `GetNullableString`, that save you from having to distinguish between `null` and `DBNull` all the time. The `FlatFileDataReader` and `DataTableExtensions` classes also provide a lot of features/options surrounding `null` handling. One area where this was overlooked was in the `DataRecordExtensions.GetValues` method.

It was decided that `object[] GetValues()` *(extension)* and `int GetValues(object[])` *(built-in)* should have similar semantics, so instead an optional parameter was added: `replaceDBNulls` that allows specifying whether `DBNull`s should be replaced in the destination array or not (defaults to keeping `DBNull`s). An additional extension: `int GetValues(object[], bool)` was added to provide similar semantics for the built-in method.

### Other
There were also some tests failing due to culture-specific date formatting. Those unit tests were updated to format dates in a culture-insensitive way.

## 4.5.0 (2019-05-29)
**Summary** - Disable wrapping delimited values in quotes.

### New Features
* The `QuoteBehavior` enum now supports a `Never` option, to disable quotes within delimited files. This is set on the `SeparatedValueOptions.QuoteBehavior` property.
    * *NOTE - This can allow the generation of invalid delimited files if they contain the separator token*

## 4.4.0 (2019-05-25)
**Summary** - Allow pre- and post- parsing and formatting on columns. Allow specifying a global `IFormatProvider` in `IOptions`.

### New Features
* The `IOptions` interface now has a `IFormatProvider FormatProvider` property. If provided, all columns will automatically use this `IFormatProvider` as their default.
    * The `IFormatProvider` specified on a `IColumnDefinition` will override what is specified in `IOptions`.
    * If no `IFormatProvider` is found at either level, FlatFiles defaults to `CultureInfo.CurrentInfo`, as before.
* The `IColumnDefinition` interface now exposes four new properties:
    * OnParsing - Fires before attempting to parse the `string` value; the logical successor to `Preprocessor`. 
    * OnParsed - Fires after parsing the column, providing access to the parsed `object`.
    * OnFormatting - Fires before formatting the column, providing access to the `object`.
    * OnFormatted - Fires after formatting the column, providing access to the generated `string`.

Unlike `Preprocessor`, each of the new properties have the type `Func<IColumnContext, T, T>`, providing access to the column context.

### Deprecated
Since `Preprocessor` and `OnParsing` are effectively redundant, `Preprocessor` has been marked deprecated. Please upgrade any existing code to use `OnParsing` instead. The `Preprocessor` property will be removed in the next major release (a.k.a., v5.0).

## 4.3.4 (2019-03-10)
**Summary** - This release allows directly writing arbitrary text to the underlying `TextWriter`, for those who need it.

### New Features
* The `IWriter` interface now exposes `WriteRaw` and `WriteRawAsync` methods.
    * This is implemented by the `SeparatedValueWriter` and `FixedLengthWriter`.
* The `TypedReader` and `TypedWriter` classes now allow directly accessing the underlying `IReader` and `IWriter` objects.

## 4.3.3 (2018-10-15)
**Summary** - The `FixedLengthReader` class would loop indefinitely when partitioning a record whenever there was a metadata column.

### Bug Fixes
The `FixedLengthReader` class would loop indefinitely when partitioning a record whenever there was a metadata column. In this situation, the same index was being used to iterate over the columns/windows and to store the partitioned values in the value array. However, metadata columns are skipped so the index was not being incremented. The solution was to introduce a separate index for the columns/windows and the value array.

## 4.3.2 (2018-09-25)
**Summary** - The `RecordErrorEventArgs.Exception` property is always `null`.

## 4.3.1 (2018-09-25)
**Summary** - Handle `DBNull.Value` when writing out a `DataTable` to a file, treating it as `null`.

## 4.3.0 (2018-07-16)
**Summary** - Further ADO.NET support.

### New Features
* Expose `sbyte`, `ushort`, `uint` and `ulong` accessors on `FlatFileDataReader`.
* Modify `DataTableExtensions.ReadFlatFile` to support merging into a table with an existing schema and data.
* Improve use cases and performance of `DataTableExtensions.WriteFlatFile`.

### Bug Fixes
* The `DataTableExtensions.WriteFlatFile` method threw an exception for schemas including ignored columns.

I wanted to make sure `FlatFileDataReader` can be used to retrieve data of any built-in .NET type.

The `ReadFlatFile` and `WriteFlatFile` extension methods were previously very limited. When reading, `Reset` would be called on the `DataTable`, wiping out any schema and data, which may not be desired/expected. Going forward, `ReadFlatFile` will attempt to reuse an existing schema and upsert the data. For that reason, additional overloads are now provided to match overloads of the [DataTable.Load](https://docs.microsoft.com/en-us/dotnet/api/system.data.datatable.load?view=netframework-4.7.2) method.

Old versions of `WriteFlatFile` were inefficient, repeatedly getting and setting row values using column names, rather than indexes. Furthermore, now the same underlying array is used to hold values between writes, so an entire file can be written using a single array allocation, improving performance dramatically. Most importantly, the previous version of this method threw errors when schemas included ignored columns.

## 4.2.0 (2018-07-15)
**Summary** - Expose `DateTimeOffset` and `TimeSpan` accessors on `FlatFileDataReader`.

### New Features
* Expose `DateTimeOffset` and `TimeSpan` accessors on `FlatFileDataReader`.

## 4.1.0 (2018-07-15)
**Summary** - Support for `TimeSpan` columns and properties.

### New Features
* Support for `TimeSpan` columns and properties. Ability to treat days, hours, minutes, seconds, milliseconds or ticks as `TimeSpan`.

## 4.0.0 (2018-07-15)
**Summary** - Column-level error handling and further support for nulls.

### New Features
* Errors can now be inspected at the column-level using the `ColumnError` events. 
* Columns can be marked at nullable or not using the `IsNullable` property.
* A default value can be returned when nulls are encountered when `IsNullable = false`.
* Support for `DateTimeOffset` columns and properties.

### Breaking Changes
* The `Error` event of the `IReader` and `ITypedReader` interfaces were renamed to `RecordError`.
* `INullHandler` renamed to `INullFormatter`. Related properties renamed, as well.

## 3.0.1 (2018-07-08)
**Summary** - The `DataRecordExtensions.GetNullableString` calling itself recursively.

### Bug Fixes
* The `DataRecordExtensions.GetNullableString` method was calling itself, causing a `StackOverflowException`.

## 3.0.0 (2018-06-29)
**Summary** - Introducing custom mapping support and more contextual information.

### New Features
* The new type mapper method, `CustomMapping`, grants full control over the way values are mapped between raw `object[]` values and entities. See the [readme](https://github.com/jehugaleahsa/FlatFiles/blob/master/README.md#custom-mapping).
* Automatic column-to-property mapping for delimited file formats, via `GetAutoMappedReader` and `GetAutoMappedWriter` methods. See the [readme](https://github.com/jehugaleahsa/FlatFiles/blob/master/README.md#automatic-mapping-for-delimited-files).

### Enhancements
* All exceptions, events and custom mapping features now provide access to column, record and/or execution context.
* Fewer restrictions on the number of environments FlatFiles can run.
* Support for ADO.NET utilities for .NET Standard 2.0 and above (`IDataReader`/`IDataRecord`).

### Breaking Changes
* The `CustomProperty` and `WriteOnlyProperty` methods have been superseded by the `CustomMapping` method, so they have been removed.
* The `IProcessMetadata` interface has been replaced by the `IRecordContext` interface.
* The `RecordNumber` property of `RecordProcessingException` has been replaced with the `RecordContext` property.
* The `ProcessingErrorEventArgs` class has been replaced by the `ExecutionErrorEventArgs` class.
* The `IncludeFilteredRecords` property of `RecordNumberColumn` has been renamed to `IncludeSkippedRecords`.
* The `IColumnDefinition` interface methods `Parse` and `Format` now accept `IColumnContext` objects.
* The `IMetadataColumn` interface no longer has the `GetValue` method. Use the `MetadataColumn` base class instead. See the updated [readme](https://github.com/jehugaleahsa/FlatFiles/blob/master/README.md#metadata).
* Rename `FlatFileReader` to `FlatFileDataReader`.

Most significantly of all, previous versions of FlatFiles used `DynamicMethod` to generate code at runtime. A `DynamicMethod` can be configured to allow the generated code to access non-public classes and members from other assemblies. However, this additional access requires the code to be running in a trusted environment, meaning FlatFiles could not be used in a sandboxed environment.

The new custom mapping functionality required the creation of types at runtime, so `DynamicMethod` was no longer an option. However, there is no means of granting dynamic types access to non-public classes/members in other assemblies. Going forward, FlatFiles will only be able to access public classes and members in your projects. If you need FlatFiles to access internal classes/members, you can add this line to you `Assembly.cs` file:

```csharp
[assembly: InternalsVisibleTo("FlatFiles.DynamicAssembly,PublicKey=00240000048000009400000006020000002400005253413100040000010001009b9e44f637b293021ec4d8625071e5fe1682eeb167c233b46314cca79bf2769606285d5d1225cba8ce1e75be9e8ab7251d17eaf2c3b00fde5eac50a0f7dc7fec2f70279ff71c72341ad2738661babfdc6792479f14fd64d841285644d5c09c2902e9467f574e0d369161caee632087c5d819c3c36f76622306b09a4f868230c1")]
```

Otherwise, you can disable runtime optimization by calling `OptimizeMapping(false)` on your mapping, which will cause FlatFiles to fallback on reflection which can access private members at the cost of runtime overhead. Another alternative is to pass a delegate that accesses the internal member to the `CustomMapping` method.

Forcing users to add the `[InternalsVisibleTo]` attribute is in-line with what other .NET libraries involving runtime generation of types are doing (e.g., Moq and Castle.DynamicProxy). While this is may be inconvenient to some users, it makes the library more portable. It also mean, FlatFiles no longer depends on the [System.Reflection.Emit.Lightweight](https://www.nuget.org/packages/System.Reflection.Emit.Lightweight) NuGet package which is now considered [obsolete](https://github.com/dotnet/source-build/issues/532). You can read more in the [readme](https://github.com/jehugaleahsa/FlatFiles/blob/master/README.md#accessing-non-public-classes-and-members).

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
var mapper = SeparatedValueTypeMapper.Define(() => new Person());
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
