using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class NonPublicAccessTester
    {
        [Fact]
        public void MapInternalClass()
        {
            var mapper = SeparatedValueTypeMapper.Define<InternalClass>();

            mapper.Property(x => x.Identifier);
            mapper.Property(x => x.Status);
            mapper.Property(x => x.EffectiveDate).InputFormat("yyyyMMdd");
            mapper.Property(x => x.ModificationDate).InputFormat("yyyyMMddHHmmss");
            mapper.Property(x => x.IsInternal);

            string rawData = @"ABC123,Doing Fine,20180115,20180115145100,true";
            StringReader reader = new StringReader(rawData);
            var data = mapper.Read(reader, new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                RecordSeparator = "\n",
                Separator = ",",
                Quote = '"'
            }).ToArray();

            Assert.Single(data);
            var result = data[0];
            Assert.Equal("ABC123", result.Identifier);
            Assert.Equal("Doing Fine", result.Status);
            Assert.Equal(new DateTime(2018, 1, 15), result.EffectiveDate);
            Assert.Equal(new DateTime(2018, 1, 15, 14, 51, 00), result.ModificationDate);
            Assert.True(result.IsInternal);
        }

        [Fact]
        public void MapInternalClass_Dynamic()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(InternalClass));

            mapper.StringProperty("Identifier");
            mapper.StringProperty("Status");
            mapper.DateTimeProperty("EffectiveDate").InputFormat("yyyyMMdd");
            mapper.DateTimeProperty("ModificationDate").InputFormat("yyyyMMddHHmmss");
            mapper.BooleanProperty("IsInternal");

            string rawData = @"ABC123,Doing Fine,20180115,20180115145100,true";
            StringReader reader = new StringReader(rawData);
            var data = mapper.Read(reader, new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                RecordSeparator = "\n",
                Separator = ",",
                Quote = '"'
            }).ToArray();

            Assert.Single(data);
            dynamic result = data[0];
            Assert.Equal("ABC123", result.Identifier);
            Assert.Equal("Doing Fine", result.Status);
            Assert.Equal(new DateTime(2018, 1, 15), result.EffectiveDate);
            Assert.Equal(new DateTime(2018, 1, 15, 14, 51, 00), result.ModificationDate);
            Assert.Equal(true, result.IsInternal);
        }

        [Fact]
        public void MapPrivateClass()
        {
            var mapper = SeparatedValueTypeMapper.Define<PrivateClass>();

            mapper.Property(x => x.Identifier);
            mapper.Property(x => x.Status);
            mapper.Property(x => x.EffectiveDate).InputFormat("yyyyMMdd");
            mapper.Property(x => x.ModificationDate).InputFormat("yyyyMMddHHmmss");

            string rawData = @"ABC123,Doing Fine,20180115,20180115145100";
            StringReader reader = new StringReader(rawData);
            var data = mapper.Read(reader, new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                RecordSeparator = "\n",
                Separator = ",",
                Quote = '"'
            }).ToArray();

            Assert.Single(data);
            var result = data[0];
            Assert.Equal("ABC123", result.Identifier);
            Assert.Equal("Doing Fine", result.Status);
            Assert.Equal(new DateTime(2018, 1, 15), result.EffectiveDate);
            Assert.Equal(new DateTime(2018, 1, 15, 14, 51, 00), result.ModificationDate);
        }

        [Fact]
        public void MapPrivateClass_Dynamic()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(PrivateClass));

            mapper.StringProperty("Identifier");
            mapper.StringProperty("Status");
            mapper.DateTimeProperty("EffectiveDate").InputFormat("yyyyMMdd");
            mapper.DateTimeProperty("ModificationDate").InputFormat("yyyyMMddHHmmss");

            string rawData = @"ABC123,Doing Fine,20180115,20180115145100";
            StringReader reader = new StringReader(rawData);
            var data = mapper.Read(reader, new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                RecordSeparator = "\n",
                Separator = ",",
                Quote = '"'
            }).ToArray();

            Assert.Single(data);
            dynamic result = data[0];
            Assert.Equal("ABC123", result.Identifier);
            Assert.Equal("Doing Fine", result.Status);
            Assert.Equal(new DateTime(2018, 1, 15), result.EffectiveDate);
            Assert.Equal(new DateTime(2018, 1, 15, 14, 51, 00), result.ModificationDate);
        }

        [Fact]
        public void MapPrivateClass_PrivateCtor_Dynamic()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(PrivateCtorClass));

            mapper.Int32Property("Id");

            string rawData = @"123";
            StringReader reader = new StringReader(rawData);
            var data = mapper.Read(reader, new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false,
                RecordSeparator = "\n",
                Separator = ",",
                Quote = '"'
            }).ToArray();

            Assert.Single(data);
            dynamic result = data[0];
            Assert.Equal(123, result.Id);
        }

        internal class InternalClass
        {
            internal string Identifier { get; set; }
            internal string Status { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public DateTime? ModificationDate { get; set; }
            internal bool IsInternal = false;

            public void SetInternal()
            {
                IsInternal = true;
            }
        }

        private class PrivateClass
        {
            public string Identifier { get; set; }
            public string Status { get; set; }
            public DateTime? EffectiveDate { get; set; }
            public DateTime? ModificationDate { get; set; }
        }

        private class PrivateCtorClass
        {
            private PrivateCtorClass() { }
            public int Id { get; set; }
        }
    }
}
