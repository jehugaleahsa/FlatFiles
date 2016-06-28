using System;
using System.Globalization;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class ConstantNullHandlerTester
    {
        [Fact]
        public void ShouldTreatConstantAsNull()
        {
            string content = "----,5.12,----,apple" + Environment.NewLine;            

            object[] values = parseValues(content);

            Assert.Equal(4, values.Length);
            Assert.Null(values[0]);
            Assert.Equal(5.12m, values[1]);
            Assert.Null(values[2]);
            Assert.Equal("apple", values[3]);

            string output = writeValues(values);

            Assert.Equal(content, output);
        }

        private static object[] parseValues(string content)
        {
            StringReader stringReader = new StringReader(content);
            var schema = getSchema();
            SeparatedValueReader reader = new SeparatedValueReader(stringReader, schema);
            Assert.True(reader.Read(), "The record could not be read.");
            object[] values = reader.GetValues();
            Assert.False(reader.Read(), "Too many records were read.");
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

        [Fact]
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
            Assert.Equal(1, products.Count());

            Product product = products.Single();
            Assert.Null(product.Name);
            Assert.Equal(5.12m, product.Cost);
            Assert.Null(product.Available);
            Assert.Equal("apple", product.Vendor);

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, products);
            string output = stringWriter.ToString();

            Assert.Equal(content, output);
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
