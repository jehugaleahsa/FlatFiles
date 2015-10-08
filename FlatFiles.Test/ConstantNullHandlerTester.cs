using System;
using System.IO;
using System.Linq;
using System.Text;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;

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
            schema.AddColumn(new DecimalColumn("Cost") { NullHandler = nullHandler, FormatProvider = CultureInfo.InvariantCulture });
            schema.AddColumn(new SingleColumn("Available") { NullHandler = nullHandler });
            schema.AddColumn(new StringColumn("Vendor") { NullHandler = nullHandler });

            return schema;
        }

        [TestMethod]
        public void ShouldTreatConstantAsNull_TypeMapper()
        {
            var nullHandler = ConstantNullHandler.For("----");
            var mapper = SeparatedValueTypeMapper.Define<Product>();
            mapper.Property(p => p.Name).ColumnName("name").NullHandler(nullHandler);
            mapper.Property(p => p.Cost).ColumnName("cost").NullHandler(nullHandler);
            mapper.Property(p => p.Cost).ColumnName("cost").FormatProvider(CultureInfo.InvariantCulture);
            mapper.Property(p => p.Available).ColumnName("available").NullHandler(nullHandler);
            mapper.Property(p => p.Vendor).ColumnName("vendor").NullHandler(nullHandler);

            string content = "----,5.12,----,apple" + Environment.NewLine;
            byte[] encoded = Encoding.Default.GetBytes(content);
            MemoryStream inputStream = new MemoryStream(encoded);
            var products = mapper.Read(inputStream);
            Assert.AreEqual(1, products.Count(), "The wrong number of products were found.");

            Product product = products.Single();
            Assert.IsNull(product.Name, "The name was not interpreted as null.");
            Assert.AreEqual(5.12m, product.Cost, "The cost was not read correctly.");
            Assert.IsNull(product.Available, "The available was not interpreted as null.");
            Assert.AreEqual("apple", product.Vendor, "The vendor was not read correctly.");

            MemoryStream outputStream = new MemoryStream();
            mapper.Write(outputStream, products);
            outputStream.Position = 0;
            string output = Encoding.Default.GetString(outputStream.ToArray());

            Assert.AreEqual(content, output, "The null handler was not respected when writing null.");
        }

        public class Product
        {
            public string Name { get; set; }

            public decimal? Cost { get; set; }

            public float? Available { get; set; }

            public string Vendor { get; set; }
        }
    }
}
