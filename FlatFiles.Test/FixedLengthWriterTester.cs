using System;
using System.IO;
using System.Text;
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
            Assert.AreEqual(OverflowTruncationPolicy.TruncateLeading, options.TruncationPolicy, "TruncateLeading not the default policy.");
        }

        [TestMethod]
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
            Assert.AreEqual(expected, output, "The values were not truncated properly.");
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
            Assert.AreEqual(expected, output, "The values were not truncated properly.");
        }
    }
}
