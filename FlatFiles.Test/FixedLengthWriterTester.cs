using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class FixedLengthWriterTester
    {
        /// <summary>
        /// When outputting a value whose length is wider than the column, it should truncate the
        /// value. If the column's Alignment is Left, then it should keep the start of the value
        /// and chop off the end.
        /// </summary>
        [TestMethod]
        public void TestWrite_LeftAlignedValueTooWide_TruncatesKeepingLeft()
        {
            var schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("Name"), new Window(5, FixedAlignment.LeftAligned));

            using (MemoryStream stream = new MemoryStream())
            {
                using (var writer = new FixedLengthWriter(stream, schema))
                {
                    writer.Write(new object[] { "1234567890" });
                    writer.Flush();

                    stream.Position = 0;  // go back to the beginning of the stream

                    FlatFileReader reader = new FlatFileReader(new FixedLengthReader(stream, schema));
                    Assert.IsTrue(reader.Read(), "The writer did not write the entities.");
                    
                    string value = reader.GetString(0);
                    
                    Assert.AreEqual("12345", value, "The value was not truncated properly.");
                    Assert.IsFalse(reader.Read(), "The writer wrote too many records.");
                }
            }
        }

        /// <summary>
        /// When outputting a value whose length is wider than the column, it should truncate the
        /// value. If the column's Alignment is Right, then it should keep the end of the value
        /// and chop off the start.
        /// </summary>
        [TestMethod]
        public void TestWrite_RightAlignedValueTooWide_TruncatesKeepingRight()
        {
            var schema = new FixedLengthSchema();
            schema.AddColumn(new StringColumn("Name"), new Window(5, FixedAlignment.RightAligned));

            using (MemoryStream stream = new MemoryStream())
            {
                using (var writer = new FixedLengthWriter(stream, schema))
                {
                    writer.Write(new object[] { "1234567890" });
                    writer.Flush();

                    stream.Position = 0;  // go back to the beginning of the stream

                    FlatFileReader reader = new FlatFileReader(new FixedLengthReader(stream, schema));
                    Assert.IsTrue(reader.Read(), "The writer did not write the entities.");

                    string value = reader.GetString(0);

                    Assert.AreEqual("67890", value, "The value was not truncated properly.");
                    Assert.IsFalse(reader.Read(), "The writer wrote too many records.");
                }
            }
        }
    }
}
