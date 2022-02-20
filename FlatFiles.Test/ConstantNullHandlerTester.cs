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

            object[] values = ParseValues(content);

            Assert.AreEqual(4, values.Length);
            Assert.IsNull(values[0]);
            Assert.AreEqual(5.12m, values[1]);
            Assert.IsNull(values[2]);
            Assert.AreEqual("apple", values[3]);

            string output = WriteValues(values);

            Assert.AreEqual(content, output);
        }

        private static object[] ParseValues(string content)
        {
            StringReader stringReader = new StringReader(content);
            var schema = GetSchema();
            DelimitedReader reader = new DelimitedReader(stringReader, schema);
            Assert.IsTrue(reader.Read(), "The record could not be read.");
            object[] values = reader.GetValues();
            Assert.IsFalse(reader.Read(), "Too many records were read.");
            return values;
        }

        private static string WriteValues(object[] values)
        {
            var schema = GetSchema();
            StringWriter stringWriter = new StringWriter();
            DelimitedWriter writer = new DelimitedWriter(stringWriter, schema);
            writer.Write(values);

            return stringWriter.ToString();
        }

        private static DelimitedSchema GetSchema()
        {
            var nullHandler = NullFormatter.ForValue("----");

            DelimitedSchema schema = new DelimitedSchema();
            schema.AddColumn(new StringColumn("Name") { NullFormatter = nullHandler });
            schema.AddColumn(new DecimalColumn("Cost") { NullFormatter = nullHandler, FormatProvider = CultureInfo.InvariantCulture });
            schema.AddColumn(new SingleColumn("Available") { NullFormatter = nullHandler });
            schema.AddColumn(new StringColumn("Vendor") { NullFormatter = nullHandler });

            return schema;
        }

        [TestMethod]
        public void ShouldTreatConstantAsNull_TypeMapper()
        {
            var nullHandler = NullFormatter.ForValue("----");
            var mapper = DelimitedTypeMapper.Define<Product>();
            mapper.Property(p => p.Name).ColumnName("name").NullFormatter(nullHandler);
            mapper.Property(p => p.Cost).ColumnName("cost").NullFormatter(nullHandler).FormatProvider(CultureInfo.InvariantCulture);
            mapper.Property(p => p.Available).ColumnName("available").NullFormatter(nullHandler);
            mapper.Property(p => p.Vendor).ColumnName("vendor").NullFormatter(nullHandler);

            string content = "----,5.12,----,apple" + Environment.NewLine;
            StringReader stringReader = new StringReader(content);
            var products = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, products.Length);

            Product product = products.Single();
            Assert.IsNull(product.Name);
            Assert.AreEqual(5.12m, product.Cost);
            Assert.IsNull(product.Available);
            Assert.AreEqual("apple", product.Vendor);

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, products);
            string output = stringWriter.ToString();

            Assert.AreEqual(content, output);
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
