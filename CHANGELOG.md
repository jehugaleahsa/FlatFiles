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