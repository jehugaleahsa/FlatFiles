# FlatFiles

Read and write CSV, fixed-length and other flat file formats.

Download using NuGet: [FlatFiles](http://nuget.org/packages/FlatFiles)

## Support for .NET Core
As of version 0.3.18.0, FlatFiles now supports .NET Core (.NETStandard v1.6) and .NET 4.5.1!

## Performance Tuning & Field Support
As a bonus for those of you using FlatFiles, as of version 1.2, you should now be seeing a nearly 2x performance improvement over previous versions, both read and write, both CSV and fixed-length. I spent some time playing with BenchmarkDotNet and using the profiler to squeeze every ounce of performance out of FlatFiles.

You can also now map to both properties and fields, even intermingled in the same class, using the same `Property` methods you already know and love.

## Async Support
As of version 1.3, you can now read and write files asynchronously!

## Overview
A lot of us still need to work with flat files (e.g. CSV, fixed-length, etc.) either because we're interfacing with older systems or because we're running one-time migration scripts. As common as these legacy file formats are, it's surprising there's nothing built-in to .NET for handling them. Worse, it seems like each system has its own little quirks. People have a pretty easy time reading most flat file formats but we as developers spend an enormous amount of time tweaking our code to handle every oddball edge case.

FlatFiles focuses on schema configuration, allowing you to very clearly define the schema of your file, whether reading or writing. It gives complete control over the way dates, numbers, enums, etc. are interpreted in your files. This means less time chasing down edge cases and dealing with conversion issues.

FlatFiles makes it easy to work with flat files in many different ways. It supports type mappers for directly reading and writing with data objects. It even has support for `DataTable`s and `IDataReader` if you need to interface with ADO.NET classes. If you really want to, you can read and write values using raw `object` arrays.

## Type Mappers
Using the type mappers, you can directly read file contents into your classes:

```csharp
var mapper = SeparatedValueTypeMapper.Define<Customer>();
mapper.Property(c => c.CustomerId).ColumnName("customer_id");
mapper.Property(c => c.Name).ColumnName("name");
mapper.Property(c => c.Created).ColumnName("created").InputFormat("yyyyMMdd");
mapper.Property(c => c.AverageSales).ColumnName("avg_sales");
using (StreamReader reader = new StreamReader(File.OpenRead(@"C:\path\to\file.csv")))
{
    var customers = mapper.Read(reader).ToList();
}
```

To define the schema when working with type mappers, call `Property` in the order that the fields appear in the file. The type of the column is determined by the type of the mapped property. Each property configuration provides options for controlling the way FlatFiles handles strings, numbers, date/times, GUIDs, enums and more. Once the properties are configured, you can call `Read` or `Write` on the type mapper.

*Note* The `Read` method only retrieves records from the underlying file on-demand. To bring the entire file into memory at once, just call `ToList` or `ToArray`, or loop over the records inside of a `foreach`.

Writing to a file is just as easily:

```csharp
mapper.Property(c => c.Created).OutputFormat("yyyyMMdd");
mapper.Property(c => c.AverageSales).OutputFormat("N2");
using (StreamWriter writer = new StreamWriter(File.OpenCreate(@"C:\path\to\file2.csv")))
{
    mapper.Write(writer, customers);
}
```

*Note* I was able to customize the `OutputFormat` of properties that were already configured. The first time `Property` is called on a property, it assumes it's the next column to appear in the flat file. However, subsequent configuration on the property doesn't change the order of the columns or reset any other settings.

## Schemas
Under the hood, type mapping internally defines a schema, giving each column a name, order and type in the flat file. You can get access to the schema by calling `GetSchema` on the mapper.

You can work directly with schemas if you don't plan on using the type mappers. For instance, this is how we would define a CSV file schema:

```csharp
SeparatedValueSchema schema = new SeparatedValueSchema();
schema.AddColumn(new Int64Column("customer_id"))
      .AddColumn(new StringColumn("name"))
      .AddColumn(new DateTimeColumn("created") { InputFormat = "yyyyMMdd", OutputFormat = "yyyyMMdd" })
      .AddColumn(new DoubleColumn("avg_sales") { OutputFormat = "N2" });
```

Or, if the schema is for a fixed-length file:

```csharp
FixedLengthSchema schema = new FixedLengthSchema();
schema.AddColumn(new Int64Column("customer_id"), 10)
  .AddColumn(new StringColumn("name"), 255)
  .AddColumn(new DateTimeColumn("created") { InputFormat = "yyyyMMdd", OutputFormat = "yyyyMMdd" }, 8)
  .AddColumn(new DoubleColumn("avg_sales") { OutputFormat = "N2" }, 10);
```

The `FixedLengthSchema` class is the same as the `SeparatedValueSchema` class, except it associates a `Window` to each column. A `Window` records the `Width` of the column in the file. It also allows you to specify the `Alignment` (left or right) in cases where the value doesn't fill the entire width of the column (the default is left aligned). The `FillCharacter` property can be used to say what character is used as padding. You can also set the `TruncationPolicy` to say whether to chop off the front or the end of values that exceed their width.

*Note* Some fixed-length files may have columns that are not used. The fixed-length schema doesn't provide a way to specify a starting index for a column. Look at the **Ignored Fields** section below to learn about ways to to handle this.

## Separated Value Files (CSV, TSV, etc.)
If you are working with delimited files, such as comma-separated (CSV) or tab-separated (TSV) files, you want to use the `SeparatedValueTypeMapper`. Internally, the mapper uses the `SeparatedValueReader` and `SeparatedValueWriter` classes, both of which work in terms of raw `object` arrays. In effect, all the mapper does is map the values in the array to the properties in your data objects. These classes read data from a `TextReader`, such as a `StreamReader` or a `StringReader`, and write data to a `TextWriter`, such as a `StreamWriter` or a `StringWriter`. Internally, the mapper will build a `SeparatedValueSchema` based on the property/column configuration; this is where you customize the schema to match your file format. For more global settings, there is also a `SeparatedValueOptions` object that allows you to customize the read/write behavior to suit your needs.

When parsing separated value files, you can surround fields with double quotes (`"`). This way you can include the separator string within the field. You can override the "quote" character in the `SeparatedValueOptions` class, if needed. The `SeparatedValueOptions` class supports a `Separator` property for specifying the string/character that separates your fields. A comma (`,`) is the default separator; you'd obviously want to change this to tab (`\t`) for a TSV file.

The `RecordSeparator` property specifies what string/character is used to separate records. By default, FlatFiles will look for `\r`, `\n` or `\r\n`, which are the default line separators for Mac, Linux and Windows, respectively. When writing files, `Environment.NewLine` is used by default; this means by default you'll get different output if you run the same code on different platforms. If you need to target a specific platform, be sure to set the `RecordSeparator` property explicitly.

When working directly with the `SeparatedValueReader` class, the `IsFirstRecordSchema` option tells the reader to treat the first record in the file as the schema. This is useful when you don't know the schema ahead of time or you are just dumping files into staging tables. If you provide a schema, this setting tells the reader to simply skip the first record. If you are using it to determine the schema from the file, the reader treats every column as a string - it doesn't try to interpret the type from the data. While occasionally useful, you will normally want to provide a schema to give you the most control over the parsing process - that's what FlatFiles is good at!

When working directly with the `SeparatedValueWriter` class, setting `IsFirstRecordSchema` to `true` option causes a header to be written to the file upon writing the first record.

## Fixed Length Files
If you have a file with fixed length columns, you will want to use the `FixedLengthTypeMapper` class. Internally, the mapper uses the `FixedLengthReader` and `FixedLengthWriter` classes, both of which work in terms of raw `object` arrays. In effect, all the mapper does is map the values in the array to the properties in your data objects. These classes read data from a `TextReader`, such as `StreamReader` or `StringReader`, and write data to a `TextWriter`, such as `StreamWriter` or `StringWriter`. Internally, the mapper will build a `FixedLengthSchema` based on the property/column configuration; this is where you customize the schema to match your file format. For more global settings, there is also a `FixedLengthOptions` object that allows you to customize the read/write behavior to suit your needs.

Since each column has a fixed length, FlatFiles provides configuration options to specify how to handle values that are too short or too long: `FillCharacter`, `Alignment` and `TruncationPolicy`. `FillCharacter` specifies what character is used to pad values on the left or the right, using space (` `) by default. You can configure whether the padding should go to the left or the right using the `Alignment` property, putting padding to the right by default (`LeftAligned`). `TruncationPolicy` tells FlatFiles how to crop values that exceed the width of their column when writing out to a file, removing leading characters by default. These options can be specified globally in the `FixedLengthOptions` object or overridden at the column level using the `Window` object.

The `RecordSeparator` property specifies what string/character is used to separate records. By default, FlatFiles will look for `\r`, `\n` or `\r\n`, which are the default line separators for Mac, Linux and Windows, respectively. When writing files, `Environment.NewLine` is used by default; this means by default you'll get different output if you run the same code on different platforms. If you need to target a specific platform, be sure to set the `RecordSeparator` property explicitly. 

By default, FlatFiles assumes there is a separator string/character between each record.  If you set the `HasRecordSeparator` to `false`, FlatFiles will read the next record immediately following the last character of the previous record. When writing, it will not insert a separator, writing immediately after the last character of the previous record.

If the `FixedLengthOptions`'s `IsFirstRecordHeader` property is set to `true`, the first record in the file will be skipped when reading. Unlike the `SeparatedValueReader`, you must *always provide a schema for fixed-length files*, since the width of the columns cannot be determined from the file format. When writing, a header will be written to the file upon writing the first record.

## Handling Nulls
By default, FlatFiles will treat blank or empty strings as `null`. If `null`s are represented differently in your file, you can pass a custom `INullHandler` to the schema. If it is a fixed value, you can use the `ConstantNullHandler` class.

```csharp
DateTimeColumn dtColumn = new DateTimeColumn("created") { NullHandler = ConstantNullHandler.For("NULL") };
```

Or, if you are using Type Mappers, you can simply use the `NullValue` or `NullHandler` methods.

```csharp
mapper.Property(c => c.Created).ColumnName("created").NullValue("NULL");
```

You can implement the `INullHandler` interface if you need to support something more complex.

## Ignored Fields
Most of the time, we don't have control over the flat file we're working with. This usually leads to columns that our code just doesn't care about. Both type mappers and schemas expect columns to be listed in the order they appear in the document. For type mappers, this would mean defining properties in your classes that you'd never use (e.g., Ignored1, Ignored2, etc.). Another common problem with fixed-length files is that they will separate fields with pipes (`|`), or other characters, even though they are fixed-length.

Types mappers support the `Ignored` method that will tell FlatFiles to simply ignore the next column in the file. Unlike other mapper methods, it does not take a property, since the value is simply thrown away. You can optionally call `ColumnName` if you want to provide a column name when writing out schemas.

When working with the `IFixedLengthTypeMapper`, `Ignored` takes a `Window`. This is a great way to skip unused sections within the document. You can even set the `FillCharacter` property to insert pipes (`|`), or another character, between fields.

Under the hood, type mappers is adding an `IgnoredColumn` to the underlying schema. `IgnoredColumn` has a constructor to optionally specify a column name. `IgnoredColumn` affects the way you work with readers and writers. The readers will initially retrieve all columns from the document and then throw away any ignored values. You'll only see values for the columns that aren't ignored. Also, you do not need to provide values to the writers for ignored columns; the schema will automatically take care of writing out blanks for them. From a development perspective, it's as if those columns didn't exist in the underlying document.

## Skipping Records
If you work directly with `SeparatedValueReader` or `FixedLengthReader`, you can call `Skip` to arbitrarily skip records in the input file. However, you often need the ability to inspect the record to determine whether it needs skipped. However, what if you are trying to skip records *because* they can't be parsed? If you need more control over what records to skip, FlatFiles provides options to inspect records during the parsing process. These options work the same whether you use type mappers or directly with readers.

Parsing a record goes through the following life-cycle:
1) Read text until a record terminator (usually a newline) is found.
2) For fixed-length records, partition the text into string columns based on the configured windows.
3) Converting the string columns to the designated column types, as defined in the schema.

For CSV files, breaking a record into columns is automatically performed while searching for the record terminator. Prior to trying to convert the text to ints, date/times, etc., FlatFiles provides you the opportunity to inspect the raw string values and skip records.

Within the `SeparatedValueOptions` class, there is a `Func<string[], bool> PartitionedRecordFilter` property, allowing you to provide a custom filter to skip unwanted records. For example, you could use the code below to find and skip CSV records missing the necessary number of columns:

```csharp
var options = new SeparatedValueOptions()
{
    PartitionedRecordFilter = (values) => values.Length < 10
};
```

Fixed-length files come in two flavors: those with and without record terminators. If there is no record terminator, the assumption is *all records are the same length*. Otherwise, each record can be a different length. For that reason, FlatFiles provides an extra opportunity to filter out records prior to splitting the text into columns. This is useful for filtering out records not meeting a minimum length requirement or those using a character to indicate something like comments.

Within the `FixedLengthOptions` class, you can use the `Func<string, bool> UnpartitionedRecordFilter` property, allowing you to provide a custom filter to skip unwanted records. For example, you could use the code below to find and skip records starting with a `#` symbol:

```csharp
var options = new FixedLengthOptions()
{
    UnpartitionedRecordFilter = (record) => record.StartsWith("#")
};
```

Similar to CSV files, you can also filter out fixed-length records *after* they are broken into columns. However, it is important to note that the record is expected to fit the configured windows.  You could use this filter to skip records whose values cannot be parsed.

Within the `FixedLengthOptions` class, you can use the `Func<string[], bool> PartitionedRecordFilter` property, allowing you to provide a custom filter to skip unwanted records. For example, you could use the code below to find and skip records whose third column has a flag:

```csharp
var options = new FixedLengthOptions()
{
    PartitionedRecordFilter = (values) => values[2] == "ERROR"
};
```

## Runtime Types and Support for Other Programming Languages
Even if you don't know the type of a class at compile time, it can still be beneficial to use the type mappers to populate these objects from a file. Or, if you are working in a language without support for expression trees, you'll be glad to know FlatFiles provides an alternative way to configure type mappers.

The code below illustrates how you could define a mapper for a type that is only known at runtime:

```csharp
// Assume there is a runtime-generated type called "entityType" and a "TextReader".
var mapper = SeparatedValueMapper.DefineDynamic(entityType);
mapper.Int32Property("Id");
mapper.StringProperty("Name");
var entities = mapper.Read(reader).ToArray();
// Do something with the entities.
```

## Disabling Optimization
FlatFile's type mappers can serialize and deserialize extremely quickly by generating code at runtime, using classes in the  `System.Reflection.Emit` namespace. For most of us, that's awesome news because it means mapping values to and from your entities is almost as fast as if you had done the mapping by hand. However, there are some enviornments, like Mono running on iOS, that do not support runtime JIT'ing, so FlatFiles would not work.

As of version 1.0, mappers support a new method `OptimizeMapping` that can be used to switch to normal (A.K.A., slow) reflection. For example:

```csharp
var mapper = SeparatedValueTypeMapper.Define<Person>(() => new Person());
mapper.Property(x => x.Id);
mapper.Property(x => x.Name);
mapper.OptimizeMapping(false);  // Use normal reflection to get and set properties
```

## DataTables
If you are using `DataTable`s, you can read and write to a `DataTable` using the `ReadFlatFile` and `WriteFlatFile` extension methods. Just pass the corresponding reader or writer object.

```csharp
DataTable customerTable = new DataTable("Customer");
using (StreamReader streamReader = new StreamReader(File.OpenRead(@"C:\path\to\file.csv")))
{
    IReader reader = new SeparatedValueReader(streamReader, schema);
    customerTable.ReadFlatFile(reader);
}
```

## FlatFileReader
For low-level ADO.NET file reading, you can use the `FlatFileReader` class. It provides an `IDataReader` interface to the records in the file, making it compatible with other ADO.NET interfaces.

```csharp
// The DataReader Approach
FlatFileReader reader = new FlatFileReader(new SeparatedValueReader(@"C:\path\to\file.csv", schema);
List<Customer> customers = new List<Customer>();
while (reader.Read())
{
    Customer customer = new Customer();
    customer.CustomerId = reader.GetInt32(0);
    customer.Name = reader.GetString(1);
    customer.Created = reader.GetDateTime(2);
    customer.AverageSales = reader.GetDouble(3);
    customers.Add(customer);
}
return customers;
```

Usually in cases like this, it is just easier to use the type mappers. However, this could be useful if you are swapping out an actual database call with CSV data inside of a unit test.

## License
This is free and unencumbered software released into the public domain.

Anyone is free to copy, modify, publish, use, compile, sell, or
distribute this software, either in source code form or as a compiled
binary, for any purpose, commercial or non-commercial, and by any
means.

In jurisdictions that recognize copyright laws, the author or authors
of this software dedicate any and all copyright interest in the
software to the public domain. We make this dedication for the benefit
of the public at large and to the detriment of our heirs and
successors. We intend this dedication to be an overt act of
relinquishment in perpetuity of all present and future rights to this
software under copyright law.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

For more information, please refer to <http://unlicense.org>
