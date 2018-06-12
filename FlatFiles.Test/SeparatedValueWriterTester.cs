using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class SeparatedValueWriterTester
    {
        [TestMethod]
        public void ShouldNotWriteSchemaIfNoSchemaProvided()
        {
            StringWriter stringWriter = new StringWriter();
            SeparatedValueWriter writer = new SeparatedValueWriter(stringWriter, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            writer.Write(new string[] { "a" });

            string output = stringWriter.ToString();
            string expected = "a" + Environment.NewLine;
            Assert.AreEqual(expected, output);
        }

        [TestMethod]
        public void ShouldWriteSchemaIfExplicit()
        {
            StringWriter stringWriter = new StringWriter();
            // Explicitly indicate that the first record is NOT the schema
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("Col1"));
            SeparatedValueWriter writer = new SeparatedValueWriter(stringWriter, schema, new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false
            });
            writer.WriteSchema();  // Explicitly write the schema
            writer.Write(new string[] { "a" });

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var reader = new SeparatedValueReader(stringReader, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            var parsedSchema = reader.GetSchema();
            Assert.AreEqual(schema.ColumnDefinitions.Count, parsedSchema.ColumnDefinitions.Count);
            Assert.AreEqual(schema.ColumnDefinitions[0].ColumnName, parsedSchema.ColumnDefinitions[0].ColumnName);

            Assert.IsTrue(reader.Read(), "The record was not retrieved after the schema.");
            Assert.IsFalse(reader.Read(), "Encountered more than the expected number of records.");
        }
    }
}
