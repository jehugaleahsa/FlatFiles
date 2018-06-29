using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class SeparatedValueRuntimeTypeTester
    {
        [TestMethod]
        public void TestAnonymousTypeDefinition()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new
            {
                Name = (string)null
            });
            mapper.Property(x => x.Name).ColumnName("Name");
            StringWriter writer = new StringWriter();
            mapper.Write(writer, new[]
            {
                new { Name = "John" }, new { Name = "Sam" }
            });
            string result = writer.ToString();
            string expected = $"John{Environment.NewLine}Sam{Environment.NewLine}";
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void TestRuntimeTypeDefinition()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(Person));
            mapper.StringProperty("Name").ColumnName("Name");
            mapper.Int32Property("IQ").ColumnName("IQ");
            mapper.DateTimeProperty("BirthDate").ColumnName("BirthDate");
            mapper.DecimalProperty("TopSpeed").ColumnName("TopSpeed");

            var people = new[]
            {
                new Person() { Name = "John", IQ = null, BirthDate = new DateTime(1954, 10, 29), TopSpeed = 3.4m },
                new Person() { Name = "Susan", IQ = 132, BirthDate = new DateTime(1984, 3, 15), TopSpeed = 10.1m }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people);
            string result = writer.ToString();

            StringReader reader = new StringReader(result);
            var parsed = mapper.Read(reader).ToArray();
            Assert.AreEqual(2, parsed.Length);
            Assert.IsInstanceOfType(parsed[0], typeof(Person));
            Assert.IsInstanceOfType(parsed[1], typeof(Person));
            assertEqual(people[0], (Person)parsed[0]);
            assertEqual(people[1], (Person)parsed[1]);
        }

        [TestMethod]
        public void TestRuntimeTypeDefinition_ReaderWriter()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(Person));
            mapper.StringProperty("Name").ColumnName("Name");
            mapper.Int32Property("IQ").ColumnName("IQ");
            mapper.DateTimeProperty("BirthDate").ColumnName("BirthDate");
            mapper.DecimalProperty("TopSpeed").ColumnName("TopSpeed");

            var people = new[]
            {
                new Person() { Name = "John", IQ = null, BirthDate = new DateTime(1954, 10, 29), TopSpeed = 3.4m },
                new Person() { Name = "Susan", IQ = 132, BirthDate = new DateTime(1984, 3, 15), TopSpeed = 10.1m }
            };

            StringWriter writer = new StringWriter();
            var entityWriter = mapper.GetWriter(writer);
            foreach (var person in people)
            {
                entityWriter.Write(person);
            }
            string result = writer.ToString();

            StringReader reader = new StringReader(result);
            var entityReader = mapper.GetReader(reader);
            var parsed = new List<object>();
            while (entityReader.Read())
            {
                parsed.Add(entityReader.Current);
            }
            Assert.AreEqual(2, parsed.Count);
            Assert.IsInstanceOfType(parsed[0], typeof(Person));
            Assert.IsInstanceOfType(parsed[1], typeof(Person));
            assertEqual(people[0], (Person)parsed[0]);
            assertEqual(people[1], (Person)parsed[1]);
        }

        private void assertEqual(Person person1, Person person2)
        {
            Assert.AreEqual(person1.Name, person2.Name);
            Assert.AreEqual(person1.IQ, person2.IQ);
            Assert.AreEqual(person1.BirthDate, person2.BirthDate);
            Assert.AreEqual(person1.TopSpeed, person2.TopSpeed);
        }

        [TestMethod]
        public void TestInternalType()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(InternalPerson));
            mapper.StringProperty("Name").ColumnName("Name");

            string expected = $"John{Environment.NewLine}Susan{Environment.NewLine}";

            StringReader reader = new StringReader(expected);
            var people = mapper.Read(reader).ToArray();
            Assert.AreEqual(2, people.Length);
            Assert.IsInstanceOfType(people[0], typeof(InternalPerson));
            Assert.IsInstanceOfType(people[1], typeof(InternalPerson));

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people);

            string actual = writer.ToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestInternalType_Unoptimized()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(InternalPerson));
            mapper.StringProperty("Name").ColumnName("Name");
            mapper.OptimizeMapping(false);

            string expected = $"John{Environment.NewLine}Susan{Environment.NewLine}";

            StringReader reader = new StringReader(expected);
            var people = mapper.Read(reader).ToArray();
            Assert.AreEqual(2, people.Length);
            Assert.IsInstanceOfType(people[0], typeof(InternalPerson));
            Assert.IsInstanceOfType(people[1], typeof(InternalPerson));

            StringWriter writer = new StringWriter();
            mapper.Write(writer, people);

            string actual = writer.ToString();
            Assert.AreEqual(expected, actual);
        }

        public class Person
        {
            public string Name { get; set; }

            public int? IQ { get; set; }

            public DateTime BirthDate { get; set; }

            public decimal TopSpeed { get; set; }
        }

        internal class InternalPerson
        {
            internal InternalPerson()
            {
            }

            internal string Name { get; set; }
        }
    }
}
