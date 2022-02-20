using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class DelimitedTypeMapperFieldTester
    {
        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [TestMethod]
        public void TestTypeMapper_Roundtrip()
        {
            var mapper = DelimitedTypeMapper.Define<Person>();
            mapper.Property(p => p.Id).ColumnName("id");
            mapper.Property(p => p.Name).ColumnName("name");
            mapper.Property(p => p.Created).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(p => p.IsActive).ColumnName("active");

            var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19), IsActive = true };

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Person[] { bob });

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, people.Length);
            var person = people.SingleOrDefault();
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
        }

        internal class Person
        {
            public int Id;

            public string Name;

            public DateTime Created;

            public bool? IsActive;
        }
    }
}
