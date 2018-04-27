using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class SeparatedValueWriterMetadataTester
    {
        [Fact]
        public void TestWriter_WithSchema_SchemaNotCounted()
        {
            var outputMapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            outputMapper.Property(x => x.Name);
            outputMapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"));
            outputMapper.Property(x => x.CreatedOn).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person() { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person() { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person() { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            var inputMapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            inputMapper.Property(x => x.Name);
            inputMapper.Property(x => x.RecordNumber);
            inputMapper.Property(x => x.CreatedOn).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(1, results[0].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.Equal("Tom", results[1].Name);
            Assert.Equal(2, results[1].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.Equal("Jane", results[2].Name);
            Assert.Equal(3, results[2].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        [Fact]
        public void TestWriter_WithSchema_SchemaCounted()
        {
            var outputMapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            outputMapper.Property(x => x.Name);
            outputMapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = true
            });
            outputMapper.Property(x => x.CreatedOn).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person() { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person() { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person() { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            var inputMapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            inputMapper.Property(x => x.Name);
            inputMapper.Property(x => x.RecordNumber);
            inputMapper.Property(x => x.CreatedOn).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(2, results[0].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.Equal("Tom", results[1].Name);
            Assert.Equal(3, results[1].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.Equal("Jane", results[2].Name);
            Assert.Equal(4, results[2].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        [Fact]
        public void TestWriter_NoSchema_SchemaNotCounted()
        {
            var outputMapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            outputMapper.Property(x => x.Name);
            outputMapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = false
            });
            outputMapper.Property(x => x.CreatedOn).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person() { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person() { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person() { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            var inputMapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            inputMapper.Property(x => x.Name);
            inputMapper.Property(x => x.RecordNumber);
            inputMapper.Property(x => x.CreatedOn).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(1, results[0].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.Equal("Tom", results[1].Name);
            Assert.Equal(2, results[1].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.Equal("Jane", results[2].Name);
            Assert.Equal(3, results[2].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }
        
        [Fact]
        public void TestWriter_WithSchema_WithIgnoredColumn()
        {
            var outputMapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            outputMapper.Property(x => x.Name);
            outputMapper.Ignored();
            outputMapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = true
            });
            outputMapper.Ignored();
            outputMapper.Property(x => x.CreatedOn).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person() { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person() { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person() { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            string output = writer.ToString();

            var inputMapper = new SeparatedValueTypeMapper<Person>(() => new Person());
            inputMapper.Property(x => x.Name);
            inputMapper.Ignored();
            inputMapper.Property(x => x.RecordNumber);
            inputMapper.Ignored();
            inputMapper.Property(x => x.CreatedOn).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
            Assert.Equal(3, results.Length);
            Assert.Equal("Bob", results[0].Name);
            Assert.Equal(2, results[0].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.Equal("Tom", results[1].Name);
            Assert.Equal(3, results[1].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.Equal("Jane", results[2].Name);
            Assert.Equal(4, results[2].RecordNumber);
            Assert.Equal(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }
        
        public class Person
        {
            public string Name { get; set; }

            public int RecordNumber { get; set; }

            public DateTime CreatedOn { get; set; }
        }
    }
}
