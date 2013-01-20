using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFileReaders.Test
{
    /// <summary>
    /// Tests the FixedLengthParserTester class.
    /// </summary>
    [TestClass]
    public class FixedLengthParserTester
    {
        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_TextNull_Throws()
        {
            string text = null;
            FixedLengthSchema schema = new FixedLengthSchema();
            new FixedLengthParser(text, schema);
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_TextNull_Throws()
        {
            Stream stream = null;
            FixedLengthSchema schema = new FixedLengthSchema();
            FixedLengthParserOptions options = new FixedLengthParserOptions();
            new FixedLengthParser(stream, schema, options);
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_SchemaNull_Throws()
        {
            string text = String.Empty;
            FixedLengthSchema schema = null;
            new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_SchemaNull_Throws()
        {
            string text = String.Empty;
            FixedLengthSchema schema = null;
            FixedLengthParserOptions options = new FixedLengthParserOptions();
            new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
        }

        /// <summary>
        /// If we try to pass null options to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_OptionsNull_Throws()
        {
            string text = String.Empty;
            FixedLengthSchema schema = new FixedLengthSchema();
            FixedLengthParserOptions options = null;
            new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
        }

        /// <summary>
        /// If we pass a single record, Read should return true once.
        /// </summary>
        [TestMethod]
        public void TestRead_SingleRecord_ReturnsTrueOnce()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), 10).AddColumn(new StringColumn("name"), 25).AddColumn(new DateTimeColumn("created"), 10);
            FixedLengthParser parser = new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            Assert.IsTrue(parser.Read(), "Could not read the record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
            Assert.IsFalse(parser.Read(), "No more records should have been read.");
        }

        /// <summary>
        /// If we try to get the values before calling Read, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRead_GetValuesWithoutReading_Throws()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), 10).AddColumn(new StringColumn("name"), 25).AddColumn(new DateTimeColumn("created"), 10);
            FixedLengthParser parser = new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            parser.GetValues();
        }

        /// <summary>
        /// We should be able to read the same values as many times as we want.
        /// </summary>
        [TestMethod]
        public void TestRead_MultipleCallsToValues_ReturnsSameValues()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), 10).AddColumn(new StringColumn("name"), 25).AddColumn(new DateTimeColumn("created"), 10);
            FixedLengthParser parser = new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
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
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), 10).AddColumn(new StringColumn("name"), 25).AddColumn(new DateTimeColumn("created"), 10);
            FixedLengthParser parser = new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            Assert.IsTrue(parser.Read(), "Could not read the record.");
            Assert.IsFalse(parser.Read(), "We should have reached the end of the file.");
            parser.GetValues();
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values
        /// and can be retrieved.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_ParsesValues()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), 10).AddColumn(new StringColumn("name"), 25).AddColumn(new DateTimeColumn("created"), 10);
            IParser parser = new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            Schema actual = parser.GetSchema();
            Assert.AreSame(schema.Schema, actual, "The underlying schema was not returned.");
        }

        /// <summary>
        /// The records in the file must have a value for each column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ParserException))]
        public void TestGetSchema_SchemaProvided_WrongNumberOfColumns_Throws()
        {
            const string text = @"       123                      Bob";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), 10)
                  .AddColumn(new StringColumn("name"), 25)
                  .AddColumn(new DateTimeColumn("created"), 10);
            FixedLengthParser parser = new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            parser.Read();
        }

        /// <summary>
        /// If we specify a custom record separator, it should be used
        /// to split records in the file.
        /// </summary>
        [TestMethod]
        public void TestGetValues_CustomRecordSeparator_SplitsFile()
        {
            const string text = "       123                      Bob 1/19/2013BOOM       234                      Sam12/20/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), 10)
                  .AddColumn(new StringColumn("name"), 25)
                  .AddColumn(new DateTimeColumn("created"), 10);
            FixedLengthParserOptions options = new FixedLengthParserOptions() { RecordSeparator = "BOOM" };
            FixedLengthParser parser = new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);

            Assert.IsTrue(parser.Read(), "Could not read the first record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The values for the first record were wrong.");

            Assert.IsTrue(parser.Read(), "Could not read the second record.");
            expected = new object[] { 234, "Sam", new DateTime(2013, 12, 20) };
            actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The values for the second record were wrong.");
        }

        /// <summary>
        /// If we specify a custom fill character, it should be used to buffer fields in the file.
        /// </summary>
        [TestMethod]
        public void TestGetValues_CustomFillCharacter_TrimsFill()
        {
            const string text = "@@@@@@@123@@@@@@@@@@@@@@@@@@@@@@Bob@1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), 10)
                  .AddColumn(new StringColumn("name"), 25)
                  .AddColumn(new DateTimeColumn("created"), 10);
            FixedLengthParserOptions options = new FixedLengthParserOptions() { FillCharacter = '@' };
            FixedLengthParser parser = new FixedLengthParser(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);

            Assert.IsTrue(parser.Read(), "Could not read the first record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The values for the first record were wrong.");
        }
    }
}
