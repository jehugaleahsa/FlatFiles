# FlatFiles

Work with flat files using the ADO.NET classes.

Download using NuGet: [FlatFiles](http://nuget.org/packages/FlatFiles)

## Overview
A lot of us still need to work with flat files (e.g. CSV or fixed-length) whether because we're interfacing with older systems or because we're running one-time migration scripts. It's a pain that there's nothing built into .NET for treating flat files like a database table.

FlatFiles makes it easy to read and write flat files in many different ways. It supports type mappers for directly reading and writing with data objects, using a style similar to Entity Framework Code First. You can also go back and forth between files and `DataTable`s. It also supports the ability to expose a file using the `IDataReader` interface, for working with the low-level ADO.NET classes. If you really want to, you can read and write values with raw `object` arrays. FlatFiles also supports a large number of options for customizing how files are interpreted, to support the most common types of flat files.

## Type Mappers
Using the type mappers, you can directly read file contents into your classes:

    var mapper = SeparatedValueTypeMapper.Define<Customer>();
    mapper.Property(c => c.CustomerId).ColumnName("customer_id");
    mapper.Property(c => c.Name).ColumnName("name");
    mapper.Property(c => c.Created).ColumnName("created").InputFormat("yyyyMMdd");
    mapper.Property(c => c.AverageSales).ColumnName("avg_sales");
    var customers = mapper.Read(@"C:\path\to\file.csv");
	
Writing to a file is just as easily:

    mapper.Property(c => c.Created).OutputFormat("yyyyMMdd");
    mapper.Property(c => c.AverageSales).OutputFormat("N2");
    mapper.Write(@"C:\path\to\file2.csv", customers);
	
Note that the mapper assumes the order `Property` is called the first time for a particular property matches the order the columns appear in the file. Additional references to the property have no impact on the expected order.
	
## Schemas
Type mapping internally defines a schema, which defines the name, order and type of each column in the flat file. In order to use the other classes in FlatFiles, we must define the schema explicitly. For instance, this is how we would define a CSV file schema:

    SeparatedValueSchema schema = new SeparatedValueSchema();
    schema.AddColumn(new Int64Column("customer_id"))
          .AddColumn(new StringColumn("name"))
          .AddColumn(new DateTimeColumn("created") { InputFormat = "yyyyMMdd", OutputFormat = "yyyyMMdd" })
          .AddColumn(new DoubleColumn("avg_sales") { OutputFormat = "N2" });
		  
Or, if the schema is for a fixed-length file:

    FixedLengthSchema schema = new FixedLengthSchema();
    schema.AddColumn(new Int64Column("customer_id"), 10)
      .AddColumn(new StringColumn("name"), 255)
      .AddColumn(new DateTimeColumn("created", 8) { InputFormat = "yyyyMMdd", OutputFormat = "yyyyMMdd" })
      .AddColumn(new DoubleColumn("avg_sales", 10) { OutputFormat = "N2" });
	  
The `FixedLengthSchema` class is the same as the `SeparatedValueSchema` class, except it associates a `Window` to each column. A `Window` records the `Width` of the column in the file. It also allows you to specify the `Alignment` (left or right) in cases where the value doesn't fill the entire width of the column (the default is left aligned). The `FillCharacter` property can be used say what character is used as padding.

Some fixed-length files may have columns that are not used. The fixed-length schema doesn't provide a way to specify a starting index for a column. Simply define "ignored" columns for gaps in the input file.

## SeparatedValueReader
If you are working with delimited files, such as comma-separated or tab-separated files, you will want to use the `SeparatedValueReader` class. The constructor accepts a combination of a file name (or stream), a `SeparatedValueSchema` object and/or a `SeparatedValueOptions` object.

When parsing separated files, you can surround fields with double or single quotes. This way you can include the separator string within the field. Of course, this won't work if the separator is the quote character itself!

The `SeparatedValueOptions` class supports a `Separator` property for specifying the string that separates your fields. A comma (`,`) is the default separator.

The `RecordSeparator` property specifies what character sequence is used to separate records. By default, this is `Environment.NewLine` (`\r\n`). This is useful if you are working on files from other systems, such as Linux (`\n`) or Macintosh (`\r`).

The `IsFirstRecordSchema` property tells the reader to treat the first record in the file as the schema. Since the types of the fields cannot be determined from a file, they are assumed to be strings. If you provide the schema to the constructor, it will be used instead, and the first record will simply be skipped. By default, this property is set to `false`.

## SeparateValueWriter
If you want to build a delimited file, you can use the `SeparatedValueWriter` class. It accepts the same schema and options arguments. If the `SeparatedValueOptions`'s `IsFirstRecordSchema` property is set to `true`, the schema will be written to the file upon writing the first record.

## FixedLengthReader
If you are working with files whose fields are a fixed-length you will want to use the `FixedLengthReader` class. The constructor accepts a combination of a file name (or stream), a `FixedLengthSchema` object and/or a `FixedLengthOptions` object.

The `FixedLengthOptions` class supports a `FillCharacter` property to specify which character is used as a fill character in the columns. A space (` `) is the default.

It also supports a `RecordSeparator` property for specifying what value indicates the end of a record. By default, this is `Environment.NewLine` (`\r\n`). This is useful if you are working on files from other systems, such as Linux (`\n`) or Macintosh (`\r`).

## FixedLengthWriter
If you want to build a fixed-length file, you can use the `FixedLengthWriter` class. It accepts the same schema and options arguments used to read files. If you want to control the alignment of the columns, you can specify the `FixedAlignment` for each column when defining the schema. This will control whether padding is put to the right or the left of the value.
	
## DataTables
If you are using `DataTable`s, you can read and write to a `DataTable` using the `ReadFlatFile` and `WriteFlatFile` extension methods. Just pass the corresponding reader or writer object.

    DataTable customerTable = new DataTable("Customer");
    using (IReader reader = new SeparatedValueReader(@"C:\path\to\file.csv", schema))
    {
        customerTable.ReadFlatFile(reader);
    }

## FlatFileReader
For low-level file reading, you can use the `FlatFileReader` class. It provides an `IDataReader` interface to the records in the file, making it compatible with other ADO.NET interfaces.

    // The DataRead Approach
    using (FlatFileReader reader = new FlatFileReader(new SeparatedValueReader(@"C:\path\to\file.csv", schema))
    {
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
    }
