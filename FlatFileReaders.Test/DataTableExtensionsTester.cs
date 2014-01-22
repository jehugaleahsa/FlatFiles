using System;
using System.Data;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFileReaders.Test
{
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// Tests the DataTableExtensions class.
    /// </summary>
    [TestClass]
    public class DataTableExtensionsTester
    {
        /// <summary>
        /// Setup for tests.
        /// </summary>
        [TestInitialize]
        public void TestSetup()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// An exception should be thrown if the table is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadFlatFile_DataTableNull_Throws()
        {
            const string text = @"id,name,created,avg
123,Bob,12/31/2012,3.14159";
            SeparatedValueParserOptions options = new SeparatedValueParserOptions() { IsFirstRecordSchema = true };
            Stream stream = new MemoryStream(Encoding.Default.GetBytes(text));
            DataTable table = null;
            IParser parser = new SeparatedValueParser(stream, options);
            DataTableExtensions.ReadFlatFile(table, parser);
        }

        /// <summary>
        /// An exception should be thrown if the table is null.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestReadFlatFile_ParserNull_Throws()
        {
            DataTable table = new DataTable();
            IParser parser = null;
            table.ReadFlatFile(parser);
        }

        /// <summary>
        /// The schema should be extracted from the file and then the data is used
        /// to populate the table.
        /// </summary>
        [TestMethod]
        public void TestReadFlatFile_ExtractsSchema_PopulatesTable()
        {
            const string text = @"id,name,created,avg
123,Bob,12/31/2012,3.14159";
            SeparatedValueParserOptions options = new SeparatedValueParserOptions() { IsFirstRecordSchema = true };
            Stream stream = new MemoryStream(Encoding.Default.GetBytes(text));
            DataTable table = new DataTable();
            IParser parser = new SeparatedValueParser(stream, options);
            table.ReadFlatFile(parser);
            Assert.AreEqual(4, table.Columns.Count, "The wrong number of columns were extracted.");
            Assert.IsTrue(table.Columns.Contains("id"), "The ID column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("name"), "The name column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("created"), "The created column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("avg"), "The AVG column was not extracted.");
            Assert.AreEqual(1, table.Rows.Count, "Not all of the records were extracted.");
            DataRow row = table.Rows[0];
            object[] expected = new object[] { "123", "Bob", "12/31/2012", "3.14159" };
            object[] values = row.ItemArray;
            CollectionAssert.AreEqual(expected, values, "The wrong values were extracted");
        }

        /// <summary>
        /// If there is any existing schema information or rows, it should be cleared out.
        /// </summary>
        [TestMethod]
        public void TestReadFlatFile_ClearsSchemaAndData()
        {
            // fill the table with some existing columns, constraints and data
            DataTable table = new DataTable();
            DataColumn column = table.Columns.Add("blah", typeof(int));
            table.Columns.Add("bleh", typeof(string));
            table.Constraints.Add("PK_blah", column, true);
            DataRow row = table.Rows.Add(new object[] { 123, "dopey" });
            row.AcceptChanges();

            const string text = @"id,name,created,avg
123,Bob,12/31/2012,3.14159";
            SeparatedValueParserOptions options = new SeparatedValueParserOptions() { IsFirstRecordSchema = true };
            Stream stream = new MemoryStream(Encoding.Default.GetBytes(text));
            IParser parser = new SeparatedValueParser(stream, options);
            table.ReadFlatFile(parser);
            Assert.AreEqual(4, table.Columns.Count, "The wrong number of columns were extracted.");
            Assert.IsTrue(table.Columns.Contains("id"), "The ID column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("name"), "The name column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("created"), "The created column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("avg"), "The AVG column was not extracted.");
            Assert.AreEqual(1, table.Rows.Count, "Not all of the records were extracted.");
            row = table.Rows[0];
            object[] expected = new object[] { "123", "Bob", "12/31/2012", "3.14159" };
            object[] values = row.ItemArray;
            CollectionAssert.AreEqual(expected, values, "The wrong values were extracted");
        }

        /// <summary>
        /// If a schema is provided, it should be used to set the column types.
        /// </summary>
        [TestMethod]
        public void TestReadFlatFile_SchemaProvided_TypesUsed()
        {
            const string text = @"123,Bob,12/31/2012,3.14159";
            Stream stream = new MemoryStream(Encoding.Default.GetBytes(text));
            DataTable table = new DataTable();
            Schema schema = new Schema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"))
                  .AddColumn(new DoubleColumn("avg"));
            IParser parser = new SeparatedValueParser(stream, schema);
            table.ReadFlatFile(parser);
            Assert.AreEqual(4, table.Columns.Count, "The wrong number of columns were extracted.");
            Assert.IsTrue(table.Columns.Contains("id"), "The ID column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("name"), "The name column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("created"), "The created column was not extracted.");
            Assert.IsTrue(table.Columns.Contains("avg"), "The AVG column was not extracted.");
            Assert.AreEqual(1, table.Rows.Count, "Not all of the records were extracted.");
            DataRow row = table.Rows[0];
            object[] expected = new object[] { 123, "Bob", new DateTime(2012, 12, 31), 3.14159 };
            object[] values = row.ItemArray;
            CollectionAssert.AreEqual(expected, values, "The wrong values were extracted");
        }
    }
}
