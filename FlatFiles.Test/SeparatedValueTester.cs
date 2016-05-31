using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class SeparatedValueTester
    {
        [TestMethod]
        public void ShouldNotFindRecordsInEmptyFile()
        {
            string source = String.Empty;
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                Assert.IsFalse(reader.Read(), "No records should be read from an empty file.");
            }
        }

        [TestMethod]
        public void ShouldFindOneRecordOneColumn_CharactersFollowedByEndOfStream()
        {
            string source = "Hello";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "Hello" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldFindOneRecordTwoColumn_CharactersFollowedByEndOfStream()
        {
            string source = "Hello,World";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "Hello", "World" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldFindTwoRecordsOneColumn()
        {
            string source = "Hello\r\nWorld\r\n";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "Hello" },
                    new object[] { "World" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldFindTwoRecordsOneColumn_MissingClosingRecordSeparator()
        {
            string source = "Hello\r\nWorld";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "Hello" },
                    new object[] { "World" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldFindOneRecordTwoColumn_EOROverlapsEOT()
        {
            string source = "a\rb\r\n";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n", Separator = "\r" };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a", "b" },
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldFindTwoRecordsOneColumn_EOROverlapsEOT()
        {
            string source = "a\r\nb";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n", Separator = "\r" };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a" },
                    new object[] { "b" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldFindOneRecordTwoColumn_EOTOverlapsEOR()
        {
            string source = "a\r\nb\r";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r", Separator = "\r\n" };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a", "b" },
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldFindTwoRecordsOneColumn_EOTOverlapsEOR()
        {
            string source = "a\rb";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r", Separator = "\r\n" };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a" },
                    new object[] { "b" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldSeparateBlanks()
        {
            string source = ",";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { String.Empty, String.Empty },
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldSeparateBlanksAcrossRecords()
        {
            string source = ",,\r\n,,";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { String.Empty, String.Empty, String.Empty },
                    new object[] { String.Empty, String.Empty, String.Empty },
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldHandleSingleEmptyRecord()
        {
            string source = "\r\n";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { String.Empty },
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldHandleMultipleEmptyRecords()
        {
            string source = "\r\n\r\n";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { String.Empty },
                    new object[] { String.Empty },
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldStripLeadingWhitespace()
        {
            string source = " a";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldStripLeadingWhitespace_MultipleSpaces_TwoColumn()
        {
            string source = "  a, \t\n  b";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a", "b" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldStripTrailingWhitespace()
        {
            string source = "a ";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldStripTrailingWhitespace_MultipleSpaces_TwoColumn()
        {
            string source = "a  ,b \t\n  ";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a", "b" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldStripLeadingAndTrailingWhitespace()
        {
            string source = " a ";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldStripLeadingAndTrailingWhitespace_MultipleSpaces_TwoColumn()
        {
            string source = "  a  , \t\n  b \t\n  ";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a", "b" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldFindOneColumnOneRecordIfAllWhitespace()
        {
            string source = " \t\n\r ";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { String.Empty }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldNotStripEmbeddedWhitespace()
        {
            string source = " a b ";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a b" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldHandleDoubleWhitespaceAsSeparator()
        {
            string source = " a  b ";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, Separator = "  " };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a", "b" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldIgnoreInvalidSeparatorSharingSharedPrefix()
        {
            string source = "axxcb";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, Separator = "xxa", RecordSeparator = "xxb" };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "axxcb" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldExtractQuotedValue()
        {
            string source = "'a'";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions()
                {
                    IsFirstRecordSchema = false,
                    Quote = '\''
                };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldIncludeSpacesBetweenQuotes()
        {
            string source = "' a  '";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions()
                {
                    IsFirstRecordSchema = false,
                    Quote = '\''
                };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { " a  " }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldIgnoreSeparatorsBetweenQuotes()
        {
            string source = "'a,b'";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions()
                {
                    IsFirstRecordSchema = false,
                    Quote = '\''
                };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a,b" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldHandleEscapedQuotesWithinQuotes()
        {
            string source = "'a''b'";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions()
                {
                    IsFirstRecordSchema = false,
                    Quote = '\''
                };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a'b" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldIgnoreLeadingWhiteSpaceBeforeQuote()
        {
            string source = "   'a'";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions()
                {
                    IsFirstRecordSchema = false,
                    Quote = '\''
                };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        public void ShouldIgnoreTrailingWhiteSpaceAfterQuote()
        {
            string source = "'a' ";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions()
                {
                    IsFirstRecordSchema = false,
                    Quote = '\''
                };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] { "a" }
                };
                assertRecords(expected, reader);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FlatFileException))]
        public void ShouldThrowSyntaxExceptionIfQuoteFollowedByEOS()
        {
            string source = "'";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions()
                {
                    IsFirstRecordSchema = false,
                    Quote = '\''
                };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                reader.Read();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(FlatFileException))]
        public void ShouldThrowSyntaxExceptionIfQuoteFollowedByNonSeparator()
        {
            string source = "'a'b";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions()
                {
                    IsFirstRecordSchema = false,
                    Quote = '\''
                };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                reader.Read();
            }
        }

        [TestMethod]
        public void ShouldIgnoreRecordSeparatorsWithinQuotes()
        {
            string source = @"'John','Smith','123 Playtown Place', 'Grangewood','CA' ,12345,'John likes to travel to far away places.
His favorite travel spots are Tannis, Venice and Chicago.
When he''s not traveling, he''s at home with his lovely wife, children and leather armchair.'
Mary,Smith,'1821 Grover''s Village',West Chattingham,WA,43221,'Likes cats.'";
            using (MemoryStream stream = getStream(source))
            {
                SeparatedValueOptions options = new SeparatedValueOptions()
                {
                    IsFirstRecordSchema = false,
                    Quote = '\''
                };
                SeparatedValueReader reader = new SeparatedValueReader(stream, options);
                object[][] expected = new object[][]
                {
                    new object[] 
                    { "John", "Smith", "123 Playtown Place", "Grangewood", "CA", "12345", @"John likes to travel to far away places.
His favorite travel spots are Tannis, Venice and Chicago.
When he's not traveling, he's at home with his lovely wife, children and leather armchair."
                    },
                    new object[] { "Mary", "Smith", "1821 Grover's Village", "West Chattingham", "WA", "43221", "Likes cats." }
                };
                assertRecords(expected, reader);
            }
        }

        private static MemoryStream getStream(string value, Encoding encoding = null)
        {
            Encoding actualEncoding = encoding ?? Encoding.Default;
            byte[] data = actualEncoding.GetBytes(value);
            return new MemoryStream(data);
        }

        private void assertRecords(object[][] expected, SeparatedValueReader reader)
        {
            for (int recordIndex = 0; recordIndex != expected.Length; ++recordIndex)
            {
                Assert.IsTrue(reader.Read(), "The record could not be read (Record {0}).", recordIndex);
                object[] actualValues = reader.GetValues();
                object[] expectedValues = expected[recordIndex];
                assertRecord(expectedValues, actualValues, recordIndex);
            }
            Assert.IsFalse(reader.Read(), "There were more records read than expected.");
        }

        private static void assertRecord(object[] expected, object[] actual, int record)
        {
            Assert.AreEqual(expected.Length, actual.Length, "The actual record had a different number of columns than expected (Record {0}).", record);
            for (int index = 0; index != expected.Length; ++index)
            {
                assertToken(expected[index], actual[index], index);
            }
        }

        private static void assertToken(object expected, object actual, int column)
        {
            Type expectedType = expected.GetType();
            Assert.IsInstanceOfType(actual, expectedType, "The actual value did not match the type of the expected value (Column {0}).", column);
            Assert.AreEqual(expected, actual, "The actual value did not equal the expected value (Column {0}).", column);
        }
    }
}
