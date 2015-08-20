using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class ConstantNullHandlerTester
    {
        [TestMethod]
        public void ShouldTreatConstantAsNull()
        {
            string content = "----,5.12,----,apple" + Environment.NewLine;            

            object[] values = parseValues(content);

            Assert.AreEqual(4, values.Length, "The wrong number of values were read.");
            Assert.IsNull(values[0], "The first value was not interpreted as null.");
            Assert.AreEqual(5.12m, values[1], "The second value was not read correctly.");
            Assert.IsNull(values[2], "The third value was not read correctly.");
            Assert.AreEqual("apple", values[3], "The forth value was not read correctly.");

            string output = writeValues(values);

            Assert.AreEqual(content, output, "The null handler was not respected when writing null.");
        }

        private static object[] parseValues(string content)
        {
            byte[] encoded = Encoding.Default.GetBytes(content);
            var schema = getSchema();
            using (MemoryStream stream = new MemoryStream(encoded))
            using (SeparatedValueReader reader = new SeparatedValueReader(stream, schema))
            {
                Assert.IsTrue(reader.Read(), "The record could not be read.");
                object[] values = reader.GetValues();
                Assert.IsFalse(reader.Read(), "Too many records were read.");
                return values;
            }
        }

        private static string writeValues(object[] values)
        {
            var schema = getSchema();
            using (MemoryStream stream = new MemoryStream())
            {
                using (SeparatedValueWriter writer = new SeparatedValueWriter(stream, schema))
                {
                    writer.Write(values);
                }

                stream.Position = 0;
                byte[] encoded = stream.ToArray();
                return Encoding.Default.GetString(encoded);
            }
        }

        private static SeparatedValueSchema getSchema()
        {
            var nullHandler = ConstantNullHandler.For("----");

            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("Name") { NullHandler = nullHandler });
            schema.AddColumn(new DecimalColumn("Cost") { NullHandler = nullHandler });
            schema.AddColumn(new SingleColumn("Available") { NullHandler = nullHandler });
            schema.AddColumn(new StringColumn("Vendor") { NullHandler = nullHandler });

            return schema;
        }
    }
}
