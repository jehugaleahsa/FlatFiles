using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class FixedLengthTypeMapperMixedMemberTester
    {
        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [Fact]
        public void TestTypeMapper_Roundtrip()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(p => p.IsActive, new Window(5)).ColumnName("active");

            var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19), IsActive = true };
            var options = new FixedLengthOptions() { FillCharacter = '@' };

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Person[] { bob }, options);

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader, options).ToArray();
            Assert.Equal(1, people.Count());
            var person = people.SingleOrDefault();
            Assert.Equal(bob.Id, person.Id);
            Assert.Equal(bob.Name, person.Name);
            Assert.Equal(bob.Created, person.Created);
        }

        private class Person
        {
            public int Id;

            public string Name { get; set; }

            public DateTime Created;

            public bool? IsActive { get; set; }
        }
    }
}
