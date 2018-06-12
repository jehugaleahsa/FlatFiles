using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the FixedLengthParserTester class.
    /// </summary>
    [TestClass]
    public class FixedLengthAsyncReaderTester
    {
        public FixedLengthAsyncReaderTester()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [TestMethod]
        public async Task TestTypeMapper_Roundtrip()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(p => p.IsActive, new Window(5)).ColumnName("active");

            var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19), IsActive = true };

            StringWriter stringWriter = new StringWriter();
            await mapper.WriteAsync(stringWriter, new Person[] { bob });

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var reader = mapper.GetReader(stringReader);
            var people = new List<Person>();
            while (await reader.ReadAsync())
            {
                people.Add(reader.Current);
            }
            Assert.AreEqual(1, people.Count);
            var person = people.SingleOrDefault();
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
            Assert.AreEqual(true, person.IsActive);
        }

        [TestMethod]
        public async Task TestTypeMapper_Roundtrip_NoSeparator()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(p => p.IsActive, new Window(5)).ColumnName("active");

            var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19), IsActive = true };
            var options = new FixedLengthOptions() { HasRecordSeparator = false };

            StringWriter stringWriter = new StringWriter();
            await mapper.WriteAsync(stringWriter, new Person[] { bob, bob }, options);

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var reader = mapper.GetReader(stringReader, options);
            var people = new List<Person>();
            while (await reader.ReadAsync())
            {
                people.Add(reader.Current);
            }
            Assert.AreEqual(2, people.Count());
            var person1 = people.First();
            Assert.AreEqual(bob.Id, person1.Id);
            Assert.AreEqual(bob.Name, person1.Name);
            Assert.AreEqual(bob.Created, person1.Created);
            Assert.AreEqual(true, person1.IsActive);
        }

        private class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Created { get; set; }

            public bool? IsActive { get; set; }
        }
    }
}
