using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Text;

namespace FlatFileReaders.Test
{
    /// <summary>
    /// Tests the SeparatedValueParser class.
    /// </summary>
    [TestClass]
    public class SeparatedValueParserTester
    {
        /// <summary>
        /// If we try to pass a null stream to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_NullStream_Throws()
        {
            Stream stream = null;
            new SeparatedValueParser(stream);
        }

        /// <summary>
        /// If we try to pass a null stream to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_NullStream_Throws()
        {
            Stream stream = null;
            SeparatedValueParserOptions options = new SeparatedValueParserOptions();
            new SeparatedValueParser(stream, options);
        }

        /// <summary>
        /// If we try to pass a null stream to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_NullOptions_Throws()
        {
            Stream stream = new MemoryStream();
            SeparatedValueParserOptions options = null;
            new SeparatedValueParser(stream, options);
        }

        /// <summary>
        /// If we pass a single record, Read should return true once.
        /// </summary>
        [TestMethod]
        public void TestRead_SingleRecord_ReturnsTrueOnce()
        {
            Stream stream = getRecordStream("a", "b", "c");
            SeparatedValueParser parser = new SeparatedValueParser(stream);
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            string[] expected = new string[] { "a", "b", "c" };
            string[] actual = parser.GetValues();
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
            Stream stream = getRecordStream("a", "b", "c");
            SeparatedValueParser parser = new SeparatedValueParser(stream);
            string[] values = parser.GetValues();
        }

        /// <summary>
        /// We should be able to read the same values as many times as we want.
        /// </summary>
        [TestMethod]
        public void TestRead_MultipleCallsToValues_ReturnsSameValues()
        {
            Stream stream = getRecordStream("a", "b", "c");
            SeparatedValueParser parser = new SeparatedValueParser(stream);
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            string[] expected = new string[] { "a", "b", "c" };
            string[] actual = parser.GetValues();
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
            Stream stream = getRecordStream("a", "b", "c");
            SeparatedValueParser parser = new SeparatedValueParser(stream);
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            canRead = parser.Read();
            Assert.IsFalse(canRead, "We should have reached the end of the file.");
            string[] actual = parser.GetValues();
        }

        /// <summary>
        /// If we do not explicitly say that the first record is the schema, we cannot retrieve it later.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetSchema_NotExtracted_Throws()
        {
            Stream stream = getRecordStream("a", "b", "c");
            SeparatedValueParserOptions options = new SeparatedValueParserOptions() { IsFirstRecordSchema = false };
            SeparatedValueParser parser = new SeparatedValueParser(stream, options);
            parser.GetSchema();
        }

        /// <summary>
        /// If we say that the first record is the schema, we can retrieve it later on.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_Extracted_ReturnsColumnNames()
        {
            Stream stream = getRecordStream("a", "b", "c");
            SeparatedValueParserOptions options = new SeparatedValueParserOptions() { IsFirstRecordSchema = true };
            SeparatedValueParser parser = new SeparatedValueParser(stream, options);
            string[] names = parser.GetSchema();
            string[] expected = new string[] { "a", "b", "c" };
            CollectionAssert.AreEqual(expected, names, "The schema was not extracted as expected.");
        }

        private static Stream getRecordStream(params string[] values)
        {
            string joined = String.Join(",", values);
            byte[] data = Encoding.Default.GetBytes(joined);
            return new MemoryStream(data);
        }
    }
}
