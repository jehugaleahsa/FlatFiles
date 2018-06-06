using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class FixedLengthRuntimeTypeTester
    {
        [Fact]
        public void TestAnonymousTypeDefinition()
        {
            var mapper = FixedLengthTypeMapper.Define(() => new
            {
                Name = (string)null
            });
            mapper.Property(x => x.Name, 10).ColumnName("Name");
            StringWriter writer = new StringWriter();
            mapper.Write(writer, new[]
            {
                new { Name = "John" }, new { Name = "Sam" }
            });
            string result = writer.ToString();
            string expected = $"John      {Environment.NewLine}Sam       {Environment.NewLine}";
            Assert.Equal(expected, result);
        }

        [Fact]
        public void TestRuntimeTypeDefinition()
        {
            var mapper = FixedLengthTypeMapper.DefineDynamic(typeof(Person));
            mapper.StringProperty("Name", 10).ColumnName("Name");
            mapper.Int32Property("IQ", 3).ColumnName("IQ");
            mapper.DateTimeProperty("BirthDate", 10).ColumnName("BirthDate").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.DecimalProperty("TopSpeed", 10).ColumnName("TopSpeed");

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
            Assert.Equal(2, parsed.Length);
            Assert.IsType<Person>(parsed[0]);
            Assert.IsType<Person>(parsed[1]);
            assertEqual(people[0], (Person)parsed[0]);
            assertEqual(people[1], (Person)parsed[1]);
        }

        [Fact]
        public void TestRuntimeTypeDefinition_ReaderWriter()
        {
            var mapper = FixedLengthTypeMapper.DefineDynamic(typeof(Person));
            mapper.StringProperty("Name", 10).ColumnName("Name");
            mapper.Int32Property("IQ", 3).ColumnName("IQ");
            mapper.DateTimeProperty("BirthDate", 10).ColumnName("BirthDate").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.DecimalProperty("TopSpeed", 10).ColumnName("TopSpeed");

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
            Assert.Equal(2, parsed.Count);
            Assert.IsType<Person>(parsed[0]);
            Assert.IsType<Person>(parsed[1]);
            assertEqual(people[0], (Person)parsed[0]);
            assertEqual(people[1], (Person)parsed[1]);
        }

        private void assertEqual(Person person1, Person person2)
        {
            Assert.Equal(person1.Name, person2.Name);
            Assert.Equal(person1.IQ, person2.IQ);
            Assert.Equal(person1.BirthDate, person2.BirthDate);
            Assert.Equal(person1.TopSpeed, person2.TopSpeed);
        }

        public class Person
        {
            public string Name { get; set; }

            public int? IQ { get; set; }

            public DateTime BirthDate { get; set; }

            public decimal TopSpeed { get; set; }
        }
    }
}
