using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class ColumnErrorHandlingTester
    {
        [TestMethod]
        public void ShouldSubstituteBadValues_CSV()
        {
            const string data = @"ABC,2018-02-30,{1234-5678-9123-000000}";
            var stringReader = new StringReader(data);
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("Int32"));
            schema.AddColumn(new DateTimeColumn("DateTime"));
            schema.AddColumn(new GuidColumn("Guid"));
            var csvReader = new SeparatedValueReader(stringReader, schema);
            csvReader.ColumnError += (sender, e) =>
            {
                if (e.ColumnContext.ColumnDefinition.ColumnName == "Int32")
                {
                    e.Substitution = 1;
                    e.IsHandled = true;
                }
                else if (e.ColumnContext.ColumnDefinition.ColumnName == "DateTime")
                {
                    e.Substitution = new DateTime(2018, 07, 08);
                    e.IsHandled = true;
                }
                else if (e.ColumnContext.ColumnDefinition.ColumnName == "Guid")
                {
                    e.Substitution = Guid.Empty;
                    e.IsHandled = true;
                }
            };
            Assert.IsTrue(csvReader.Read(), "Could not read the first record.");
            var values = csvReader.GetValues();
            var expected = new object[] { 1, new DateTime(2018, 07, 08), Guid.Empty };
            CollectionAssert.AreEqual(expected, values, "The wrong values were substituted.");
            Assert.IsFalse(csvReader.Read(), "Read too many records.");
        }

        [TestMethod]
        public void ShouldSubstituteBadValues_FixedLength()
        {
            const string data = @"ABC  2018-02-30{1234-5678-9123-000000}         ";
            var stringReader = new StringReader(data);
            var schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("Int32"), 5);
            schema.AddColumn(new DateTimeColumn("DateTime"), 10);
            schema.AddColumn(new GuidColumn("Guid"), 32);
            var csvReader = new FixedLengthReader(stringReader, schema);
            csvReader.ColumnError += (sender, e) =>
            {
                if (e.ColumnContext.ColumnDefinition.ColumnName == "Int32")
                {
                    e.Substitution = 1;
                    e.IsHandled = true;
                }
                else if (e.ColumnContext.ColumnDefinition.ColumnName == "DateTime")
                {
                    e.Substitution = new DateTime(2018, 07, 08);
                    e.IsHandled = true;
                }
                else if (e.ColumnContext.ColumnDefinition.ColumnName == "Guid")
                {
                    e.Substitution = Guid.Empty;
                    e.IsHandled = true;
                }
            };
            Assert.IsTrue(csvReader.Read(), "Could not read the first record.");
            var values = csvReader.GetValues();
            var expected = new object[] { 1, new DateTime(2018, 07, 08), Guid.Empty };
            CollectionAssert.AreEqual(expected, values, "The wrong values were substituted.");
            Assert.IsFalse(csvReader.Read(), "Read too many records.");
        }
    }
}
