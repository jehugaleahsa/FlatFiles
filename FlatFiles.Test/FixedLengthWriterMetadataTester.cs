using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class FixedLengthWriterMetadataTester
    {
        [TestMethod]
        public void TestWriter_WithSchema_SchemaNotCounted()
        {
            var outputMapper = FixedLengthTypeMapper.Define(() => new Person());
            outputMapper.Property(x => x.Name, 10);
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber"), 10)
                .WithReader((p, v) => p.RecordNumber = (int)v)
                .WithWriter(p => p.RecordNumber);
            outputMapper.Property(x => x.CreatedOn, 10).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new FixedLengthOptions { IsFirstRecordHeader = true });
            string output = writer.ToString();

            var inputMapper = FixedLengthTypeMapper.Define(() => new Person());
            inputMapper.Property(x => x.Name, 10);
            inputMapper.Property(x => x.RecordNumber, 10);
            inputMapper.Property(x => x.CreatedOn, 10).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new FixedLengthOptions { IsFirstRecordHeader = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(3, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        [TestMethod]
        public void TestWriter_WithSchema_SchemaCounted()
        {
            var outputMapper = FixedLengthTypeMapper.Define(() => new Person());
            outputMapper.Property(x => x.Name, 10);
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = true }, 10)
                .WithReader((p, v) => p.RecordNumber = (int)v)
                .WithWriter(p => p.RecordNumber);
            outputMapper.Property(x => x.CreatedOn, 10).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new FixedLengthOptions { IsFirstRecordHeader = true });
            string output = writer.ToString();

            var inputMapper = FixedLengthTypeMapper.Define(() => new Person());
            inputMapper.Property(x => x.Name, 10);
            inputMapper.Property(x => x.RecordNumber, 10);
            inputMapper.Property(x => x.CreatedOn, 10).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new FixedLengthOptions { IsFirstRecordHeader = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(2, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(3, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(4, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        [TestMethod]
        public void TestWriter_NoSchema_SchemaNotCounted()
        {
            var outputMapper = FixedLengthTypeMapper.Define(() => new Person());
            outputMapper.Property(x => x.Name, 10);
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = false }, 10)
                .WithReader((p, v) => p.RecordNumber = (int)v)
                .WithWriter(p => p.RecordNumber);
            outputMapper.Property(x => x.CreatedOn, 10).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new FixedLengthOptions { IsFirstRecordHeader = true });
            string output = writer.ToString();

            var inputMapper = FixedLengthTypeMapper.Define(() => new Person());
            inputMapper.Property(x => x.Name, 10);
            inputMapper.Property(x => x.RecordNumber, 10);
            inputMapper.Property(x => x.CreatedOn, 10).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new FixedLengthOptions { IsFirstRecordHeader = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(3, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }
        
        [TestMethod]
        public void TestWriter_WithSchema_WithIgnoredColumns()
        {
            var outputMapper = FixedLengthTypeMapper.Define(() => new Person());
            outputMapper.Property(x => x.Name, 10);
            outputMapper.Ignored(1);
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = true }, 10)
                .WithReader((p, v) => p.RecordNumber = (int)v)
                .WithWriter(p => p.RecordNumber);
            outputMapper.Ignored(1);
            outputMapper.Property(x => x.CreatedOn, 10).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new FixedLengthOptions { IsFirstRecordHeader = true });
            string output = writer.ToString();

            var inputMapper = FixedLengthTypeMapper.Define(() => new Person());
            inputMapper.Property(x => x.Name, 10);
            inputMapper.Ignored(1);
            inputMapper.Property(x => x.RecordNumber, 10);
            inputMapper.Ignored(1);
            inputMapper.Property(x => x.CreatedOn, 10).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new FixedLengthOptions { IsFirstRecordHeader = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(2, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(3, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(4, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        [TestMethod]
        public void TestWriter_NoRecordSeparator_ValidRecordCounts()
        {
            var outputMapper = FixedLengthTypeMapper.Define(() => new Person());
            outputMapper.Property(x => x.Name, 10);
            outputMapper.Ignored(1);
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = true }, 10)
                .WithReader((p, v) => p.RecordNumber = (int)v)
                .WithWriter(p => p.RecordNumber);
            outputMapper.Ignored(1);
            outputMapper.Property(x => x.CreatedOn, 10).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new FixedLengthOptions
                                               {
                IsFirstRecordHeader = true,
                HasRecordSeparator = false
            });
            string output = writer.ToString();

            var inputMapper = FixedLengthTypeMapper.Define(() => new Person());
            inputMapper.Property(x => x.Name, 10);
            inputMapper.Ignored(1);
            inputMapper.Property(x => x.RecordNumber, 10);
            inputMapper.Ignored(1);
            inputMapper.Property(x => x.CreatedOn, 10).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new FixedLengthOptions
                                                   {
                IsFirstRecordHeader = true,
                HasRecordSeparator = false
            }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(2, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(3, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(4, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        [TestMethod]
        public void TestWriter_WriteOnlyColumn_WithIgnoredColumn()
        {
            var outputMapper = FixedLengthTypeMapper.Define(() => new Person());
            outputMapper.Property(x => x.Name, 10);
            outputMapper.Ignored(1);
            outputMapper.CustomMapping(new RecordNumberColumn("RecordNumber") { IncludeSchema = true }, 10).WithWriter((ctx, p) => ctx.RecordContext.PhysicalRecordNumber);
            outputMapper.Ignored(1);
            outputMapper.Property(x => x.CreatedOn, 10).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new Person { Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new Person { Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new Person { Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            outputMapper.Write(writer, people, new FixedLengthOptions { IsFirstRecordHeader = true });
            string output = writer.ToString();

            var inputMapper = FixedLengthTypeMapper.Define(() => new Person());
            inputMapper.Property(x => x.Name, 10);
            inputMapper.Ignored(1);
            inputMapper.Property(x => x.RecordNumber, 10);
            inputMapper.Ignored(1);
            inputMapper.Property(x => x.CreatedOn, 10).InputFormat("MM/dd/yyyy");

            StringReader reader = new StringReader(output);
            var results = inputMapper.Read(reader, new FixedLengthOptions { IsFirstRecordHeader = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(2, results[0].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(3, results[1].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 26), results[1].CreatedOn);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(4, results[2].RecordNumber);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[2].CreatedOn);
        }

        public class Person
        {
            public string Name { get; set; }

            public int RecordNumber { get; set; }

            public DateTime CreatedOn { get; set; }
        }
    }
}
