using System;
using System.IO;
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
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            Assert.IsFalse(reader.Read(), "No records should be read from an empty file.");
        }

        [TestMethod]
        public void ShouldFindOneRecordOneColumn_CharactersFollowedByEndOfStream()
        {
            string source = "Hello";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "Hello" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldFindOneRecordTwoColumn_CharactersFollowedByEndOfStream()
        {
            string source = "Hello,World";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "Hello", "World" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldFindTwoRecordsOneColumn()
        {
            string source = "Hello\r\nWorld\r\n";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "Hello" },
                new object[] { "World" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldFindTwoRecordsOneColumn_MissingClosingRecordSeparator()
        {
            string source = "Hello\r\nWorld";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "Hello" },
                new object[] { "World" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldFindOneRecordTwoColumn_EOROverlapsEOT()
        {
            string source = "a\rb\r\n";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n", Separator = "\r" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a", "b" },
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldFindTwoRecordsOneColumn_EOROverlapsEOT()
        {
            string source = "a\r\nb";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n", Separator = "\r" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a" },
                new object[] { "b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldFindOneRecordTwoColumn_EOTOverlapsEOR()
        {
            string source = "a\r\nb\r";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r", Separator = "\r\n" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a", "b" },
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldFindTwoRecordsOneColumn_EOTOverlapsEOR()
        {
            string source = "a\rb";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r", Separator = "\r\n" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a" },
                new object[] { "b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldSeparateBlanks()
        {
            string source = ",";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { null, null },
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldSeparateBlanksAcrossRecords()
        {
            string source = ",,\r\n,,";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { null, null, null },
                new object[] { null, null, null },
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldHandleSingleEmptyRecord()
        {
            string source = "\r\n";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { null },
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldHandleMultipleEmptyRecords()
        {
            string source = "\r\n\r\n";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { null },
                new object[] { null },
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldStripLeadingWhitespace()
        {
            string source = " a";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldPreserveLeadingWhitespaceIfConfigured()
        {
            string source = " a";
            StringReader stringReader = new StringReader(source);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("a") { Trim = false });
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, PreserveWhiteSpace = true };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema, options);
            object[][] expected = new object[][]
            {
                new object[] { " a" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldStripLeadingWhitespace_MultipleSpaces_TwoColumn()
        {
            string source = "  a, \t\n  b";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a", "b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldPreserveLeadingWhitespaceIfConfigured_MultipleSpaces_TwoColumn()
        {
            string source = "  a, \t\n  b";
            StringReader stringReader = new StringReader(source);
            //SeparatedValueSchema schema = new SeparatedValueSchema();
            //schema.AddColumn(new StringColumn("a") { Trim = false });
            //schema.AddColumn(new StringColumn("b") { Trim = false });
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n", PreserveWhiteSpace = true };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, /*schema,*/ options);
            object[][] expected = new object[][]
            {
                new object[] { "  a", " \t\n  b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldStripTrailingWhitespace()
        {
            string source = "a ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldPreserveTrailingWhitespaceIfConfigured()
        {
            string source = "a ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("a") { Trim = false });
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, PreserveWhiteSpace = true };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema, options);
            object[][] expected = new object[][]
            {
                new object[] { "a " }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldStripTrailingWhitespace_MultipleSpaces_TwoColumn()
        {
            string source = "a  ,b \t\n  ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a", "b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldPreserveTrailingWhitespaceIfConfigured_MultipleSpaces_TwoColumn()
        {
            string source = "a  ,b \t\n  ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("a") { Trim = false });
            schema.AddColumn(new StringColumn("b") { Trim = false });
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n", PreserveWhiteSpace = true };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema, options);
            object[][] expected = new object[][]
            {
                new object[] { "a  ", "b \t\n  " }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldStripLeadingAndTrailingWhitespace()
        {
            string source = " a ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldPreserveLeadingAndTrailingWhitespaceIfConfigured()
        {
            string source = " a ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("a") { Trim = false });
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, PreserveWhiteSpace = true };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema, options);
            object[][] expected = new object[][]
            {
                new object[] { " a " }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldStripLeadingAndTrailingWhitespace_MultipleSpaces_TwoColumn()
        {
            string source = "  a  , \t\n  b \t\n  ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a", "b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldPreserveLeadingAndTrailingWhitespaceIfConfigured_MultipleSpaces_TwoColumn()
        {
            string source = "  a  , \t\n  b \t\n  ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("a") { Trim = false });
            schema.AddColumn(new StringColumn("b") { Trim = false });
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n", PreserveWhiteSpace = true };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema, options);
            object[][] expected = new object[][]
            {
                new object[] { "  a  ", " \t\n  b \t\n  " }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldFindOneColumnOneRecordIfAllWhitespace()
        {
            string source = " \t\n\r ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { null }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldPreserveWhiteSpaceIfConfigured_AllWhitespace()
        {
            string source = " \t\n\r ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("a") { Trim = false, NullFormatter = NullFormatter.ForValue(null) });
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, RecordSeparator = "\r\n", PreserveWhiteSpace = true };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema, options);
            object[][] expected = new object[][]
            {
                new object[] { " \t\n\r " }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldNotStripEmbeddedWhitespace()
        {
            string source = " a b ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldNotStripEmbeddedWhitespace_PreservingWhiteSpace()
        {
            string source = " a b ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("a") { Trim = false });
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, PreserveWhiteSpace = true };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema, options);
            object[][] expected = new object[][]
            {
                new object[] { " a b " }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldHandleDoubleWhitespaceAsSeparator()
        {
            string source = " a  b ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, Separator = "  " };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a", "b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldIgnoreInvalidSeparatorSharingSharedPrefix()
        {
            string source = "axxcb";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, Separator = "xxa", RecordSeparator = "xxb" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "axxcb" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldHandleLongUndoOperation()
        {
            string source = "axxxb";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false, Separator = "xxxx" };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "axxxb" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldExtractQuotedValue()
        {
            string source = "'a'";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Quote = '\''
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldIncludeSpacesBetweenQuotes()
        {
            string source = "' a  '";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Quote = '\''
            };
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("a") { Trim = false });
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema, options);
            object[][] expected = new object[][]
            {
                new object[] { " a  " }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldIgnoreSeparatorsBetweenQuotes()
        {
            string source = "'a,b'";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Quote = '\''
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a,b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldHandleEscapedQuotesWithinQuotes()
        {
            string source = "'a''b'";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Quote = '\''
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a'b" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldIgnoreLeadingWhiteSpaceBeforeQuote()
        {
            string source = "   'a'";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Quote = '\''
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldIgnoreTrailingWhiteSpaceAfterQuote()
        {
            string source = "'a' ";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Quote = '\''
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "a" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldThrowSyntaxExceptionIfQuoteFollowedByEOS()
        {
            string source = "'";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Quote = '\''
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            Assert.ThrowsException<RecordProcessingException>(() => reader.Read());
        }

        [TestMethod]
        public void ShouldThrowSyntaxExceptionIfQuoteFollowedByNonSeparator()
        {
            string source = "'a'b";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Quote = '\''
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            Assert.ThrowsException<RecordProcessingException>(() => reader.Read());
        }

        [TestMethod]
        public void ShouldIgnoreRecordSeparatorsWithinQuotes()
        {
            string source = @"'John','Smith','123 Playtown Place', 'Grangewood','CA' ,12345,'John likes to travel to far away places.
His favorite travel spots are Tannis, Venice and Chicago.
When he''s not traveling, he''s at home with his lovely wife, children and leather armchair.'
Mary,Smith,'1821 Grover''s Village',West Chattingham,WA,43221,'Likes cats.'";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Quote = '\''
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
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

        [TestMethod]
        public void ShouldHandleSeparatorAfterQuoteIfPreservingWhiteSpace()
        {
            string source = "26087,C Country C,,1,3,7,Randy E,(555) 555-5500,,\"P.O.Box 60,\",,,Woodsland,CA,56281,,,,0292315c-0daa-df11-9397-0019b9e7d4cd,,0,8713cbdd-fb50-dc11-a545-000423c05bf1,40,79527,,False";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                PreserveWhiteSpace = true
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "26087","C Country C",null,"1","3","7","Randy E","(555) 555-5500",null,"P.O.Box 60,",null,null,"Woodsland","CA","56281",null,null,null,"0292315c-0daa-df11-9397-0019b9e7d4cd",null,"0","8713cbdd-fb50-dc11-a545-000423c05bf1","40","79527",null,"False" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldHandleEmbeddedQuotes()
        {
            string source = "x\ty\tabc\"def\tz";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Separator = "\t"
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] { "x", "y", "abc\"def", "z" }
            };
            assertRecords(expected, reader);
        }

        [TestMethod]
        public void ShouldHandleEmbeddedQuotes2()
        {
            string source = "DebtConversionConvertedInstrumentAmount1\tus-gaap/2019" +
                "\t0\t0\tmonetary\tD\tC\tDebt Conversion, Converted Instrument, Amount" +
                "\tThe value of the financial instrument(s) that the original debt is being converted into in a noncash (or part noncash) transaction. \"Part noncash refers to that portion of the transaction not resulting in cash receipts or cash payments in the period.";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Separator = "\t",
                PreserveWhiteSpace = false,
                QuoteBehavior = QuoteBehavior.Never
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] 
                { 
                    "DebtConversionConvertedInstrumentAmount1",
                    "us-gaap/2019", 
                    "0", 
                    "0",
                    "monetary",
                    "D",
                    "C",
                    "Debt Conversion, Converted Instrument, Amount",
                    "The value of the financial instrument(s) that the original debt is being converted into in a noncash (or part noncash) transaction. \"Part noncash refers to that portion of the transaction not resulting in cash receipts or cash payments in the period."
                }
            };
            assertRecords(expected, reader);
        }

        //[TestMethod]
        public void ShouldHandleNonTerminatingEmbeddedQuotes()
        {
            string source = "This\tis\t\"not\"the\tend\"\tof\tthe\tmessage";
            StringReader stringReader = new StringReader(source);
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                Separator = "\t"
            };
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, options);
            object[][] expected = new object[][]
            {
                new object[] 
                { 
                    "This", "is", "not\"the\tend", "of", "the", "message"
                }
            };
            assertRecords(expected, reader);
        }

        private void assertRecords(object[][] expected, SeparatedValueReader reader)
        {
            for (int recordIndex = 0; recordIndex != expected.Length; ++recordIndex)
            {
                Assert.IsTrue(reader.Read(), String.Format("The record could not be read (Record {0}).", recordIndex));
                object[] actualValues = reader.GetValues();
                object[] expectedValues = expected[recordIndex];
                assertRecord(expectedValues, actualValues, recordIndex);
            }
            Assert.IsFalse(reader.Read(), "There were more records read than expected.");
        }

        private static void assertRecord(object[] expected, object[] actual, int record)
        {
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
