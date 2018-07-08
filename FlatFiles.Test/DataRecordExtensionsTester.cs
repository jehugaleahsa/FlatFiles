using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class DataRecordExtensionsTester
    {
        [TestMethod]
        public void TestNullableExtensions_AllNull()
        {
            string data = String.Join(",", typeof(NullableValues).GetProperties().Select(x => (string)null));
            var schema = GetSchema();
            var stringReader = new StringReader(data);
            var csvReader = new SeparatedValueReader(stringReader, schema);
            var dataReader = new FlatFileDataReader(csvReader);
            Assert.IsTrue(dataReader.Read(), "A record was not read.");
            Assert.IsNull(dataReader.GetNullableByte(0), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableInt16(1), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableInt32(2), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableInt64(3), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableFloat(4), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableDouble(5), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableDecimal(6), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableString(7), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableDateTime(8), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableGuid(9), "The byte was not null.");
            Assert.IsNull(dataReader.GetNullableEnum<DayOfWeek>(10), "The byte was not null.");
            Assert.IsFalse(dataReader.Read(), "Too many records were read.");
        }

        [TestMethod]
        public void TestNullableExtensions_AllNotNull()
        {
            string data = String.Join(",", new object[] 
            {
                (byte)0,  // Byte
                (short)1,  // Short
                2,  // Int
                3L,  // Long
                4f,  // Float
                5.0,  // Double
                6m,  // Decimal
                "abc",  // String
                new DateTime(2018, 07, 08),  // DateTime
                new Guid("{2E13CDEB-6A06-4A79-A446-B057F2881406}"),  // Guid
                DayOfWeek.Sunday  // Enum
            });
            var schema = GetSchema();
            var stringReader = new StringReader(data);
            var csvReader = new SeparatedValueReader(stringReader, schema);
            var dataReader = new FlatFileDataReader(csvReader);
            Assert.IsTrue(dataReader.Read(), "A record was not read.");
            Assert.AreEqual((byte)0, dataReader.GetNullableByte(0), "The byte was not null.");
            Assert.AreEqual((short)1, dataReader.GetNullableInt16(1), "The byte was not null.");
            Assert.AreEqual(2, dataReader.GetNullableInt32(2), "The byte was not null.");
            Assert.AreEqual(3L, dataReader.GetNullableInt64(3), "The byte was not null.");
            Assert.AreEqual(4f, dataReader.GetNullableFloat(4), "The byte was not null.");
            Assert.AreEqual(5.0, dataReader.GetNullableDouble(5), "The byte was not null.");
            Assert.AreEqual(6m, dataReader.GetNullableDecimal(6), "The byte was not null.");
            Assert.AreEqual("abc", dataReader.GetNullableString(7), "The byte was not null.");
            Assert.AreEqual(new DateTime(2018, 07, 08), dataReader.GetNullableDateTime(8), "The byte was not null.");
            Assert.AreEqual(new Guid("{2E13CDEB-6A06-4A79-A446-B057F2881406}"), dataReader.GetNullableGuid(9), "The byte was not null.");
            Assert.AreEqual(DayOfWeek.Sunday, dataReader.GetNullableEnum<DayOfWeek>(10), "The byte was not null.");
            Assert.IsFalse(dataReader.Read(), "Too many records were read.");
        }

        private static SeparatedValueSchema GetSchema()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new NullableValues());
            mapper.Property(x => x.ByteValue);
            mapper.Property(x => x.ShortValue);
            mapper.Property(x => x.IntValue);
            mapper.Property(x => x.LongValue);
            mapper.Property(x => x.FloatValue);
            mapper.Property(x => x.DoubleValue);
            mapper.Property(x => x.DecimalValue);
            mapper.Property(x => x.StringValue);
            mapper.Property(x => x.DateTimeValue);
            mapper.Property(x => x.GuidValue);
            mapper.EnumProperty(x => x.EnumValue);

            var schema = mapper.GetSchema();
            return schema;
        }

        public class NullableValues
        {
            public byte? ByteValue { get; set; }

            public short? ShortValue { get; set; }

            public int? IntValue { get; set; }

            public long? LongValue { get; set; }

            public float? FloatValue { get; set; }

            public double? DoubleValue { get; set; }

            public decimal? DecimalValue { get; set; }

            public string StringValue { get; set; }

            public DateTime? DateTimeValue { get; set; }

            public Guid? GuidValue { get; set; }

            public DayOfWeek? EnumValue { get; set; }
        }
    }
}
