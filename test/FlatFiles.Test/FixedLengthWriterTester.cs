using System;
using System.IO;
using Xunit;

namespace FlatFiles.Test
{
    public class FixedLengthWriterTester
    {
        [Fact]
        public void ShouldUseLeadingTruncationByDefault()
        {
            FixedLengthOptions options = new FixedLengthOptions();
            Assert.Equal(OverflowTruncationPolicy.TruncateLeading, options.TruncationPolicy);
        }

        [Fact]
        public void ShouldTruncateOverflow()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("Default"), new Window(5));
            schema.AddColumn(new StringColumn("Leading"), new Window(5) { TruncationPolicy = OverflowTruncationPolicy.TruncateLeading });
            schema.AddColumn(new StringColumn("Trailing"), new Window(5) { TruncationPolicy = OverflowTruncationPolicy.TruncateTrailing });
            FixedLengthOptions options = new FixedLengthOptions();
            options.TruncationPolicy = OverflowTruncationPolicy.TruncateLeading; // this is the default anyway

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema, options);
            writer.Write(new object[] { "Pineapple", "Pineapple", "Pineapple" });

            string output = stringWriter.ToString();
            
            string expected = "appleapplePinea" + Environment.NewLine;
            Assert.Equal(expected, output);
        }

        [Fact]
        public void ShouldWriteHeader()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("First"), new Window(10) { FillCharacter = '@' });
            schema.AddColumn(new StringColumn("Second"), new Window(10) { FillCharacter = '!' });
            schema.AddColumn(new StringColumn("Third"), new Window(10) { FillCharacter = '$' });
            FixedLengthOptions options = new FixedLengthOptions() { IsFirstRecordHeader = true };

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema, options);
            writer.Write(new object[] { "Apple", "Grape", "Pear" });

            string output = stringWriter.ToString();

            string expected = "First@@@@@Second!!!!Third$$$$$" 
                + Environment.NewLine 
                + "Apple@@@@@Grape!!!!!Pear$$$$$$"
                + Environment.NewLine;
            Assert.Equal(expected, output);
        }

        [Fact]
        public void ShouldWriteHeader_IgnoredColumns()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("First"), new Window(10) { FillCharacter = '@' });
            schema.AddColumn(new IgnoredColumn(), new Window(1) { FillCharacter = '|' });
            schema.AddColumn(new StringColumn("Second"), new Window(10) { FillCharacter = '!' });
            schema.AddColumn(new IgnoredColumn(), new Window(1) { FillCharacter = '|' });
            schema.AddColumn(new StringColumn("Third"), new Window(10) { FillCharacter = '$' });
            FixedLengthOptions options = new FixedLengthOptions() { IsFirstRecordHeader = true };

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema, options);
            writer.Write(new object[] { "Apple", "Grape", "Pear" });

            string output = stringWriter.ToString();

            string expected = "First@@@@@|Second!!!!|Third$$$$$"
                + Environment.NewLine
                + "Apple@@@@@|Grape!!!!!|Pear$$$$$$"
                + Environment.NewLine;
            Assert.Equal(expected, output);
        }

        [Fact]
        public void ShouldWriteHeader_NoRecordSeparator()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("First"), new Window(10) { FillCharacter = '@' });
            schema.AddColumn(new StringColumn("Second"), new Window(10) { FillCharacter = '!' });
            schema.AddColumn(new StringColumn("Third"), new Window(10) { FillCharacter = '$' });
            FixedLengthOptions options = new FixedLengthOptions() { IsFirstRecordHeader = true, HasRecordSeparator = false };

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter writer = new FixedLengthWriter(stringWriter, schema, options);
            writer.Write(new object[] { "Apple", "Grape", "Pear" });

            string output = stringWriter.ToString();

            string expected = "First@@@@@Second!!!!Third$$$$$Apple@@@@@Grape!!!!!Pear$$$$$$";
            Assert.Equal(expected, output);
        }

        [Fact]
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
            Assert.Equal(expected, output);
        }
    }
}
