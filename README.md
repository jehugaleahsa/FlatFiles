# FlatFiles

Read and write CSV, fixed-length and other flat file formats.

Download using NuGet: [FlatFiles](http://nuget.org/packages/FlatFiles)

## Backward Compatibility
As of version 0.2.0.0, FlatFiles is no longer backward compatible. Read the [wiki](https://github.com/jehugaleahsa/FlatFiles/wiki/FlatFiles-0.2.0.0-and-0.3.0.0-Not-Backward-Compatible) to learn more.

## Overview
A lot of us still need to work with flat files (e.g. CSV, fixed-length, etc.) either because we're interfacing with older systems or because we're running one-time migration scripts. As common as these legacy file formats are, it's surprising there's nothing built-in to .NET for handling them. Worse, there's no standard format. There's a general need for a simple library that can be customized to your needs.

FlatFiles makes it easy to read and write flat files in many different ways. It supports type mappers for directly reading and writing with data objects. It even has support for `DataTable`s and `IDataReader` if you need to interface with ADO.NET classes. If you really want to, you can read and write values with raw `object` arrays.

Unlike many other libraries, FlatFiles also allows you to precisely define a file's schema. This allows you to control how FlatFiles interprets columnx with fine-grain precision. It gives you complete control over the way dates, numbers and enums are interpreted in your files.

## Type Mappers
Using the type mappers, you can directly read file contents into your classes:

    var mapper = SeparatedValueTypeMapper.Define<Customer>();
    mapper.Property(c => c.CustomerId).ColumnName("customer_id");
    mapper.Property(c => c.Name).ColumnName("name");
    mapper.Property(c => c.Created).ColumnName("created").InputFormat("yyyyMMdd");
    mapper.Property(c => c.AverageSales).ColumnName("avg_sales");
    using (StreamReader reader = new StreamReader(File.OpenRead(@"C:\path\to\file.csv")))
    {
    	var customers = mapper.Read(reader).ToList();
    }
	
Writing to a file is just as easily:

    mapper.Property(c => c.Created).OutputFormat("yyyyMMdd");
    mapper.Property(c => c.AverageSales).OutputFormat("N2");
    using (StreamWriter writer = new StreamWriter(File.OpenCreate(@"C:\path\to\file2.csv")))
    {
    	mapper.Write(writer, customers);
    }

Call `Property` in the order that the fields appear in the file to define the schema. The type of the column is determined by the type of the mapped property. Each property configuration provides options for controlling the way FlatFiles handles strings, numbers, date/times, GUIDs, enums and more. Once the properties are configured, you can call `Read` or `Write` on the type mapper.
    
*Note* The `Read` method only retrieves records from the underlying file on-demand. To bring the entire file into memory at once, just call `ToList` or `ToArray`, or loop over the records inside of a `foreach`.

*Note* I was able to customize the `OutputFormat` of properties that were already configured. The first time `Property` is called on a property, it assumes it's the next column to appear in the flat file. However, subsequent configuration on the property doesn't change the order of the columns.
	
## Schemas
One of the features that sets FlatFiles apart are schemas. Schemas provide complete control over the way values are interpreted. Under the hood, type mapping internally defines a schema, giving each column a name, order and type in the flat file. You can get access to the schema by calling `GetSchema` on the mapper.

However, you can work directly with schemas if you don't plan on using the type mappers. For instance, this is how we would define a CSV file schema:

    SeparatedValueSchema schema = new SeparatedValueSchema();
    schema.AddColumn(new Int64Column("customer_id"))
          .AddColumn(new StringColumn("name"))
          .AddColumn(new DateTimeColumn("created") { InputFormat = "yyyyMMdd", OutputFormat = "yyyyMMdd" })
          .AddColumn(new DoubleColumn("avg_sales") { OutputFormat = "N2" });
		  
Or, if the schema is for a fixed-length file:

    FixedLengthSchema schema = new FixedLengthSchema();
    schema.AddColumn(new Int64Column("customer_id"), 10)
      .AddColumn(new StringColumn("name"), 255)
      .AddColumn(new DateTimeColumn("created") { InputFormat = "yyyyMMdd", OutputFormat = "yyyyMMdd" }, 8)
      .AddColumn(new DoubleColumn("avg_sales") { OutputFormat = "N2" }, 10);
	  
The `FixedLengthSchema` class is the same as the `SeparatedValueSchema` class, except it associates a `Window` to each column. A `Window` records the `Width` of the column in the file. It also allows you to specify the `Alignment` (left or right) in cases where the value doesn't fill the entire width of the column (the default is left aligned). The `FillCharacter` property can be used to say what character is used as padding. You can also set the `TruncationPolicy` to say whether to chop off the front or the end of values that exceed their width. These settings can also be overridden at the column level.

*Note* Some fixed-length files may have columns that are not used. The fixed-length schema doesn't provide a way to specify a starting index for a column. Simply define "ignored" columns for gaps in the input file.

## SeparatedValueReader
If you are working with delimited files, such as comma-separated (CSV) or tab-separated (TSV) files, you will want to use the `SeparatedValueReader` class. The constructor accepts a combination of a `TextReader`, a `SeparatedValueSchema` object and/or a `SeparatedValueOptions` object.

When parsing separated value files, you can surround fields with double quotes (`"`). This way you can include the separator string within the field. You can override the quote character in the `SeparatedValueOptions` class, if needed. The `SeparatedValueOptions` class supports a `Separator` property for specifying the string that separates your fields. A comma (`,`) is the default separator.

The `RecordSeparator` property specifies what character sequence is used to separate records. By default, this is `Environment.NewLine` (`\r\n`). This is useful if you are working on files from other systems, such as Linux (`\n`) or Macintosh (`\r`).

The `IsFirstRecordSchema` property tells the reader to treat the first record in the file as the schema. Since the types of the fields cannot be determined from a file, they are assumed to be `string`s. If you provide the schema to the constructor, it will be used instead, and the first record will simply be skipped. By default, this property is set to `false`.

## SeparateValueWriter
If you want to build a delimited file, you can use the `SeparatedValueWriter` class. It accepts the same schema and options arguments. If the `SeparatedValueOptions`'s `IsFirstRecordSchema` property is set to `true`, the schema will be written to the file upon writing the first record.

## FixedLengthReader
If you are working with files whose fields are a fixed-length you will want to use the `FixedLengthReader` class. The constructor accepts a combination of a `TextReader`, a `FixedLengthSchema` object and/or a `FixedLengthOptions` object.

The `FixedLengthOptions` class supports a `FillCharacter` property to specify which character is used as a fill character in the columns. A space (` `) is the default. You can also override this on a per-column basis.

It also supports a `RecordSeparator` property for specifying what value indicates the end of a record. By default, this is `Environment.NewLine` (`\r\n`). This is useful if you are working on files from other systems, such as Linux (`\n`) or Macintosh (`\r`).

## FixedLengthWriter
If you want to build a fixed-length file, you can use the `FixedLengthWriter` class. It accepts the same schema and options arguments used to read files. If you want to control the alignment of the columns, you can specify the `FixedAlignment` for each column when defining the schema. This will control whether padding is put to the right or the left of the value.

## Handling Nulls
By default, FlatFiles will treat blank or empty strings as `null`. If `null`s are represented differently in your file, you can pass a custom `INullHandler` to the schema. If it is a fixed value, you can use the `ConstantNullHandler` class.

    DateTimeColumn dtColumn = new DateTimeColumn("created") { NullHandler = ConstantNullHandler.For("NULL") };
    
Or, if you are using Type Mappers, you can simply use the `NullValue` or `NullHandler` methods.

    mapper.Property(c => c.Created).ColumnName("created").NullValue("NULL");
    
You can implement the `INullHandler` interface if you need to support something more complex.

## DataTables
If you are using `DataTable`s, you can read and write to a `DataTable` using the `ReadFlatFile` and `WriteFlatFile` extension methods. Just pass the corresponding reader or writer object.

    DataTable customerTable = new DataTable("Customer");
    using (StreamReader streamReader = new StreamReader(File.OpenRead(@"C:\path\to\file.csv")))
    {
        IReader reader = new SeparatedValueReader(streamReader, schema);
        customerTable.ReadFlatFile(reader);
    }

## FlatFileReader
For low-level ADO.NET file reading, you can use the `FlatFileReader` class. It provides an `IDataReader` interface to the records in the file, making it compatible with other ADO.NET interfaces.

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
