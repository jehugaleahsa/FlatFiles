using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class SeparatedValueReaderMetadataTester
    {
        [Fact]
        public void TestReader_WithSchema_NoFilter_LogicalRecordsOnly()
        {
            var mapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"));
            StringReader reader = new StringReader(output);
            var results = mapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(1, results[0].RecordNumber);
            Assert.Equal("Tom", results[1].Name);
            Assert.Equal(2, results[1].RecordNumber);
            Assert.Equal("Jane", results[2].Name);
            Assert.Equal(3, results[2].RecordNumber);
        }

        [Fact]
        public void TestReader_WithSchema_WithFilter_LogicalRecordsOnly()
        {
            var mapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"));
            StringReader stringReader = new StringReader(output);
            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordRead += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length == 1 && e.Values[0] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.Equal(2, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(1, results[0].RecordNumber);
            Assert.Equal("Jane", results[1].Name);
            Assert.Equal(2, results[1].RecordNumber);
        }

        [Fact]
        public void TestReader_WithSchema_WithFilter_LineCount()
        {
            var mapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = true,
                IncludeFilteredRecords = true
            });
            StringReader stringReader = new StringReader(output);
            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordRead += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length == 1 && e.Values[0] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.Equal(2, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(2, results[0].RecordNumber);
            Assert.Equal("Jane", results[1].Name);
            Assert.Equal(4, results[1].RecordNumber);
        }

        [Fact]
        public void TestReader_WithSchema_NoFilter_LineCount()
        {
            var mapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = true,
                IncludeFilteredRecords = true
            });
            StringReader reader = new StringReader(output);
            var results = mapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(2, results[0].RecordNumber);
            Assert.Equal("Tom", results[1].Name);
            Assert.Equal(3, results[1].RecordNumber);
            Assert.Equal("Jane", results[2].Name);
            Assert.Equal(4, results[2].RecordNumber);
        }

        [Fact]
        public void TestReader_WithSchema_SchemaNotCounted_WithFilter_LineCount()
        {
            var mapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = false,
                IncludeFilteredRecords = true
            });
            StringReader stringReader = new StringReader(output);
            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordRead += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length == 1 && e.Values[0] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.Equal(2, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(1, results[0].RecordNumber);
            Assert.Equal("Jane", results[1].Name);
            Assert.Equal(3, results[1].RecordNumber);
        }

        [Fact]
        public void TestReader_WithSchema_SchemaNotCounted_NoFilter_LineCountMinusOne()
        {
            var mapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = false,
                IncludeFilteredRecords = true
            });
            StringReader reader = new StringReader(output);
            var results = mapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(1, results[0].RecordNumber);
            Assert.Equal("Tom", results[1].Name);
            Assert.Equal(2, results[1].RecordNumber);
            Assert.Equal("Jane", results[2].Name);
            Assert.Equal(3, results[2].RecordNumber);
        }

        [Fact]
        public void TestReader_WithSchema_NoFilter_WithIgnoredColumn_LogicalRecordsOnly()
        {
            var mapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name);
            mapper.Ignored();

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"));
            StringReader reader = new StringReader(output);
            var results = mapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(1, results[0].RecordNumber);
            Assert.Equal("Tom", results[1].Name);
            Assert.Equal(2, results[1].RecordNumber);
            Assert.Equal("Jane", results[2].Name);
            Assert.Equal(3, results[2].RecordNumber);
        }

        [Fact]
        public void TestReader_WithSchema_WithIgnoredColumn_WithFilter_LogicalRecordsOnly()
        {
            var mapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name);
            mapper.Ignored();

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"));
            StringReader stringReader = new StringReader(output);
            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordRead += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length >= 1 && e.Values[0] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.Equal(2, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(1, results[0].RecordNumber);
            Assert.Equal("Jane", results[1].Name);
            Assert.Equal(2, results[1].RecordNumber);
        }

        public class Person
        {
            public string Name { get; set; }

            public int RecordNumber { get; set; }
        }
    }
}
