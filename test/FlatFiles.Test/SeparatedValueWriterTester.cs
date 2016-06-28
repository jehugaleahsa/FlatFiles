using System;
using System.IO;
using Xunit;

namespace FlatFiles.Test
{
    public class SeparatedValueWriterTester
    {
        [Fact]
        public void ShouldNotWriteSchemaIfNoSchemaProvided()
        {
            StringWriter stringWriter = new StringWriter();
            SeparatedValueWriter writer = new SeparatedValueWriter(stringWriter, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            writer.Write(new string[] { "a" });

            string output = stringWriter.ToString();
            string expected = "a" + Environment.NewLine;
            Assert.Equal(expected, output);
        }
    }
}
