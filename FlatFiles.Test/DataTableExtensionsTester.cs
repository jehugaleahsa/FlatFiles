using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the DataTableExtensions class.
    /// </summary>
    [TestClass]
    public class DataTableExtensionsTester
    {
        /// <summary>
        /// Setup for tests.
        /// </summary>
        public DataTableExtensionsTester()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// An exception should be thrown if the table is null.
        /// </summary>
        [TestMethod]
        public void TestReadFlatFile_DataTableNull_Throws()
        {
            DataTable table = null;
            StringReader stringReader = new StringReader(String.Empty);
            IReader parser = new SeparatedValueReader(stringReader);
            Assert.ThrowsException<ArgumentNullException>(() => DataTableExtensions.ReadFlatFile(table, parser));
        }

        /// <summary>
        /// An exception should be thrown if the table is null.
        /// </summary>
        [TestMethod]
        public void TestReadFlatFile_ParserNull_Throws()
        {
            DataTable table = new DataTable();
            IReader parser = null;
            Assert.ThrowsException<ArgumentNullException>(() => table.ReadFlatFile(parser));
        }

        /// <summary>
        /// The schema should be extracted from the file and then the data is used to populate the table.
        /// </summary>
        [TestMethod]
        public void TestReadFlatFile_ExtractsSchema_PopulatesTable()
        {
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                .AddColumn(new StringColumn("name"))
                .AddColumn(new DateTimeColumn("created") { InputFormat = "MM/dd/yyyy", OutputFormat = "MM/dd/yyyy" })
                .AddColumn(new DecimalColumn("avg"));
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };

            StringWriter stringWriter = new StringWriter();
            SeparatedValueWriter builder = new SeparatedValueWriter(stringWriter, schema, options);
            var data = new object[] { 123, "Bob", new DateTime(2012, 12, 31), 3.14159m };
            builder.Write(data);

            StringReader stringReader = new StringReader(stringWriter.ToString());
            DataTable table = new DataTable();
            IReader parser = new SeparatedValueReader(stringReader, schema, options);
            table.ReadFlatFile(parser);

            Assert.AreEqual(4, table.Columns.Count);
            Assert.IsTrue(table.Columns.Contains("id"), "The ID column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("name"), "The name column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("created"), "The created column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("avg"), "The AVG column was not extracted.");

            Assert.AreEqual(1, table.Rows.Count);            
            CollectionAssert.AreEqual(data, table.Rows[0].ItemArray);
        }

        /// <summary>
        /// If there is existing schema information or rows, it must match the flat file schema.
        /// </summary>
        [TestMethod]
        public void TestReadFlatFile_AddsData()
        {
            // fill the table with some existing columns, constraints and data
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add("id", typeof(int));
            DataColumn nameColumn = table.Columns.Add("name", typeof(string));
            DataColumn createdColumn = table.Columns.Add("created", typeof(DateTime));
            DataColumn avgColumn = table.Columns.Add("avg", typeof(decimal));
            table.Constraints.Add("PK_blah", idColumn, true);
            DataRow row = table.Rows.Add(new object[] { 1, "Bob", new DateTime(2018, 07, 16), 12.34m });
            row.AcceptChanges();

            const string text = @"id,name,created,avg
2,John,07/17/2018,23.45
3,Susan,07/18/2018,34.56";
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            StringReader stringReader = new StringReader(text);
            IReader csvReader = new SeparatedValueReader(stringReader, options);
            table.ReadFlatFile(csvReader);

            Assert.AreEqual(4, table.Columns.Count);
            Assert.IsTrue(table.Columns.Contains("id"), "The ID column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("name"), "The name column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("created"), "The created column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("avg"), "The AVG column was not extracted.");

            Assert.AreEqual(3, table.Rows.Count);
            CollectionAssert.AreEqual(new object[] { 1, "Bob", new DateTime(2018, 07, 16), 12.34m }, table.Rows[0].ItemArray);
            CollectionAssert.AreEqual(new object[] { 2, "John", new DateTime(2018, 07, 17), 23.45m }, table.Rows[1].ItemArray);
            CollectionAssert.AreEqual(new object[] { 3, "Susan", new DateTime(2018, 07, 18), 34.56m }, table.Rows[2].ItemArray);
        }

        /// <summary>
        /// If there is existing schema information, missing columns should be filled with nulls.
        /// </summary>
        [TestMethod]
        public void TestReadFlatFile_ColumnMissing_InsertsNulls()
        {
            // fill the table with some existing columns, constraints and data
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add("id", typeof(int));
            DataColumn nameColumn = table.Columns.Add("name", typeof(string));
            DataColumn createdColumn = table.Columns.Add("created", typeof(DateTime));
            DataColumn avgColumn = table.Columns.Add("avg", typeof(string));
            table.Constraints.Add("PK_blah", idColumn, true);
            DataRow row = table.Rows.Add(new object[] { 1, "Bob", new DateTime(2018, 07, 16), "12.34" });
            row.AcceptChanges();

            const string text = @"id,name,created
2,John,07/17/2018
3,Susan,07/18/2018";
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            StringReader stringReader = new StringReader(text);
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new DateTimeColumn("created"));
            IReader csvReader = new SeparatedValueReader(stringReader, schema, options);
            table.ReadFlatFile(csvReader);

            Assert.AreEqual(4, table.Columns.Count);
            Assert.IsTrue(table.Columns.Contains("id"), "The ID column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("name"), "The name column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("created"), "The created column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("avg"), "The AVG column was not extracted.");

            Assert.AreEqual(3, table.Rows.Count);
            CollectionAssert.AreEqual(new object[] { 1, "Bob", new DateTime(2018, 07, 16), "12.34" }, table.Rows[0].ItemArray);
            CollectionAssert.AreEqual(new object[] { 2, "John", new DateTime(2018, 07, 17), DBNull.Value }, table.Rows[1].ItemArray);
            CollectionAssert.AreEqual(new object[] { 3, "Susan", new DateTime(2018, 07, 18), DBNull.Value }, table.Rows[2].ItemArray);
        }

        /// <summary>
        /// If there is existing schema information, missing columns should be filled with nulls.
        /// </summary>
        [TestMethod]
        public void TestReadFlatFile_Merge_PreserveChanges()
        {
            // fill the table with some existing columns, constraints and data
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add("id", typeof(int));
            DataColumn nameColumn = table.Columns.Add("name", typeof(string));
            DataColumn createdColumn = table.Columns.Add("created", typeof(DateTime));
            DataColumn avgColumn = table.Columns.Add("avg", typeof(decimal));
            table.Constraints.Add("PK_blah", idColumn, true);
            DataRow row = table.Rows.Add(new object[] { 1, "Bob", new DateTime(2018, 07, 16), 12.34m });
            row.AcceptChanges();

            row.SetField(avgColumn, 99.99m);  // Change but do not accept

            const string text = @"id,name,created
1,Robert,07/19/2018,78.90
2,John,07/17/2018,23.45
3,Susan,07/18/2018,34.56";
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            StringReader stringReader = new StringReader(text);
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new DateTimeColumn("created"));
            schema.AddColumn(new DecimalColumn("avg"));
            IReader csvReader = new SeparatedValueReader(stringReader, schema, options);
            table.ReadFlatFile(csvReader, LoadOption.PreserveChanges);

            Assert.AreEqual(4, table.Columns.Count);
            Assert.IsTrue(table.Columns.Contains("id"), "The ID column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("name"), "The name column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("created"), "The created column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("avg"), "The AVG column was not extracted.");

            var additions = table.GetChanges(DataRowState.Added);
            Assert.IsNull(additions);

            var deletions = table.GetChanges(DataRowState.Deleted);
            Assert.IsNull(deletions);

            var modifications = table.GetChanges(DataRowState.Modified);
            Assert.IsNotNull(modifications);
            Assert.AreEqual(1, modifications.Rows.Count);

            Assert.AreEqual(3, table.Rows.Count);
            CollectionAssert.AreEqual(new object[] { 1, "Bob", new DateTime(2018, 07, 16), 99.99m }, table.Rows[0].ItemArray);
            CollectionAssert.AreEqual(new object[] { 2, "John", new DateTime(2018, 07, 17), 23.45m }, table.Rows[1].ItemArray);
            CollectionAssert.AreEqual(new object[] { 3, "Susan", new DateTime(2018, 07, 18), 34.56m }, table.Rows[2].ItemArray);
        }

        [TestMethod]
        public void TestReadFlatFile_Merge_Upsert()
        {
            // fill the table with some existing columns, constraints and data
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add("id", typeof(int));
            DataColumn nameColumn = table.Columns.Add("name", typeof(string));
            DataColumn createdColumn = table.Columns.Add("created", typeof(DateTime));
            DataColumn avgColumn = table.Columns.Add("avg", typeof(decimal));
            table.Constraints.Add("PK_blah", idColumn, true);
            DataRow row = table.Rows.Add(new object[] { 1, "Bob", new DateTime(2018, 07, 16), 12.34m });
            row.AcceptChanges();

            row.SetField(avgColumn, 99.99m);  // Change but do not accept

            const string text = @"id,name,created
1,Robert,07/19/2018,78.90
2,John,07/17/2018,23.45
3,Susan,07/18/2018,34.56";
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            StringReader stringReader = new StringReader(text);
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new DateTimeColumn("created"));
            schema.AddColumn(new DecimalColumn("avg"));
            IReader csvReader = new SeparatedValueReader(stringReader, schema, options);
            table.ReadFlatFile(csvReader, LoadOption.Upsert);

            Assert.AreEqual(4, table.Columns.Count);
            Assert.IsTrue(table.Columns.Contains("id"), "The ID column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("name"), "The name column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("created"), "The created column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("avg"), "The AVG column was not extracted.");

            var additions = table.GetChanges(DataRowState.Added);
            Assert.IsNotNull(additions);
            Assert.AreEqual(2, additions.Rows.Count);

            var deletions = table.GetChanges(DataRowState.Deleted);
            Assert.IsNull(deletions);

            var modifications = table.GetChanges(DataRowState.Modified);
            Assert.IsNotNull(modifications);
            Assert.AreEqual(1, modifications.Rows.Count);

            Assert.AreEqual(3, table.Rows.Count);
            CollectionAssert.AreEqual(new object[] { 1, "Robert", new DateTime(2018, 07, 19), 78.90m }, table.Rows[0].ItemArray);
            CollectionAssert.AreEqual(new object[] { 2, "John", new DateTime(2018, 07, 17), 23.45m }, table.Rows[1].ItemArray);
            CollectionAssert.AreEqual(new object[] { 3, "Susan", new DateTime(2018, 07, 18), 34.56m }, table.Rows[2].ItemArray);
        }

        [TestMethod]
        public void TestReadFlatFile_Merge_Overwrite()
        {
            // fill the table with some existing columns, constraints and data
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add("id", typeof(int));
            DataColumn nameColumn = table.Columns.Add("name", typeof(string));
            DataColumn createdColumn = table.Columns.Add("created", typeof(DateTime));
            DataColumn avgColumn = table.Columns.Add("avg", typeof(decimal));
            table.Constraints.Add("PK_blah", idColumn, true);
            DataRow row = table.Rows.Add(new object[] { 1, "Bob", new DateTime(2018, 07, 16), 12.34m });
            row.AcceptChanges();

            row.SetField(avgColumn, 99.99m);  // Change but do not accept

            const string text = @"id,name,created
1,Robert,07/19/2018,78.90
2,John,07/17/2018,23.45
3,Susan,07/18/2018,34.56";
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            StringReader stringReader = new StringReader(text);
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new DateTimeColumn("created"));
            schema.AddColumn(new DecimalColumn("avg"));
            IReader csvReader = new SeparatedValueReader(stringReader, schema, options);
            table.ReadFlatFile(csvReader, LoadOption.OverwriteChanges);

            Assert.AreEqual(4, table.Columns.Count);
            Assert.IsTrue(table.Columns.Contains("id"), "The ID column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("name"), "The name column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("created"), "The created column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("avg"), "The AVG column was not extracted.");

            var additions = table.GetChanges(DataRowState.Added);
            Assert.IsNull(additions);
            var deletions = table.GetChanges(DataRowState.Deleted);
            Assert.IsNull(deletions);
            var modifications = table.GetChanges(DataRowState.Modified);
            Assert.IsNull(modifications);

            Assert.AreEqual(3, table.Rows.Count);
            CollectionAssert.AreEqual(new object[] { 1, "Robert", new DateTime(2018, 07, 19), 78.90m }, table.Rows[0].ItemArray);
            CollectionAssert.AreEqual(new object[] { 2, "John", new DateTime(2018, 07, 17), 23.45m }, table.Rows[1].ItemArray);
            CollectionAssert.AreEqual(new object[] { 3, "Susan", new DateTime(2018, 07, 18), 34.56m }, table.Rows[2].ItemArray);
        }

        [TestMethod]
        public void TestWriteFlatFile_MatchingSchema()
        {
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add("id", typeof(int));
            DataColumn nameColumn = table.Columns.Add("name", typeof(string));
            DataColumn createdColumn = table.Columns.Add("created", typeof(DateTime));
            DataColumn avgColumn = table.Columns.Add("avg", typeof(decimal));
            table.Constraints.Add("PK_blah", idColumn, true);

            DataRow bobRow = table.Rows.Add(new object[] { 1, "Bob", new DateTime(2018, 07, 16), 12.34m });
            DataRow johnRow = table.Rows.Add(new object[] { 2, "John", new DateTime(2018, 07, 17), 23.45m });
            DataRow susanRow = table.Rows.Add(new object[] { 3, "Susan", new DateTime(2018, 07, 18), 34.56m });
            DataRow nullRow = table.Rows.Add(new object[] { 4, null, null, null });

            var options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = true,
                RecordSeparator = "\r\n",
                FormatProvider = CultureInfo.InvariantCulture
            };
            var stringWriter = new StringWriter();
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new DateTimeColumn("created") { OutputFormat = "MM/dd/yyyy" });
            schema.AddColumn(new DecimalColumn("avg"));
            var csvWriter = new SeparatedValueWriter(stringWriter, schema, options);
            table.WriteFlatFile(csvWriter);

            string output = stringWriter.ToString();

            Assert.AreEqual(@"id,name,created,avg
1,Bob,07/16/2018,12.34
2,John,07/17/2018,23.45
3,Susan,07/18/2018,34.56
4,,,
", output);
        }

        [TestMethod]
        public void TestWriteFlatFile_MissingColumn_WritesNull()
        {
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add("id", typeof(int));
            DataColumn nameColumn = table.Columns.Add("name", typeof(string));
            //DataColumn createdColumn = table.Columns.Add("created", typeof(DateTime));
            DataColumn avgColumn = table.Columns.Add("avg", typeof(decimal));
            table.Constraints.Add("PK_blah", idColumn, true);

            DataRow bobRow = table.Rows.Add(new object[] { 1, "Bob", 12.34m });
            DataRow johnRow = table.Rows.Add(new object[] { 2, "John", 23.45m });
            DataRow susanRow = table.Rows.Add(new object[] { 3, "Susan", 34.56m });

            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            var stringWriter = new StringWriter();
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new DateTimeColumn("created") { OutputFormat = "MM/dd/yyyy" });
            schema.AddColumn(new DecimalColumn("avg"));
            var csvWriter = new SeparatedValueWriter(stringWriter, schema, options);
            table.WriteFlatFile(csvWriter);

            string output = stringWriter.ToString();

            Assert.AreEqual(@"id,name,created,avg
1,Bob,,12.34
2,John,,23.45
3,Susan,,34.56
", output);
        }

        [TestMethod]
        public void TestWriteFlatFile_ExtraColumn_Ignores()
        {
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add("id", typeof(int));
            DataColumn nameColumn = table.Columns.Add("name", typeof(string));
            DataColumn createdColumn = table.Columns.Add("created", typeof(DateTime));
            DataColumn avgColumn = table.Columns.Add("avg", typeof(decimal));
            table.Constraints.Add("PK_blah", idColumn, true);

            DataRow bobRow = table.Rows.Add(new object[] { 1, "Bob", new DateTime(2018, 07, 16), 12.34m });
            DataRow johnRow = table.Rows.Add(new object[] { 2, "John", new DateTime(2018, 07, 17), 23.45m });
            DataRow susanRow = table.Rows.Add(new object[] { 3, "Susan", new DateTime(2018, 07, 18), 34.56m });

            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            var stringWriter = new StringWriter();
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new DecimalColumn("avg"));
            var csvWriter = new SeparatedValueWriter(stringWriter, schema, options);
            table.WriteFlatFile(csvWriter);

            string output = stringWriter.ToString();

            Assert.AreEqual(@"id,name,avg
1,Bob,12.34
2,John,23.45
3,Susan,34.56
", output);
        }

        [TestMethod]
        public void TestReadFlatFile_IgnoredColumns2()
        {
            const string data =
@"A,B,C
1,2,3
4,5,6";
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("A"));
            schema.AddColumn(new IgnoredColumn("Ignored"));
            schema.AddColumn(new StringColumn("C"));

            var options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = true
            };

            var textReader = new StringReader(data);
            var csvReader = new SeparatedValueReader(textReader, schema, options);

            DataTable dataTable = new DataTable();
            dataTable.ReadFlatFile(csvReader);
            string[] columnNames = dataTable.Columns.OfType<DataColumn>()
                .Select(r => r.ColumnName)
                .ToArray();
            CollectionAssert.AreEqual(new[] { "A", "C" }, columnNames);
            Assert.AreEqual(2, dataTable.Rows.Count);
            object[] values1 = dataTable.Rows[0].ItemArray;
            CollectionAssert.AreEqual(new[] { "1", "3" }, values1);
            object[] values2 = dataTable.Rows[1].ItemArray;
            CollectionAssert.AreEqual(new[] { "4", "6" }, values2);
        }

        [TestMethod]
        public void TestWriteFlatFile_IgnoredColumns()
        {
            DataTable table = new DataTable();
            DataColumn idColumn = table.Columns.Add("id", typeof(int));
            DataColumn nameColumn = table.Columns.Add("name", typeof(string));
            DataColumn createdColumn = table.Columns.Add("created", typeof(DateTime));
            DataColumn avgColumn = table.Columns.Add("avg", typeof(decimal));
            table.Constraints.Add("PK_blah", idColumn, true);

            DataRow bobRow = table.Rows.Add(new object[] { 1, "Bob", new DateTime(2018, 07, 16), 12.34m });
            DataRow johnRow = table.Rows.Add(new object[] { 2, "John", new DateTime(2018, 07, 17), 23.45m });
            DataRow susanRow = table.Rows.Add(new object[] { 3, "Susan", new DateTime(2018, 07, 18), 34.56m });

            var options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = true,
                RecordSeparator = "\r\n",
                FormatProvider = CultureInfo.InvariantCulture
            };
            var stringWriter = new StringWriter();
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new IgnoredColumn("i0"));
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new IgnoredColumn("i1"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new IgnoredColumn("i2"));
            schema.AddColumn(new DateTimeColumn("created") { OutputFormat = "MM/dd/yyyy" });
            schema.AddColumn(new IgnoredColumn("i3"));
            schema.AddColumn(new DecimalColumn("avg"));
            schema.AddColumn(new IgnoredColumn("i4"));
            var csvWriter = new SeparatedValueWriter(stringWriter, schema, options);
            table.WriteFlatFile(csvWriter);

            string output = stringWriter.ToString();

            Assert.AreEqual(@"i0,id,i1,name,i2,created,i3,avg,i4
,1,,Bob,,07/16/2018,,12.34,
,2,,John,,07/17/2018,,23.45,
,3,,Susan,,07/18/2018,,34.56,
", output);
        }

        [TestMethod]
        public void TestReadFlatFile_DuplicateHeaderNames()
        {
            string data = @"ID,Description,Date,Time,Price,Date,Time,Price
""1"",""Net Profit"",""8/3/2020"",""9:58:48"",""$111.11"",""8/3/2020"",""10:41:10"",""$333.33""
""2"",""Net Loss"",""8/3/2020"",""14:41:10"",""$444.44"",""8/3/2020"",""16:29:08"",""$222.22""";
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("ID"));
            schema.AddColumn(new StringColumn("Description"));
            schema.AddColumn(new DateTimeColumn("PurchaseDate"));
            schema.AddColumn(new TimeSpanColumn("PurchaseTime"));
            schema.AddColumn(new DecimalColumn("PurchasePrice")
            {
                NumberStyles = NumberStyles.Currency
            });
            schema.AddColumn(new DateTimeColumn("SellDate"));
            schema.AddColumn(new TimeSpanColumn("SellTime"));
            schema.AddColumn(new DecimalColumn("SellPrice")
            {
                NumberStyles = NumberStyles.Currency
            });

            var options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = true
            };
            var reader = new SeparatedValueReader(new StringReader(data), schema, options);

            DataTable dataTable = new DataTable();
            dataTable.ReadFlatFile(reader);

            Assert.AreEqual(2, dataTable.Rows.Count);
            var actualColumns = dataTable.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToArray();
            CollectionAssert.AreEqual(new string[]
                {
                    "ID", "Description", "PurchaseDate", "PurchaseTime", "PurchasePrice", "SellDate", "SellTime", "SellPrice"
                }, actualColumns);
            CollectionAssert.AreEqual(new object[]
                {
                    1, "Net Profit", new DateTime(2020, 8, 3), new TimeSpan(9, 58, 48), 
                    111.11m, new DateTime(2020, 8, 3), new TimeSpan(10, 41, 10), 333.33m
                }, dataTable.Rows[0].ItemArray);
            CollectionAssert.AreEqual(new object[]
                {
                    2, "Net Loss", new DateTime(2020, 8, 3), new TimeSpan(14, 41, 10),
                    444.44m, new DateTime(2020, 8, 3), new TimeSpan(16, 29, 08), 222.22m
                }, dataTable.Rows[1].ItemArray);
        }
    }
}
