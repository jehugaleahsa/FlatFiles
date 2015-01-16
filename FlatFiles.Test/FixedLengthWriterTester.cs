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
            MemoryStream stream = new MemoryStream();

            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("Default"), new Window(5));
            schema.AddColumn(new StringColumn("Leading"), new Window(5) { TruncationPolicy = OverflowTruncationPolicy.TruncateLeading });
            schema.AddColumn(new StringColumn("Trailing"), new Window(5) { TruncationPolicy = OverflowTruncationPolicy.TruncateTrailing });
            FixedLengthOptions options = new FixedLengthOptions();
            options.TruncationPolicy = OverflowTruncationPolicy.TruncateLeading; // this is the default anyway
            using (FixedLengthWriter writer = new FixedLengthWriter(stream, schema, options))
            {
                writer.Write(new object[] { "Pineapple", "Pineapple", "Pineapple" });
            }

            stream.Position = 0;

            string output = Encoding.Default.GetString(stream.ToArray());
            string expected = "appleapplePinea" + Environment.NewLine;
            Assert.AreEqual(expected, output, "The values were not truncated properly.");
        }
    }
}
