using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            Assert.AreEqual(expected, output, "The wrong data was written.");
        }
    }
}
