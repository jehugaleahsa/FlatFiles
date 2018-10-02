using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class RecordErrorHandlingTester
    {
        [TestMethod]
        public void ShouldIgnoreBadRows()
        {
            const string data = @"0,2018-02-30
1,2018-10-02,{FC6AE158-F5E9-49CC-A76A-E48F3FBE6BC1}";
            var stringReader = new StringReader(data);
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("Int32"));
            schema.AddColumn(new DateTimeColumn("DateTime"));
            schema.AddColumn(new GuidColumn("Guid"));
            var csvReader = new SeparatedValueReader(stringReader, schema);
            csvReader.RecordError += (sender, e) =>
            {
                Assert.IsNotNull(e.RecordContext, "The record context was null");
                Assert.AreEqual(e.RecordContext.PhysicalRecordNumber, 1);
                Assert.IsNotNull(e.Exception, "The exception was null.");
                e.IsHandled = true;
            };
            Assert.IsTrue(csvReader.Read(), "The second record could not be read.");
            Assert.IsFalse(csvReader.Read(), "Too many records were read -- probably the first record was not skipped.");
        }
    }
}
