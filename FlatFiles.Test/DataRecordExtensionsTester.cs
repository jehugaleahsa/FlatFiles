﻿using System;
using System.Data;
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
            string data = string.Join(",", typeof(NullableValues).GetProperties().Select(x => (string)null));
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
            string data = string.Join(",", (byte)0, (short)1, 2, 3L, 4f, 5.0, 6m, "abc", new DateTime(2018, 07, 08), new Guid("{2E13CDEB-6A06-4A79-A446-B057F2881406}"), DayOfWeek.Sunday);
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

        [TestMethod]
        public void TestGetValues_DBNullToNull()
        {
            var record = new FakeDataRecord(new object[]
            {
                0, DateTime.UnixEpoch, DBNull.Value, 3.14159, 3.14159m, "A String"
            });
            var values = record.GetValues(replaceDBNulls: true);
            Assert.AreEqual(6, values.Length);
            Assert.AreEqual(null, values[2]);
        }

        [TestMethod]
        public void TestGetValues_LargerArray()
        {
            var record = new FakeDataRecord(new object[]
            {
                0, DateTime.UnixEpoch, DBNull.Value, 3.14159, 3.14159m, "A String"
            });
            var values = new object[10];
            int length = record.GetValues(values);
            Assert.AreEqual(6, length);
            var expected = new object[]
            {
                0, DateTime.UnixEpoch, DBNull.Value, 3.14159, 3.14159m, "A String", null, null, null, null
            };
            CollectionAssert.AreEqual(expected, values);
        }

        [TestMethod]
        public void TestGetValues_LargerArray_DBNullToNull()
        {
            var record = new FakeDataRecord(new object[]
            {
                0, DateTime.UnixEpoch, DBNull.Value, 3.14159, 3.14159m, "A String"
            });
            var values = new object[10];
            int length = record.GetValues(values, replaceDBNulls: true);
            Assert.AreEqual(6, length);
            var expected = new object[]
            {
                0, DateTime.UnixEpoch, null, 3.14159, 3.14159m, "A String", null, null, null, null
            };
            CollectionAssert.AreEqual(expected, values);
        }

        private class FakeDataRecord : IDataRecord
        {
            public FakeDataRecord(object[] values)
            {
                Values = values;
            }

            public object[] Values { get; }

            public bool GetBoolean(int i)
            {
                throw new NotImplementedException();
            }

            public byte GetByte(int i)
            {
                throw new NotImplementedException();
            }

            public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public char GetChar(int i)
            {
                throw new NotImplementedException();
            }

            public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
            {
                throw new NotImplementedException();
            }

            public IDataReader GetData(int i)
            {
                throw new NotImplementedException();
            }

            public string GetDataTypeName(int i)
            {
                throw new NotImplementedException();
            }

            public DateTime GetDateTime(int i)
            {
                throw new NotImplementedException();
            }

            public decimal GetDecimal(int i)
            {
                throw new NotImplementedException();
            }

            public double GetDouble(int i)
            {
                throw new NotImplementedException();
            }

            public Type GetFieldType(int i)
            {
                throw new NotImplementedException();
            }

            public float GetFloat(int i)
            {
                throw new NotImplementedException();
            }

            public Guid GetGuid(int i)
            {
                throw new NotImplementedException();
            }

            public short GetInt16(int i)
            {
                throw new NotImplementedException();
            }

            public int GetInt32(int i)
            {
                throw new NotImplementedException();
            }

            public long GetInt64(int i)
            {
                throw new NotImplementedException();
            }

            public string GetName(int i)
            {
                throw new NotImplementedException();
            }

            public int GetOrdinal(string name)
            {
                throw new NotImplementedException();
            }

            public string GetString(int i)
            {
                throw new NotImplementedException();
            }

            public object GetValue(int i)
            {
                throw new NotImplementedException();
            }

            public int GetValues(object[] values)
            {
                int length = Math.Min(values.Length, Values.Length);
                Array.Copy(Values, values, length);
                return length;
            }

            public bool IsDBNull(int i) => Values[i] == null || Values[i] == DBNull.Value;

            public int FieldCount => Values.Length;

            public object this[int i]
            {
                get { throw new NotImplementedException(); }
            }

            public object this[string name]
            {
                get { throw new NotImplementedException(); }
            }
        }
    }
}
