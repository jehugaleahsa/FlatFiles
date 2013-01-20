# FlatFileReaders

Work with flat files using ADO.NET.

## Overview
A lot of us still need to work with flat files (e.g. CSV or fixed-length) whether because we're interfacing with older systems or because we're running a one-time migration script. It's a pain that there's nothing built in to .NET for treating flat files like a database table.

One option for processing flat files would be to copy them into Excel, create an OleDB connection and yank the values out that way. However, Excel isn't very good at interpreting column types, so you end up getting back a table of strings most of the time. From there, you need to write a bunch of custom Int32 and DateTime parsing. Worst of all, you've introduced a manual step into your process!

FlatFileReaders eliminates the two most time-consuming tasks from processing flat files: getting the data into a format your .NET code can understand and parsing your values. It allows you to define a schema declaratively (or extract it from the file itself) and it makes it easy to specify custom format strings. Once you're schema is defined, you can read the file using the FlatFileReader class or load your data directly into a DataTable.

    Schema schema = new Schema();
    schema.AddColumn(new Int64Column("customer_id"))
          .AddColumn(new StringColumn("name"))
          .AddColumn(new DateTimeColumn("created") { DateTimeFormat = "yyyyMMdd" })
          .AddColumn(new DoubleColumn("avg_sales"));
    DataTable customerTable = new DataTable();
    customerTable.ReadCSV(@"C:\path\to\file.csv");
    
## Work In-Progress
I am in the process of writing the code for this project.

Completed:
* Implement a CSV parser
** Support overwriting separator string (command by default).
** Support double quotes
*** Support escaping double quotes
*** Support new-lines within double quotes
** Support single quotes
*** Support escaping single quotes
*** Support new-lines within single quotes
** Support deriving schema from first record
* Implement a fixed-length parser
** Support overwriting record separator string (\r\n by default)
** Support overwriting fill character (space by default)
* Implement Schema class
* Implement Int32Column class
* Implement StringColumn class
* Implement DateTimeColumn class

In-Progress:
* Implement a BooleanColumn class
* Implement a ByteColumn class
* Implement a CharColumn class
* Implement an Int16Column class
* Implement an Int64Column class
* Implement a SingleColumn class
* Implement a DoubleColumn class
* Implement a DecimalColumn class
* Implement a GuidColumn class
* Implement a ByteArrayColumn class
* Implement a CharArrayColumn class
* Implement FlatFileReader class
* Implement DataTable extension methods
