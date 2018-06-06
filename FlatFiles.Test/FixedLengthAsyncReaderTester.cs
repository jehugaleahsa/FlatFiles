using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the FixedLengthParserTester class.
    /// </summary>
    public class FixedLengthAsyncReaderTester
    {
        public FixedLengthAsyncReaderTester()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [Fact]
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
            Assert.Single(people);
            var person = people.SingleOrDefault();
            Assert.Equal(bob.Id, person.Id);
            Assert.Equal(bob.Name, person.Name);
            Assert.Equal(bob.Created, person.Created);
            Assert.True(person.IsActive);
        }

        [Fact]
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
            Assert.Equal(2, people.Count());
            var person1 = people.First();
            Assert.Equal(bob.Id, person1.Id);
            Assert.Equal(bob.Name, person1.Name);
            Assert.Equal(bob.Created, person1.Created);
            Assert.True(person1.IsActive);
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
