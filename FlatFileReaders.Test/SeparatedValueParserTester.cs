using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFileReaders.Test
{
    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// Tests the SeparatedValueParser class.
    /// </summary>
    [TestClass]
    public class SeparatedValueParserTester
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
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_TextNull_Throws()
        {
            string text = null;
            new SeparatedValueParser(text);
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Schema_TextNull_Throws()
        {
            string text = null;
            Schema schema = new Schema();
            new SeparatedValueParser(text, schema);
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_TextNull_Throws()
        {
            string text = null;
            SeparatedValueParserOptions options = new SeparatedValueParserOptions();
            new SeparatedValueParser(text, options);
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_Schema_TextNull_Throws()
        {
            string text = null;
            Schema schema = new Schema();
            SeparatedValueParserOptions options = new SeparatedValueParserOptions();
            new SeparatedValueParser(text, schema, options);
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_SchemaNull_Throws()
        {
            string text = String.Empty;
            Schema schema = null;
            new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_SchemaNull_Throws()
        {
            string text = String.Empty;
            Schema schema = null;
            SeparatedValueParserOptions options = new SeparatedValueParserOptions();
            new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
        }

        /// <summary>
        /// If we try to pass null options to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_OptionsNull_Throws()
        {
            string text = "";
            SeparatedValueParserOptions options = null;
            new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), options);
        }

        /// <summary>
        /// If we try to pass null options to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Schema_OptionsNull_Throws()
        {
            string text = "";
            Schema schema = new Schema();
            SeparatedValueParserOptions options = null;
            new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
        }

        /// <summary>
        /// If we pass a single record, Read should return true once.
        /// </summary>
        [TestMethod]
        public void TestRead_SingleRecord_ReturnsTrueOnce()
        {
            const string text = "a,b,c";
            SeparatedValueParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)));
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            string[] expected = new string[] { "a", "b", "c" };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
            canRead = parser.Read();
            Assert.IsFalse(canRead, "No more records should have been read.");
        }

        /// <summary>
        /// If we try to get the values before calling Read, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRead_GetValuesWithoutReading_Throws()
        {
            string text = "a,b,c";
            SeparatedValueParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)));
            parser.GetValues();
        }

        /// <summary>
        /// We should be able to read the same values as many times as we want.
        /// </summary>
        [TestMethod]
        public void TestRead_MultipleCallsToValues_ReturnsSameValues()
        {
            string text = "a,b,c";
            SeparatedValueParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)));
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            string[] expected = new string[] { "a", "b", "c" };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
            actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The same values were not returned multiple times.");
        }

        /// <summary>
        /// If Read returns false, requesting the Values will cause an exception to be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRead_ValuesAfterEndOfFile_Throws()
        {
            string text = "a,b,c";
            SeparatedValueParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)));
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            canRead = parser.Read();
            Assert.IsFalse(canRead, "We should have reached the end of the file.");
            parser.GetValues();
        }

        /// <summary>
        /// If we pass a string with CP1252 characters, it should reflect does characters when returning
        /// </summary>
        [TestMethod]
        public void TestRead_RecordWithCP1252Characters_ReturnsCorrectCharacters()
        {
            //---- Arrange -----------------------------------------------------
            // Need to convert the string to target encoding because otherwise a string declared in VS will always be encoded as UTF-8
            var text = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1252), Encoding.UTF8.GetBytes(@"123;Müller;1/17/2014"));
            var schema = new Schema();
            schema.AddColumn(new Int32Column("id")).AddColumn(new StringColumn("name")).AddColumn(new DateTimeColumn("created"));
            var options = new SeparatedValueParserOptions 
            { 
                IsFirstRecordSchema = false, 
                Separator = ";" ,
                Encoding = Encoding.GetEncoding(1252)
            };

            var testee = new SeparatedValueParser(new MemoryStream(text), schema, options);

            //---- Act ---------------------------------------------------------
            var result = testee.Read();

            //---- Assert ------------------------------------------------------
            Assert.IsTrue(result, "Could not read the record.");
            object[] expected = { 123, "Müller", new DateTime(2014, 1, 17) };
            object[] actual = testee.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
        }

        /// <summary>
        /// If we pass a string with CP1251 characters, it should reflect does characters when returning
        /// </summary>
        [TestMethod]
        public void TestRead_RecordWithCP1251Characters_ReturnsCorrectCharacters()
        {
            //---- Arrange -----------------------------------------------------
            // Need to convert the string to target encoding because otherwise a string declared in VS will always be encoded as UTF-8
            var text = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1251), Encoding.UTF8.GetBytes(@"123;Лучиано;1/17/2014"));
            var schema = new Schema();
            schema.AddColumn(new Int32Column("id")).AddColumn(new StringColumn("name")).AddColumn(new DateTimeColumn("created"));
            var options = new SeparatedValueParserOptions 
            { 
                IsFirstRecordSchema = false, 
                Separator = ";", 
                Encoding = Encoding.GetEncoding(1251) 
            };

            var testee = new SeparatedValueParser(new MemoryStream(text), schema, options);

            //---- Act ---------------------------------------------------------
            var result = testee.Read();

            //---- Assert ------------------------------------------------------
            Assert.IsTrue(result, "Could not read the record.");
            object[] expected = { 123, "Лучиано", new DateTime(2014, 1, 17) };
            object[] actual = testee.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
        }

        /// <summary>
        /// If we do not explicitly say that the first record is the schema, we cannot retrieve it later.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetSchema_NotExtracted_Throws()
        {
            string text = "a,b,c";
            SeparatedValueParserOptions options = new SeparatedValueParserOptions() { IsFirstRecordSchema = false };
            IParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), options);
            parser.GetSchema();
        }

        /// <summary>
        /// If we say that the first record is the schema, we can retrieve it later on.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_Extracted_ReturnsColumnNames()
        {
            string text = "a,b,c";
            SeparatedValueParserOptions options = new SeparatedValueParserOptions() { IsFirstRecordSchema = true };
            IParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), options);
            Schema schema = parser.GetSchema();
            Assert.IsTrue(schema.ColumnDefinitions.All(d => d is StringColumn), "Not all of the columns were treated as strings.");
            string[] actual = schema.ColumnDefinitions.Select(d => d.ColumnName).ToArray();
            string[] expected = new string[] { "a", "b", "c" };
            CollectionAssert.AreEqual(expected, actual, "The schema was not extracted as expected.");
        }

        /// <summary>
        /// If we provide a schema and say the first record is the schema, our schema takes priority
        /// and we throw away the first record.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_FirstRecordSchema_SkipsFirstRecord()
        {
            const string text = @"id,name,created";
            Schema schema = new Schema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));
            SeparatedValueParserOptions options = new SeparatedValueParserOptions() { IsFirstRecordSchema = true };
            IParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
            Schema actual = parser.GetSchema();
            Assert.AreSame(schema, actual, "The schema was passed did not take priority.");
            Assert.IsFalse(parser.Read(), "The schema record was not skipped.");
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_ParsesValues()
        {
            const string text = @"123,Bob,1/19/2013";
            Schema schema = new Schema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));
            SeparatedValueParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            Assert.IsTrue(parser.Read(), "The first record was skipped.");
            object[] actual = parser.GetValues();
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            CollectionAssert.AreEqual(expected, actual, "The values were not parsed as expected.");
        }

        /// <summary>
        /// If we provide a schema, the records in the file must have a value for each column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void TestGetSchema_SchemaProvided_WrongNumberOfColumns_Throws()
        {
            const string text = @"123,Bob";
            Schema schema = new Schema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));
            SeparatedValueParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            parser.Read();
        }

        /// <summary>
        /// If the first record is the schema, the records in the file must have the
        /// same number of columns.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void TestGetSchema_FirstRecordSchema_WrongNumberOfColumns_Throws()
        {
            const string text = @"id,name,created
123,Bob,1/19/2013,Hello";
            SeparatedValueParserOptions options = new SeparatedValueParserOptions() { IsFirstRecordSchema = true };
            SeparatedValueParser parser = new SeparatedValueParser(new MemoryStream(Encoding.Default.GetBytes(text)), options);
            parser.Read();
        }
    }
}
