using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class ComplexColumnTester
    {
        [TestMethod]
        public void ShouldRoundTrip()
        {
            const string message = @"Tom,Hanselman,2016-06-0426         Walking Ice,Ace
";
            byte[] data = Encoding.ASCII.GetBytes(message);
            using (MemoryStream inputStream = new MemoryStream(data))
            {
                SeparatedValueSchema outerSchema = new SeparatedValueSchema();
                outerSchema.AddColumn(new StringColumn("FirstName"));
                outerSchema.AddColumn(new StringColumn("LastName"));
                FixedLengthSchema innerSchema = new FixedLengthSchema();
                innerSchema.AddColumn(new DateTimeColumn("StartDate") { InputFormat = "yyyy-MM-dd", OutputFormat = "yyyy-MM-dd" }, 10);
                innerSchema.AddColumn(new Int32Column("Age"), 2);
                innerSchema.AddColumn(new StringColumn("StageName"), new Window(20) { Alignment = FixedAlignment.RightAligned });
                outerSchema.AddColumn(new FixedLengthComplexColumn("PlayerStats", innerSchema));
                outerSchema.AddColumn(new StringColumn("Nickname"));
                SeparatedValueReader reader = new SeparatedValueReader(inputStream, outerSchema);
                Assert.IsTrue(reader.Read(), "A record should have been read.");
                object[] values = reader.GetValues();
                Assert.AreEqual("Tom", values[0], "The FirstName was wrong.");
                Assert.AreEqual("Hanselman", values[1], "The LastName was wrong.");
                Assert.IsInstanceOfType(values[2], typeof(object[]), "The PlayerStats was not an object[].");
                object[] playerValues = (object[])values[2];
                Assert.AreEqual(new DateTime(2016, 06, 04), playerValues[0], "The StartDate was wrong.");
                Assert.AreEqual(26, playerValues[1], "The Age was not wrong.");
                Assert.AreEqual("Walking Ice", playerValues[2], "The StageName was wrong.");
                Assert.AreEqual("Ace", values[3], "The Nickname was wrong.");

                using (MemoryStream outputStream = new MemoryStream())
                {
                    using (SeparatedValueWriter writer = new SeparatedValueWriter(outputStream, outerSchema))
                    {
                        writer.Write(values);
                    }
                    outputStream.Position = 0;
                    byte[] outputData = outputStream.ToArray();
                    string output = Encoding.ASCII.GetString(outputData);
                    Assert.AreEqual(message, output, "The re-written message did not match the original message.");
                }
            }
        }
    }
}
