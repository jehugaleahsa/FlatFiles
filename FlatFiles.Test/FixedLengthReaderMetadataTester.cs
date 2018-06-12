using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class FixedLengthReaderMetadataTester
    {
        [TestMethod]
        public void TestReader_WithSchema_NoFilter_LogicalRecordsOnly()
        {
            var mapper = new FixedLengthTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name, 10);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions() { IsFirstRecordHeader = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"), 10);
            StringReader reader = new StringReader(output);
            var results = mapper.Read(reader, new FixedLengthOptions() { IsFirstRecordHeader = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(3, results[2].RecordNumber);
        }

        [TestMethod]
        public void TestReader_WithSchema_WithFilter_LogicalRecordsOnly()
        {
            var mapper = new FixedLengthTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name, 10);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions() { IsFirstRecordHeader = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"), 10);
            StringReader stringReader = new StringReader(output);
            var options = new FixedLengthOptions() { IsFirstRecordHeader = true };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordPartitioned += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length == 1 && e.Values[0] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual("Jane", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
        }

        [TestMethod]
        public void TestReader_WithSchema_WithFilter_LineCount()
        {
            var mapper = new FixedLengthTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name, 10);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions() { IsFirstRecordHeader = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = true,
                IncludeFilteredRecords = true
            }, 10);
            StringReader stringReader = new StringReader(output);
            var options = new FixedLengthOptions() { IsFirstRecordHeader = true };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordPartitioned += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length == 1 && e.Values[0] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(2, results[0].RecordNumber);
            Assert.AreEqual("Jane", results[1].Name);
            Assert.AreEqual(4, results[1].RecordNumber);
        }

        [TestMethod]
        public void TestReader_WithSchema_NoFilter_LineCount()
        {
            var mapper = new FixedLengthTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name, 10);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions() { IsFirstRecordHeader = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = true,
                IncludeFilteredRecords = true
            }, 10);
            StringReader reader = new StringReader(output);
            var results = mapper.Read(reader, new FixedLengthOptions() { IsFirstRecordHeader = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(2, results[0].RecordNumber);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(3, results[1].RecordNumber);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(4, results[2].RecordNumber);
        }

        [TestMethod]
        public void TestReader_WithSchema_SchemaNotCounted_WithFilter_LineCount()
        {
            var mapper = new FixedLengthTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name, 10);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions() { IsFirstRecordHeader = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = false,
                IncludeFilteredRecords = true
            }, 10);
            StringReader stringReader = new StringReader(output);
            var options = new FixedLengthOptions() { IsFirstRecordHeader = true };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordPartitioned += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length == 1 && e.Values[0] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual("Jane", results[1].Name);
            Assert.AreEqual(3, results[1].RecordNumber);
        }

        [TestMethod]
        public void TestReader_WithSchema_SchemaNotCounted_NoFilter_LineCountMinusOne()
        {
            var mapper = new FixedLengthTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name, 10);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions() { IsFirstRecordHeader = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber")
            {
                IncludeSchema = false,
                IncludeFilteredRecords = true
            }, 10);
            StringReader reader = new StringReader(output);
            var results = mapper.Read(reader, new FixedLengthOptions() { IsFirstRecordHeader = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(3, results[2].RecordNumber);
        }

        [TestMethod]
        public void TestReader_WithSchema_NoFilter_WithIgnoredColumn_LogicalRecordsOnly()
        {
            var mapper = new FixedLengthTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name, 10);
            mapper.Ignored(1);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions() { IsFirstRecordHeader = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"), 10);
            StringReader reader = new StringReader(output);
            var results = mapper.Read(reader, new FixedLengthOptions() { IsFirstRecordHeader = true }).ToArray();
            Assert.AreEqual(3, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual("Tom", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
            Assert.AreEqual("Jane", results[2].Name);
            Assert.AreEqual(3, results[2].RecordNumber);
        }

        [TestMethod]
        public void TestReader_WithSchema_WithIgnoredColumn_WithFilter_LogicalRecordsOnly()
        {
            var mapper = new FixedLengthTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name, 10);
            mapper.Ignored(1);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions() { IsFirstRecordHeader = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"), 10);
            StringReader stringReader = new StringReader(output);
            var options = new FixedLengthOptions() { IsFirstRecordHeader = true };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordPartitioned += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length >= 1 && e.Values[0] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual("Jane", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
        }


        [TestMethod]
        public void TestReader_WithSchema_WithIgnoredColumn_NoRecordSeparator_WithFilter_LogicalRecordsOnly()
        {
            var mapper = new FixedLengthTypeMapper<Person>(() => new Person());
            mapper.Property(x => x.Name, 10);
            mapper.Ignored(1);

            var people = new[]
            {
                new Person() { Name = "Bob" },
                new Person() { Name = "Tom" },
                new Person() { Name = "Jane" }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions()
            {
                IsFirstRecordHeader = true,
                HasRecordSeparator = false
            });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"), 10);
            StringReader stringReader = new StringReader(output);
            var options = new FixedLengthOptions()
            {
                IsFirstRecordHeader = true,
                HasRecordSeparator = false
            };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordPartitioned += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length >= 1 && e.Values[0] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual("Jane", results[1].Name);
            Assert.AreEqual(2, results[1].RecordNumber);
        }


        [TestMethod]
        public void TestReader_WrappedWithIgnoredColumns()
        {
            var mapper = new FixedLengthTypeMapper<ComplicatedPerson>(() => new ComplicatedPerson());
            mapper.Property(x => x.PersonId, 10);
            mapper.Ignored(1);
            mapper.Property(x => x.Name, 10);
            mapper.Ignored(1);
            mapper.Property(x => x.CreatedOn, 10).OutputFormat("MM/dd/yyyy");

            var people = new[]
            {
                new ComplicatedPerson() { PersonId = 1, Name = "Bob", CreatedOn = new DateTime(2018, 04, 25) },
                new ComplicatedPerson() { PersonId = 2, Name = "Tom", CreatedOn = new DateTime(2018, 04, 26) },
                new ComplicatedPerson() { PersonId = 3, Name = "Jane", CreatedOn = new DateTime(2018, 04, 27) }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people, new FixedLengthOptions() { IsFirstRecordHeader = true });
            string output = writer.ToString();

            mapper.CustomProperty(x => x.RecordNumber, new RecordNumberColumn("RecordNumber"), 10);
            StringReader stringReader = new StringReader(output);
            var options = new FixedLengthOptions() { IsFirstRecordHeader = true };
            var reader = mapper.GetReader(stringReader, options);
            reader.RecordPartitioned += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length >= 2 && e.Values[2] == "Tom";
            };
            var results = reader.ReadAll().ToArray();
            Assert.AreEqual(2, results.Length);
            Assert.AreEqual(1, results[0].PersonId);
            Assert.AreEqual("Bob", results[0].Name);
            Assert.AreEqual(new DateTime(2018, 04, 25), results[0].CreatedOn);
            Assert.AreEqual(1, results[0].RecordNumber);
            Assert.AreEqual(3, results[1].PersonId);
            Assert.AreEqual("Jane", results[1].Name);
            Assert.AreEqual(new DateTime(2018, 04, 27), results[1].CreatedOn);
            Assert.AreEqual(2, results[1].RecordNumber);
        }

        public class Person
        {
            public string Name { get; set; }

            public int RecordNumber { get; set; }
        }

        public class ComplicatedPerson : Person
        {
            public int PersonId { get; set; }

            public DateTime CreatedOn { get; set; }
        }
    }
}
