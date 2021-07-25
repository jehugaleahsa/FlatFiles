using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class AutoMapTester
    {
        [TestMethod]
        public void ShouldDeduceSchemaForType()
        {
            var stringWriter = new StringWriter();
            var writer = SeparatedValueTypeMapper.GetAutoMappedWriter<Person>(stringWriter);
            var expected = new[]
            {
                new Person() { Id = 1, Name = "Bob", CreatedOn = new DateTime(2018, 07, 01), IsActive = true, VisitCount = 1 },
                new Person() { Id = 2, Name = "John", CreatedOn = new DateTime(2018, 07, 02), IsActive = false, VisitCount = null },
                new Person() { Id = 3, Name = "Susan", CreatedOn = new DateTime(2018, 07, 03), IsActive = false, VisitCount = 10 }
            };
            writer.WriteAll(expected);
            string output = stringWriter.ToString();

            var stringReader = new StringReader(output);
            var reader = SeparatedValueTypeMapper.GetAutoMappedReader<Person>(stringReader);
            var results = reader.ReadAll().ToArray();

            Assert.AreEqual(3, results.Length, "The wrong number of records were read.");
            AssertEqual(expected, results, 0);
            AssertEqual(expected, results, 1);
            AssertEqual(expected, results, 2);
        }

        [TestMethod]
        public async Task ShouldDeduceSchemaForTypeAsync()
        {
            var stringWriter = new StringWriter();
            var writer = SeparatedValueTypeMapper.GetAutoMappedWriter<Person>(stringWriter);
            var expected = new[]
            {
                new Person() { Id = 1, Name = "Bob", CreatedOn = new DateTime(2018, 07, 01), IsActive = true, VisitCount = 1 },
                new Person() { Id = 2, Name = "John", CreatedOn = new DateTime(2018, 07, 02), IsActive = false, VisitCount = null },
                new Person() { Id = 3, Name = "Susan", CreatedOn = new DateTime(2018, 07, 03), IsActive = false, VisitCount = 10 }
            };
            await writer.WriteAllAsync(expected);
            string output = stringWriter.ToString();

            var stringReader = new StringReader(output);
            var reader = await SeparatedValueTypeMapper.GetAutoMappedReaderAsync<Person>(stringReader);
            var results = new List<Person>();
            while (await reader.ReadAsync())
            {
                results.Add(reader.Current);
            }

            Assert.AreEqual(3, results.Count, "The wrong number of records were read.");
            AssertEqual(expected, results, 0);
            AssertEqual(expected, results, 1);
            AssertEqual(expected, results, 2);
        }

        [TestMethod]
        public void ShouldDeduceSchemaForType_ColumnNameCustomization()
        {
            var stringWriter = new StringWriter();
            var nameResolver = AutoMapResolver.For(m => $"Prefix_{m.Name}_Postfix");
            var writer = SeparatedValueTypeMapper.GetAutoMappedWriter<Person>(stringWriter, null, nameResolver);
            var expected = new[]
            {
                new Person() { Id = 1, Name = "Bob", CreatedOn = new DateTime(2018, 07, 01), IsActive = true, VisitCount = 1 },
                new Person() { Id = 2, Name = "John", CreatedOn = new DateTime(2018, 07, 02), IsActive = false, VisitCount = null },
                new Person() { Id = 3, Name = "Susan", CreatedOn = new DateTime(2018, 07, 03), IsActive = false, VisitCount = 10 }
            };
            writer.WriteAll(expected);
            string output = stringWriter.ToString();

            var stringReader = new StringReader(output);
            var reader = SeparatedValueTypeMapper.GetAutoMappedReader<Person>(stringReader, null, AutoMapMatcher.For(nameResolver));
            var results = reader.ReadAll().ToArray();

            Assert.AreEqual(3, results.Length, "The wrong number of records were read.");
            AssertEqual(expected, results, 0);
            AssertEqual(expected, results, 1);
            AssertEqual(expected, results, 2);
        }

        [TestMethod]
        public void ShouldWriteHeadersWhenNoRecordsProvided_Writer()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new Person());
            mapper.Property(x => x.Id);
            mapper.Property(x => x.Name);
            mapper.Property(x => x.CreatedOn);
            mapper.Property(x => x.IsActive);
            var stringWriter = new StringWriter();
            var options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = true
            };
            var writer = mapper.GetWriter(stringWriter, options);
            writer.WriteAll(new Person[0]);
            writer.WriteAll(new Person[0]); // Test we don't double write headers
            var output = stringWriter.ToString();

            var stringReader = new StringReader(output);
            var reader = new SeparatedValueReader(stringReader, options);
            Assert.IsFalse(reader.Read(), "No records should have been written.");

            var schema = reader.GetSchema();
            Assert.AreEqual(4, schema.ColumnDefinitions.Count, "The wrong number of headers were found.");
            var expected = new[] { "Id", "Name", "CreatedOn", "IsActive" };
            var actual = schema.ColumnDefinitions.Select(c => c.ColumnName).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldWriteHeadersWhenNoRecordsProvided_Mapper()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new Person());
            mapper.Property(x => x.Id);
            mapper.Property(x => x.Name);
            mapper.Property(x => x.CreatedOn);
            mapper.Property(x => x.IsActive);
            var stringWriter = new StringWriter();
            var options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = true
            };
            mapper.Write(stringWriter, new Person[0], options);
            var output = stringWriter.ToString();

            var stringReader = new StringReader(output);
            var reader = new SeparatedValueReader(stringReader, options);
            Assert.IsFalse(reader.Read(), "No records should have been written.");

            var schema = reader.GetSchema();
            Assert.AreEqual(4, schema.ColumnDefinitions.Count, "The wrong number of headers were found.");
            var expected = new[] { "Id", "Name", "CreatedOn", "IsActive" };
            var actual = schema.ColumnDefinitions.Select(c => c.ColumnName).ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ShouldNotWriteHeaderWhenHeaderNotConfigured()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new Person());
            mapper.Property(x => x.Id);
            mapper.Property(x => x.Name);
            mapper.Property(x => x.CreatedOn);
            mapper.Property(x => x.IsActive);
            var stringWriter = new StringWriter();
            var options = new SeparatedValueOptions()
            {
                IsFirstRecordSchema = false
            };
            var people = new Person[]
            {
                new Person()
                {
                    Id = 1,
                    Name = "Tom",
                    CreatedOn = new DateTime(2021, 05, 10),
                    IsActive = true,
                    VisitCount = 100
                }
            };
            mapper.Write(stringWriter, people, options);
            var output = stringWriter.ToString();

            var stringReader = new StringReader(output);
            var reader = new SeparatedValueReader(stringReader, mapper.GetSchema(), options);
            Assert.IsTrue(reader.Read(), "The first record should have been written.");
            Assert.IsFalse(reader.Read(), "Too many records were written.");
        }

        [TestMethod]
        public void ShouldReturnEmptySchemaWhenFileEmpty()
        {
            var stringReader = new StringReader(String.Empty);
            var reader = SeparatedValueTypeMapper.GetAutoMappedReader<Person>(stringReader);
            var results = reader.ReadAll().ToArray();

            Assert.AreEqual(0, results.Length, "The wrong number of records were read.");
        }

        private static void AssertEqual(IList<Person> expected, IList<Person> actual, int id)
        {
            Assert.AreEqual(expected[id].Id, actual[id].Id, $"Wrong ID for person {id}");
            Assert.AreEqual(expected[id].Name, actual[id].Name, $"Wrong Name for person {id}");
            Assert.AreEqual(expected[id].CreatedOn, actual[id].CreatedOn, $"Wrong CreatedOn for person {id}");
            Assert.AreEqual(expected[id].IsActive, actual[id].IsActive, $"Wrong IsActive for person {id}");
            Assert.AreEqual(expected[id].VisitCount, actual[id].VisitCount, $"Wrong VisitCount for person {id}");
        }

        internal class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime CreatedOn { get; set; }

            public bool IsActive { get; set; }

            public int? VisitCount { get; set; }
        }
    }
}
