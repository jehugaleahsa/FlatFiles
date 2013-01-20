# FlatFileReaders

Work with flat files using the ADO.NET classes.

Download using NuGet: [FlatFileReaders](http://nuget.org/packages/FlatFileReaders)

## Overview
A lot of us still need to work with flat files (e.g. CSV or fixed-length) whether because we're interfacing with older systems or because we're running one-time migration scripts. It's a pain that there's nothing built into .NET for treating flat files like a database table.

One option for processing flat files would be to copy them into Excel, create an OleDB connection and yank the values out that way. However, Excel isn't very good at interpreting column types, so you end up getting back a table of strings most of the time. From there, you need to write a bunch of custom Int32 and DateTime parsing. Worst of all, you've introduced a manual step into your process!

FlatFileReaders makes it easy to read a file just like you'd read a query result. It allows you to define a schema declaratively (or extract it from the file itself) and it makes it easy to specify custom format strings. Once you're schema is defined, you can read the file using the FlatFileReader class or load your data directly into a DataTable.

    Schema schema = new Schema();
    schema.AddColumn(new Int64Column("customer_id"))
          .AddColumn(new StringColumn("name"))
          .AddColumn(new DateTimeColumn("created") { DateTimeFormat = "yyyyMMdd" })
          .AddColumn(new DoubleColumn("avg_sales"));
	
    // The DataRead Approach
    using (FlatFileReader reader = new FlatFileReader(new SeparatedValueParser(@"C:\path\to\file.csv", schema))
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
	
    // The DataTable Approach
    DataTable customerTable = new DataTable();
    using (IParser parser = new SeparatedValueParser(@"C:\path\to\file.csv", schema))
    {
        customerTable.ReadFlatFile(parser);
    }
