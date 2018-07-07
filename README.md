# FlatFiles

Reads and writes CSV, fixed-length and other flat file formats with a focus on schema definition, configuration and speed. Supports mapping directly between files and classes.

Download using NuGet: [FlatFiles](http://nuget.org/packages/FlatFiles)

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
using (var reader = new StreamReader(File.OpenRead(@"C:\path\to\file.csv")))
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
using (var writer = new StreamWriter(File.OpenCreate(@"C:\path\to\file2.csv")))
{
    mapper.Write(writer, customers);
}
```

*Note* I was able to customize the `OutputFormat` of properties that were previously configured. The first time `Property` is called on a property, FlatFiles assumes it's the next column to appear in the flat file. However, subsequent configuration on the property doesn't change the order of the columns or reset any other settings.

### Automatic mapping for delimited files
If your delimited file (CSV, TSV, etc.) has a schema with column names that match your class's property names, you can use the `GetAutoMappedReader` method as a shortcut. This method will read the schema from your file and map the columns to the properties automatically, returning a reader for retrieving the data. It's important to note that you cannot customize the parsing behavior of any of the columns, at which point you are better off explicitly defining the schema. Fortunately, FlatFiles uses pretty liberal parsing out-of-the-box, so most common formats will work.

By default, columns and properties are matched by name (case-insensitive). If you need more control over how columns and properties are matched, you can pass in your own `IAutoMapMatcher`. Given an `IColumnDefinition` and a `MemberInfo`, a matcher must determine whether the two map to one another. For convenience, you can also use the `AutoMapMatcher.For` method to pass a `Func<IColumnDefinition, MemberInfo, bool>` delegate rather than implement the interface.

Similarly, use the `GetAutoMappedWriter` method to automatically write out a delimited file. Note that there's no way to control the column formatting. However, you can control the name and position of the columns by passing an `IAutoMapResolver`. The `IAutoMapResolver` interface provides the `GetPosition` and a `GetColumnName` methods, both accepting a `MemberInfo`. For convenience, you can also use the `AutoMapResolver.For` method to pass delegates for determining the names/positions, rather than implement the interface. 

## Schemas
Under the hood, type mapping internally defines a schema, giving each column a name, order and type in the flat file. You can get access to the schema by calling `GetSchema` on the mapper.

You can work directly with schemas if you don't plan on using the type mappers. For instance, this is how we would define a CSV file schema:

```csharp
var schema = new SeparatedValueSchema();
schema.AddColumn(new Int64Column("customer_id"))
      .AddColumn(new StringColumn("name"))
      .AddColumn(new DateTimeColumn("created") { InputFormat = "yyyyMMdd", OutputFormat = "yyyyMMdd" })
      .AddColumn(new DoubleColumn("avg_sales") { OutputFormat = "N2" });
```

Or, if the schema is for a fixed-length file:

```csharp
var schema = new FixedLengthSchema();
schema.AddColumn(new Int64Column("customer_id"), 10)
  .AddColumn(new StringColumn("name"), 255)
  .AddColumn(new DateTimeColumn("created") { InputFormat = "yyyyMMdd", OutputFormat = "yyyyMMdd" }, 8)
  .AddColumn(new DoubleColumn("avg_sales") { OutputFormat = "N2" }, 10);
```

The `FixedLengthSchema` class is the same as the `SeparatedValueSchema` class, except it associates a `Window` to each column. A `Window` records the `Width` of the column in the file. It also allows you to specify the `Alignment` (left or right) in cases where the value doesn't fill the entire width of the column (the default is left aligned). The `FillCharacter` property can be used to say what character is used as padding. You can also set the `TruncationPolicy` to say whether to chop off the front or the back of values that exceed their width.

*Note* Some fixed-length files may have columns that are not used. The fixed-length schema doesn't provide a way to specify a starting index for a column. Look at the [Ignored Fields](#Ignored%20Fields) section below to learn about ways to to handle this.

## Separated Value Files (CSV, TSV, etc.)
If you are working with delimited files, such as comma-separated (CSV) or tab-separated (TSV) files, you want to use the `SeparatedValueTypeMapper`. Internally, the mapper uses the `SeparatedValueReader` and `SeparatedValueWriter` classes, both of which work in terms of raw `object` arrays. In effect, all the mapper does is map the values in the array to the properties in your data objects. These classes read data from a `TextReader`, such as a `StreamReader` or a `StringReader`, and write data to a `TextWriter`, such as a `StreamWriter` or a `StringWriter`. Internally, the mapper will build a `SeparatedValueSchema` based on the property/column configuration; this is where you customize the schema to match your file format. For more global settings, there is also a `SeparatedValueOptions` object that allows you to customize the read/write behavior to suit your needs.

Within separated value files, fields can be surrounded with double quotes (`"`). This way they can include the separator within the field. You can override the "quote" character in the `SeparatedValueOptions` class, if needed. The `SeparatedValueOptions` class supports a `Separator` property for specifying the string/character that separates your fields. A comma (`,`) is the default separator; you'd obviously want to change this to tab (`\t`) for a TSV file.

The `RecordSeparator` property specifies what string/character is used to separate records. By default, FlatFiles will look for `\r`, `\n` or `\r\n`, which are the default line separators for Mac, Linux and Windows, respectively. When writing files, `Environment.NewLine` is used by default; this means by default you'll get different output if you run the same code on different platforms. If you need to target a specific platform, be sure to set the `RecordSeparator` property explicitly.

When working directly with the `SeparatedValueReader` class, the `IsFirstRecordSchema` option tells the reader to treat the first record in the file as the schema. This is useful when you don't know the schema ahead of time or you are just dumping files into staging tables. If you provide a schema, this setting tells the reader to simply skip the first record. If you are using it to determine the schema from the file, the reader treats every column as a `string` - it doesn't try to interpret the type from the data. While occasionally useful, you will normally want to provide a schema to give you the most control over the parsing process - that's what FlatFiles is good at!

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

## Metadata
It is often useful to incorporate metadata with the records you are reading from a file. The most common example is tracking a record's line number, so users can be informed where to look in their files when something goes wrong.

Currently, the only out-of-the-box metadata column is `RecordNumberColumn`; however, it's really easy to create your own custom metadata columns (more on that below).

The `RecordNumberColumn` class provides options for controlling how the record number is generated. The `IncludeSchema` property indicates whether the schema or header row should be included in the count. The `IncludeSkippedRecords` property specifies whether to count records that are [skipped](#Skipping_Records).

By default, `RecordNumberColumn` will only count records that are actually returned, starting from `1`, then `2`, `3`, `4`, `5` and so on. If you count the schema, records will always start at `2`. Including the schema and skipped records is what you probably want if you're trying to simulate line number. The only time the record # wouldn't be the same as the line # is if a record spanned multiple lines. Here's an example showing how to capture this *pseudo* line number:

```csharp
var mapper = SeparatedValueTypeMapper.Define(() => new Person());
mapper.Property(x => x.Name);
mapper.CustomMapping(new RecordNumberColumn("RecordNumber")
{
    IncludeSchema = true,
    IncludeSkippedRecords = true
}).WithReader(p => p.RecordNumber);

var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
var results = mapper.Read(reader, options).ToArray();
```

### Write-Only
I've not yet come up with a reason why you'd want to write out metadata, but I provide support for it anyway (feel free to provide me an example!).

```csharp
var mapper = FixedLengthTypeMapper.Define(() => new Person());
mapper.Property(x => x.Name, 10);
mapper.Ignored(1);
// No need to define a reader or a writer - the underlying schema handles writing the metadata 
mapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = true }, 10);
mapper.Ignored(1);
mapper.Property(x => x.CreatedOn, 10).OutputFormat("MM/dd/yyyy");
```

### Creating your own metadata columns
FlatFiles provides the `MetadataColumn<T>` abstract base class to allow you to create your own metadata columns. To implement this interface, you must implement the methods:

```csharp
T OnParse(IColumnContext context);

string OnFormat(IColumnContext context);
```

Within `IColumnContext`, the following information is currently provided:

* `PhysicalIndex` - The index of the column in the file.
* `LogicalIndex` - The index of the column, excluding ignored columns.
* `RecordContext` - Details about the record this column pertains to.
    * `PhysicalRecordNumber` - The actual number of records read from the file.
    * `LogicalRecordNumber` - The number of records that have not been skipped. *This count does not yet include the current record.*
    * `ExecutionContext` - Details about the current read/write operation.
        * `Schema` - The schema being used to parse the file.
        * `Options` - The options passed to the reader/writer.

## Skipping Records
If you work directly with `SeparatedValueReader` or `FixedLengthReader`, you can call `Skip` to arbitrarily skip records in the input file. However, you often need the ability to inspect the record to determine whether it needs skipped. But what if you are trying to skip records *because* they can't be parsed? If you need more control over what records to skip, FlatFiles provides events for inspecting records during the parsing process. These events can be wired up whether you use type mappers or are working directly with readers.

Parsing a record goes through the following life-cycle:
1) Read text until a record terminator (usually a newline) is found.
2) For fixed-length records, partition the text into string columns based on the configured windows.
3) Convert the string columns to the designated column types, as defined in the schema.

For CSV files, breaking a record into columns is automatically performed while searching for the record terminator. Prior to trying to convert the text to ints, date/times, etc., FlatFiles provides you the opportunity to inspect the raw string values and skip records.

The `SeparatedValueReader` class provides a `RecordRead` event, which allows you to skip unwanted records. For example, you could use the code below to find and skip CSV records missing the necessary number of columns:

```csharp
reader.RecordRead += (sender, e) =>
{
    e.IsSkipped = e.Values.Length < 10;
};
```

Fixed-length files come in two flavors: those with and without record terminators. If there is no record terminator, the assumption is *all records are the same length*. Otherwise, each record can be a different length. For that reason, FlatFiles provides an extra opportunity to filter out records prior to splitting the text into columns. This is useful for filtering out records not meeting a minimum length requirement or those using a character to indicate something like comments.

The `FixedLengthReader` class provides the `RecordRead` event, which allows you to skip unwanted records. For example, you could use the code below to find and skip records starting with a `#` symbol:

```csharp
reader.RecordRead += (sender, e) => 
{
    e.IsSkipped = e.Record.StartsWith("#");
};
```

Similar to CSV files, you can also filter out fixed-length records *after* they are broken into columns. However, it is important to note that the record is expected to fit the configured windows.

Again, the `FixedLengthReader` class provides the `RecordPartitioned` event, which allows you to skip unwanted records. For example, you could use the code below to find and skip records whose third column has a flag:

```csharp
reader.RecordPartitioned += (sender, e) => 
{
    e.IsSkipped = e.Values[2] == "ERROR";
};
```

## Files containing multiple schemas
Some flat file formats will contain multiple schemas. Often, data appears within "blocks" with a header and perhaps a footer. It can be extremely useful to parse each type of record using a different schema and return them in the same order they appear in the file.

FlatFiles provides support for these file formats using the `SchemaSelector` and `TypeMapperSelector` classes. Once you define the schemas or type mappers, you can register them with the selector.

```csharp
var selector = new SeparatedValueTypeMapperSelector();
var dataMapper = getDataTypeMapper();
selector.When(x => x.Length == 10).Use(dataMapper);
selector.When(x => x.Length == 2).Use(getHeaderTypeMapper());
selector.When(x => x.Length == 3).Use(getFooterTypeMapper());
selector.WithDefault(dataMapper);
```

Each type mapper is associated with a predicate, which accepts the preprocessed record. If the predicate succeeds, that type mapper will be used to parse the record. Each predicate is tested in the order it is registered, so be sure to make your predicates smart enough to correctly identify the record type.

*Note: You'll might have noticed `dataMapper` is registered twice, once up-front with a specific condition and then as the default. This isn't necessary; however, making sure the most common schema is tried first may help performance in some cases.*

Once your selector is configured, you can call `GetReader`, passing in the `TextReader` and the options object. From there you can register events and read the objects from the file. Since different types can be returned, the reader will return instances of `object`.

If you want to work directly with schemas and readers, you can build a `SchemaSelector` by registering schemas with predicates in a similar fashion. The `SeparatedValueReader` and `FixedLengthReader` classes provide a constructor accepting a `SchemaSelector`. *Note that whenever you work with selectors, calls to `GetSchema` will return `null`.*

```csharp
var selector = new SeparatedValueSchemaSelector();
var recordSchema = getDataSchema();
selector.When(values => values.Length == 10).Use(recordSchema);
selector.When(values => values.Length == 2).Use(getHeaderSchema());
selector.When(values => values.Length == 3).Use(getFooterSchema());
selector.WithDefault(recordSchema);

var reader = new SeparatedValueReader(fileStream, selector);
while (reader.Read())
{
    object[] values = reader.GetValues();
    processRecord(values);
}
```

If you want to *create* multi-schema files, there are "injector" equivalents for each "selector" class. For example:

```csharp
var selector = new FixedLengthTypeMapperInjector();
selector.WithDefault(getRecordTypeMapper());
selector.When<HeaderRecord>().Use(getHeaderTypeMapper());
selector.When<FooterRecord>().Use(getFooterTypeMapper());

var stringWriter = new StringWriter();
var writer = injector.GetWriter(stringWriter);
writer.Write(new HeaderRecord() { BatchName = "First Batch", RecordCount = 2 });
writer.Write(new DataRecord() { Id = 1, Name = "Bob Smith", CreatedOn = new DateTime(2018, 06, 04), TotalAmount = 12.34m });
writer.Write(new DataRecord() { Id = 2, Name = "Jane Doe", CreatedOn = new DateTime(2018, 06, 05), TotalAmount = 34.56m });
writer.Write(new FooterRecord() { TotalAmount = 46.9m, AverageAmount = 23.45m, IsCriteriaMet = true });
```

The `When` method accepts a predicate if you need more than just the record type to decide what type mapper/schema to use.

## Custom Mapping
The `Property` methods are best when your file's schema pretty much matches one-to-one with your classes. One column goes to one property. Frequently, though, your classes are more structured than your flat files. Starting with FlatFiles 3.0, you can provide your own mapping logic to control serializing and deserializing your objects.

First, let's see how to use the `CustomMapping` method to simulate the `Property` methods:

```csharp
var mapper = SeparatedValueTypeMapper.Define(() => new Person());
mapper.CustomMapping(new StringColumn("FirstName"))
    .WithReader(p => p.FirstName)
    .WithWriter(p => p.FirstName);
```

This mapping tells FlatFiles to use a `StringColumm` to read/write the values from the file. It then says to read the values into and write out of the `FirstName` property of your class.

It's important to note that the type returned by the `IColumnDefinition` must exactly match the type of the property. You must deal with `null`s and conversions yourself. There are several other overloads that provide more control -- this particular example would be better handled sticking to the `Property` methods.

Here is a more complicated example where two columns are used to build a `Geolocation` property.

```csharp
public class Geolocation { public decimal Longitude; public decimal Latitude; }
public class Person { public Geolocation Location { get; set; } }
//...
var mapper = SeparatedValueTypeMapper.Define(() => new Person() 
{ 
    Location = new Geolocation() 
});
mapper.CustomMapping(new DecimalColumn("Longitude"))
    .WithReader((p, v) => p.Location.Longitude = (decimal)v)  // long way
    .WithWriter(p => p.Location.Longitude);
mapper.CustomMapping(new DecimalColumn("Latitude"))
    .WithReader(p => p.Location.Latitude)  // short way
    .WithWriter(p => p.Location.Latitude);
```

Finally, here is an example configuration, where multiple columns are stored in a collection.

```csharp
public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<string> PhoneNumbers { get; set; } = new List<string>();
}
//...
var mapper = FixedLengthTypeMapper.Define(() => new Contact());
mapper.CustomMapping(new Int32Column("Id"), 10)
    .WithReader(c => c.Id)
    .WithWriter(c => c.Id);
mapper.CustomMapping(new StringColumn("Name"), 10)
    .WithReader(c => c.Name)
    .WithWriter(c => c.Name);
mapper.CustomMapping(new StringColumn("Phone1"), 12)
    .WithReader(PhoneReader)
    .WithWriter(c => c.PhoneNumbers.Count > 0 ? c.PhoneNumbers[0] : null);
mapper.CustomMapping(new StringColumn("Phone2"), 12)
    .WithReader(PhoneReader)
    .WithWriter(c => c.PhoneNumbers.Count > 1 ? c.PhoneNumbers[1] : null);
//...
public void PhoneReader(Contact contact, string phoneNumber)
{
    if (phoneNumber != null)
    {
        contact.PhoneNumbers.Add(phoneNumber);
    }
}
```

In benchmarks, using `CustomMapping` is only slightly slower than using `Property`, making it a great option when you need a little extra control. 

There are versions of `WithReader` and `WithWriter` that provide contextual information (`IColumnContext`), so you can access metadata while reading and writing. The `WithWriter` method also provides an overload passing along the underlying array being written to, so you can write to multiple columns simultaneously or inspect previously serialized values.

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

As of version 1.0, mappers support a new method `OptimizeMapping` that can be used to switch to (A.K.A., slow) reflection. For example:

```csharp
var mapper = SeparatedValueTypeMapper.Define<Person>(() => new Person());
mapper.Property(x => x.Id);
mapper.Property(x => x.Name);
mapper.OptimizeMapping(false);  // Use normal reflection to get and set properties
```

## Accessing Non-Public Classes and Members
As of FlatFiles 3.0, you can no longer map to non-public classes and members (aka., `internal`, `protected` or `private`) without taking additional steps. The simplest solution is to make your classes and members `public`. Alternatively, you can [disable optimizations](#Disabling%20Optimization) which will cause FlatFiles to use normal reflection, which should be able to access anything, at the cost of some runtime overhead.

Another option is to grant FlatFiles access to your `internal` classes and members by adding the following line to you `Assembly.cs` file:

```csharp
[assembly: InternalsVisibleTo("FlatFiles.DynamicAssembly,PublicKey=00240000048000009400000006020000002400005253413100040000010001009b9e44f637b293021ec4d8625071e5fe1682eeb167c233b46314cca79bf2769606285d5d1225cba8ce1e75be9e8ab7251d17eaf2c3b00fde5eac50a0f7dc7fec2f70279ff71c72341ad2738661babfdc6792479f14fd64d841285644d5c09c2902e9467f574e0d369161caee632087c5d819c3c36f76622306b09a4f868230c1")]
```

Notice that this only grants access to `internal` types/members. You will still not be able to access `private` members.

As a final option, you can use the `CustomMapping` method, passing delegates to read/write your members. Since the delegates are part of your project, they can access non-public members without trouble. There is almost no runtime overhead using `CustomMapping` instead of `Property`.


## DataTables
If you are using `DataTable`s, you can read and write to a `DataTable` using the `ReadFlatFile` and `WriteFlatFile` extension methods. Just pass the corresponding reader or writer object.

```csharp
var customerTable = new DataTable("Customer");
using (var streamReader = new StreamReader(File.OpenRead(@"C:\path\to\file.csv")))
{
    var reader = new SeparatedValueReader(streamReader, schema);
    customerTable.ReadFlatFile(reader);
}
```

## FlatFileDataReader
For low-level ADO.NET file reading, you can use the `FlatFileDataReader` class. It provides an `IDataReader` interface to the records in the file, making it compatible with other ADO.NET interfaces.

```csharp
// The DataReader Approach
using (var fileReader = new StreamReader(File.OpenRead(@"C:\path\to\file.csv"))
{
    var csvReader = new SeparatedValueReader(fileReader, schema);
    var dataReader = new FlatFileDataReader(csvReader);
    var customers = new List<Customer>();
    while (dataReader.Read())
    {
        var customer = new Customer();
        customer.CustomerId = dataReader.GetInt32(0);
        customer.Name = dataReader.GetString(1);
        customer.Created = dataReader.GetDateTime(2);
        customer.AverageSales = dataReader.GetDouble(3);
        customers.Add(customer);
    }
    return customers;
}
```

Usually in cases like this, it is just easier to use the type mappers. However, this could be useful if you are swapping out an actual database call with CSV data inside of a unit test.

FlatFiles also provides helpful extension methods on the `IDataReader` interface to make it easier to extract data. It provides `GetNullable*` variants of the `IDataReader` methods, so you don't need to constantly call `IsDBNull`. There are also variants of each method accepting the column name rather than the ordinal position.

There are also generic `GetValue<T>` methods that can deal with type conversions automatically for you. For example, anytime you read in a CSV file without providing the schema, FlatFiles assumes each column is a `string`. When calling `GetValue<int>("Id")` or `GetValue<DateTime?>("CreatedOn")`, FlatFiles will try to convert the values for you. This is extremely helpful when you don't want to provide (*or can't provide*) a schema but can still determine the column types. For example, when you know the column names and their types but their order could be different between runs.

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
