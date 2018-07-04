using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class FlatFileDataReaderTester
    {
        [TestMethod]
        public void ShouldGetDefaultSchemaForCSVFile()
        {
            FlatFileDataReader dataReader = GetFlatFileReaderWithDefaultSchema();
            var schema = dataReader.GetSchemaTable();

            var expectedNames = new[] { "Id", "Name", "CreatedOn", "IsActive", "VisitCount" };
            var actualNames = schema.Rows.Cast<DataRow>().Select(r => r.Field<string>(SchemaTableColumn.ColumnName)).ToArray();
            CollectionAssert.AreEqual(expectedNames, actualNames);

            var expectedPositions = Enumerable.Range(0, 5).ToArray();
            var actualPositions = schema.Rows.Cast<DataRow>().Select(r => r.Field<int>(SchemaTableColumn.ColumnOrdinal)).ToArray();
            CollectionAssert.AreEqual(expectedPositions, actualPositions);

            var expectedTypes = Enumerable.Repeat(typeof(string), 5).ToArray();
            var actualTypes = schema.Rows.Cast<DataRow>().Select(r => r.Field<Type>(SchemaTableColumn.DataType)).ToArray();
            CollectionAssert.AreEqual(expectedTypes, actualTypes);
        }

        private static FlatFileDataReader GetFlatFileReaderWithDefaultSchema()
        {
            const string data = @"Id,Name,CreatedOn,IsActive,VisitCount
1,Bob,2018-07-03,true,10
2,Susan,2018-07-04,false,
";
            var reader = new StringReader(data);
            var csvReader = new SeparatedValueReader(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            var dataReader = new FlatFileDataReader(csvReader);
            return dataReader;
        }

        [TestMethod]
        public void ShouldGetSpecifiedSchemaForCSVFile()
        {
            var dataReader = GetFlatFileReader();
            var schemaTable = dataReader.GetSchemaTable();

            var expectedNames = new[] { "Id", "Name", "CreatedOn", "IsActive", "VisitCount" };
            var actualNames = schemaTable.Rows.Cast<DataRow>().Select(r => r.Field<string>(SchemaTableColumn.ColumnName)).ToArray();
            CollectionAssert.AreEqual(expectedNames, actualNames);

            var expectedPositions = Enumerable.Range(0, 5).ToArray();
            var actualPositions = schemaTable.Rows.Cast<DataRow>().Select(r => r.Field<int>(SchemaTableColumn.ColumnOrdinal)).ToArray();
            CollectionAssert.AreEqual(expectedPositions, actualPositions);

            var expectedTypes = new[] { typeof(int), typeof(string), typeof(DateTime), typeof(bool), typeof(int) };
            var actualTypes = schemaTable.Rows.Cast<DataRow>().Select(r => r.Field<Type>(SchemaTableColumn.DataType)).ToArray();
            CollectionAssert.AreEqual(expectedTypes, actualTypes);
        }

        [TestMethod]
        public void ShouldGetRecordsFromReader()
        {
            FlatFileDataReader dataReader = GetFlatFileReader();
            Assert.IsTrue(dataReader.Read(), "The first record could not be read.");
            Assert.AreEqual(1, dataReader.GetInt32("Id"), "The wrong 'Id' was retrieved for 'Bob'.");
            Assert.AreEqual("Bob", dataReader.GetString("Name"), "The wrong 'Name' was retrieved for 'Bob'.");
            Assert.AreEqual(new DateTime(2018, 07, 03), dataReader.GetDateTime("CreatedOn"), "The wrong 'CreatedOn' was retrieved for 'Bob'.");
            Assert.IsTrue(dataReader.GetBoolean("IsActive"), "The wrong 'IsActive' was retrieved for 'Bob'");
            Assert.AreEqual(10, dataReader.GetNullableInt32("VisitCount"), "The wrong 'VisitCount' was retrieved for 'Bob'.");

            Assert.IsTrue(dataReader.Read(), "The second record could not be read.");
            Assert.AreEqual(2, dataReader.GetInt32("Id"), "The wrong 'Id' was retrieved for 'Susan'.");
            Assert.AreEqual("Susan", dataReader.GetString("Name"), "The wrong 'Name' was retrieved for 'Susan'.");
            Assert.AreEqual(new DateTime(2018, 07, 04), dataReader.GetDateTime("CreatedOn"), "The wrong 'CreatedOn' was retrieved for 'Susan'.");
            Assert.IsFalse(dataReader.GetBoolean("IsActive"), "The wrong 'IsActive' was retrieved for 'Susan'");
            Assert.AreEqual(null, dataReader.GetNullableInt32("VisitCount"), "The wrong 'VisitCount' was retrieved for 'Susan'.");

            Assert.IsFalse(dataReader.Read(), "Too many records were read.");
        }

        [TestMethod]
        public void ShouldGetRecordsFromReader_GetValue()
        {
            FlatFileDataReader dataReader = GetFlatFileReader();
            Assert.IsTrue(dataReader.Read(), "The first record could not be read.");
            Assert.AreEqual(1, dataReader.GetValue<int>("Id"), "The wrong 'Id' was retrieved for 'Bob'.");
            Assert.AreEqual("Bob", dataReader.GetValue<string>("Name"), "The wrong 'Name' was retrieved for 'Bob'.");
            Assert.AreEqual(new DateTime(2018, 07, 03), dataReader.GetValue<DateTime>("CreatedOn"), "The wrong 'CreatedOn' was retrieved for 'Bob'.");
            Assert.IsTrue(dataReader.GetValue<bool>("IsActive"), "The wrong 'IsActive' was retrieved for 'Bob'");
            Assert.AreEqual(10, dataReader.GetValue<int?>("VisitCount"), "The wrong 'VisitCount' was retrieved for 'Bob'.");

            Assert.IsTrue(dataReader.Read(), "The second record could not be read.");
            Assert.AreEqual(2, dataReader.GetValue<int>("Id"), "The wrong 'Id' was retrieved for 'Susan'.");
            Assert.AreEqual("Susan", dataReader.GetValue<string>("Name"), "The wrong 'Name' was retrieved for 'Susan'.");
            Assert.AreEqual(new DateTime(2018, 07, 04), dataReader.GetValue<DateTime>("CreatedOn"), "The wrong 'CreatedOn' was retrieved for 'Susan'.");
            Assert.IsFalse(dataReader.GetValue<bool>("IsActive"), "The wrong 'IsActive' was retrieved for 'Susan'");
            Assert.AreEqual(null, dataReader.GetValue<int?>("VisitCount"), "The wrong 'VisitCount' was retrieved for 'Susan'.");

            Assert.IsFalse(dataReader.Read(), "Too many records were read.");
        }

        private static FlatFileDataReader GetFlatFileReader()
        {
            const string data = @"1,Bob,2018-07-03,true,10
2,Susan,2018-07-04,false,
";
            var reader = new StringReader(data);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("Id"));
            schema.AddColumn(new StringColumn("Name"));
            schema.AddColumn(new DateTimeColumn("CreatedOn"));
            schema.AddColumn(new BooleanColumn("IsActive"));
            schema.AddColumn(new Int32Column("VisitCount"));
            var csvReader = new SeparatedValueReader(reader, schema);
            var dataReader = new FlatFileDataReader(csvReader);
            return dataReader;
        }
    }
}
