using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class FixedLengthWriterTester
    {
        [TestMethod]
        public void ShouldUseLeadingTruncationByDefault()
        {
            FixedLengthOptions options = new FixedLengthOptions();
            Assert.AreEqual(OverflowTruncationPolicy.TruncateLeading, options.TruncationPolicy);
        }

        [TestMethod]
        public void ShouldTruncateOverflow()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("Default"), new Window(5));
            schema.AddColumn(new StringColumn("Leading"), new Window(5) { TruncationPolicy = OverflowTruncationPolicy.TruncateLeading });
            schema.AddColumn(new StringColumn("Trailing"), new Window(5) { TruncationPolicy = OverflowTruncationPolicy.TruncateTrailing });
            FixedLengthOptions options = new FixedLengthOptions
            {
                TruncationPolicy = OverflowTruncationPolicy.TruncateLeading // this is the default anyway
            };

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema, options);
            writer.Write(new object[] { "Pineapple", "Pineapple", "Pineapple" });

            string output = stringWriter.ToString();
            
            string expected = "appleapplePinea" + Environment.NewLine;
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void ShouldWriteHeader()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("First"), new Window(10) { FillCharacter = '@' });
            schema.AddColumn(new StringColumn("Second"), new Window(10) { FillCharacter = '!' });
            schema.AddColumn(new StringColumn("Third"), new Window(10) { FillCharacter = '$' });
            FixedLengthOptions options = new FixedLengthOptions { IsFirstRecordHeader = true };

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema, options);
            writer.Write(new object[] { "Apple", "Grape", "Pear" });

            string output = stringWriter.ToString();

            string expected = "First@@@@@Second!!!!Third$$$$$" 
                + Environment.NewLine 
                + "Apple@@@@@Grape!!!!!Pear$$$$$$"
                + Environment.NewLine;
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void ShouldWriteHeader_IgnoredColumns()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("First"), new Window(10) { FillCharacter = '@' });
            schema.AddColumn(new IgnoredColumn(), new Window(1) { FillCharacter = '|' });
            schema.AddColumn(new StringColumn("Second"), new Window(10) { FillCharacter = '!' });
            schema.AddColumn(new IgnoredColumn(), new Window(1) { FillCharacter = '|' });
            schema.AddColumn(new StringColumn("Third"), new Window(10) { FillCharacter = '$' });
            FixedLengthOptions options = new FixedLengthOptions { IsFirstRecordHeader = true };

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema, options);
            writer.Write(new object[] { "Apple", "Grape", "Pear" });

            string output = stringWriter.ToString();

            string expected = "First@@@@@|Second!!!!|Third$$$$$"
                + Environment.NewLine
                + "Apple@@@@@|Grape!!!!!|Pear$$$$$$"
                + Environment.NewLine;
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void ShouldWriteHeader_NoRecordSeparator()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("First"), new Window(10) { FillCharacter = '@' });
            schema.AddColumn(new StringColumn("Second"), new Window(10) { FillCharacter = '!' });
            schema.AddColumn(new StringColumn("Third"), new Window(10) { FillCharacter = '$' });
            FixedLengthOptions options = new FixedLengthOptions { IsFirstRecordHeader = true, HasRecordSeparator = false };

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema, options);
            writer.Write(new object[] { "Apple", "Grape", "Pear" });

            string output = stringWriter.ToString();

            string expected = "First@@@@@Second!!!!Third$$$$$Apple@@@@@Grape!!!!!Pear$$$$$$";
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void ShouldHandleNullValues()
        {
            MemoryStream stream = new MemoryStream();

            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("NullableInt32"), new Window(5));

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema);
            writer.Write(new object[] { null });

            string output = stringWriter.ToString();
            string expected = "     " + Environment.NewLine;
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void ShouldWriteSchemaIfExplicit()
        {
            StringWriter stringWriter = new StringWriter();
            // Explicitly indicate that the first record is NOT the schema
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("Col1"), 10);
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema, new FixedLengthOptions
                                                                                   {
                IsFirstRecordHeader = false
            });
            writer.WriteSchema();  // Explicitly write the schema
            writer.Write(new string[] { "a" });

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var reader = new FixedLengthReader(stringReader, schema, new FixedLengthOptions { IsFirstRecordHeader = true });

            Assert.IsTrue(reader.Read(), "The record was not retrieved after the schema.");
            Assert.IsFalse(reader.Read(), "Encountered more than the expected number of records.");
        }
    }
}
