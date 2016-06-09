using System;
using System.Globalization;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
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
            StringReader stringReader = new StringReader(content);
            var schema = getSchema();
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema);
            Assert.IsTrue(reader.Read(), "The record could not be read.");
            object[] values = reader.GetValues();
            Assert.IsFalse(reader.Read(), "Too many records were read.");
            return values;
        }

        private static string writeValues(object[] values)
        {
            var schema = getSchema();
            StringWriter stringWriter = new StringWriter();
            SeparatedValueWriter writer = new SeparatedValueWriter(stringWriter, schema);
            writer.Write(values);

            return stringWriter.ToString();
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
            mapper.Property(p => p.Cost).ColumnName("cost").NullHandler(nullHandler).FormatProvider(CultureInfo.InvariantCulture);
            mapper.Property(p => p.Available).ColumnName("available").NullHandler(nullHandler);
            mapper.Property(p => p.Vendor).ColumnName("vendor").NullHandler(nullHandler);

            string content = "----,5.12,----,apple" + Environment.NewLine;
            StringReader stringReader = new StringReader(content);
            var products = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, products.Count(), "The wrong number of products were found.");

            Product product = products.Single();
            Assert.IsNull(product.Name, "The name was not interpreted as null.");
            Assert.AreEqual(5.12m, product.Cost, "The cost was not read correctly.");
            Assert.IsNull(product.Available, "The available was not interpreted as null.");
            Assert.AreEqual("apple", product.Vendor, "The vendor was not read correctly.");

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, products);
            string output = stringWriter.ToString();

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
