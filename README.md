# FlatFiles

Work with flat files using the ADO.NET classes.

Download using NuGet: [FlatFiles](http://nuget.org/packages/FlatFiles)

## Overview
A lot of us still need to work with flat files (e.g. CSV or fixed-length) whether because we're interfacing with older systems or because we're running one-time migration scripts. It's a pain that there's nothing built into .NET for treating flat files like a database table.

One option for processing flat files would be to copy them into Excel, create an OleDB connection and yank the values out that way. However, Excel isn't very good at interpreting column types, so you end up getting back a table of strings most of the time. From there, you need to write a bunch of custom Int32 and DateTime parsing. Worst of all, you've introduced a manual step into your process!

FlatFiles makes it easy to read a file just like you'd read a query result. It allows you to define a schema declaratively (or extract it from the file itself) and it makes it easy to specify custom format strings. 

First we define our schema:

    SeparatedValueSchema schema = new SeparatedValueSchema();
    schema.AddColumn(new Int64Column("customer_id"))
          .AddColumn(new StringColumn("name"))
          .AddColumn(new DateTimeColumn("created") { InputFormat = "yyyyMMdd" })
          .AddColumn(new DoubleColumn("avg_sales"));

Once you're schema is defined, you can read the file using the `FlatFileReader` class:

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
    
 or you can load your data directly into a `DataTable`:

    // The DataTable Approach
    DataTable customerTable = new DataTable("Customer");
    using (IReader reader = new SeparatedValueReader(@"C:\path\to\file.csv", schema))
    {
        customerTable.ReadFlatFile(reader);
    }

## SeparatedValueReader
If you are working with delimited files, such as comma separated or tab separated files, you will want to use the `SeparatedValueReader` class. The constructor accepts a combination of a file name (or stream), a `SeparatedValueSchema` object and/or a `SeparatedValueOptions` object.

The `SeparatedValueOptions` class supports a `Separator` property for specifying the string that separates your fields. A comma (`,`) is the default separator.

When parsing separated files, you can surround fields with double or single quotes. This way you can include the separator string within the field. Of course, this won't work if the separator is the quote character itself!

It also supports a `IsFirstRecordSchema` property that tells the reader to treat the first record in the file as the schema. Since the types of the fields cannot be determined from a file, they are assumed to be strings. If you provide the schema to the constructor, it will be used instead and the first record will simply be skipped. By default, this property is set to `false`.

## SeparateValueWriter
If you want to build a delimited file, you can use the `SeparatedValueWriter` class. It accepts the same schema and options arguments used to read files.

## FixedLengthReader
If you are working with files whose fields are a fixed-length you will want to use the `FixedLengthReader` class. The constructor accepts a combination of a file name (or stream), a `FixedLengthSchema` object and/or a `FixedLengthOptions` object.

The `FixedLengthSchema` class is the same as the `SeparatedValueSchema` class, except it associates a width with each column. You can also assign a default alignment (left or right) for each column.

The `FixedLengthOptions` class supports a `FillCharacter` property to specify which character is used as a fill character in the columns. A space (` `) is the default fill character.

It also supports a `RecordSeparator` property for specifying what value indicates the end of a record. By default, this value is `Environment.NewLine` (`\r\n`). However, if you're processing Unix/Linux files, you may need to change this to `\n` instead, or `\r` for Mac.

## FixedLengthWriter
If you want to build a fixed-length file, you can use the `FixedLengthWriter` class. It accepts the same schema and options arguments used to read files. If you want to control the alignment of the columns, you can specify the `FixedAlignment` for each column when defining the schema. This will control whether padding is put to the right or the left of the value.
