using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class NestedTypeTester
    {
        [Fact]
        public void TestMappingNestedMember()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>();
            mapper.Property(x => x.Id).ColumnName("Id");
            mapper.Property(x => x.Name).ColumnName("Name");
            mapper.Property(x => x.Address.Street).ColumnName("Street");
            mapper.Property(x => x.Address.City).ColumnName("City");
            mapper.Property(x => x.Address.State).ColumnName("State");
            mapper.Property(x => x.Address.Zip).ColumnName("Zip");
            mapper.Property(x => x.IsActive).ColumnName("IsActive");
            mapper.Property(x => x.CreatedOn).ColumnName("CreatedOn");

            var recordValues = new[] { "123", "Bob", "Test Street 1", "Test City", "PA", "55555", "true", "2017-11-05" };
            var record = String.Join(",", recordValues);
            StringReader reader = new StringReader(record);
            var results = mapper.Read(reader).ToArray();
            Assert.Equal(1, results.Length);
            var result = results[0];

            var expected = new Person()
            {
                Id = 123,
                Name = "Bob",
                Address = new Address()
                {
                    Street = "Test Street 1",
                    City = "Test City",
                    State = "PA",
                    Zip = "55555"
                },
                IsActive = true,
                CreatedOn = new DateTime(2017, 11, 05)
            };
            assertEqual(expected, result);
        }

        [Fact]
        public void TestMappingNestedMemberDynamically()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(Person));
            mapper.Int32Property("Id").ColumnName("Id");
            mapper.StringProperty("Name").ColumnName("Name");
            mapper.StringProperty("Address.Street").ColumnName("Street");
            mapper.StringProperty("Address.City").ColumnName("City");
            mapper.StringProperty("Address.State").ColumnName("State");
            mapper.StringProperty("Address.Zip").ColumnName("Zip");
            mapper.BooleanProperty("IsActive").ColumnName("IsActive");
            mapper.DateTimeProperty("CreatedOn").ColumnName("CreatedOn");

            var recordValues = new[] { "123", "Bob", "Test Street 1", "Test City", "PA", "55555", "true", "2017-11-05" };
            var record = String.Join(",", recordValues);
            StringReader reader = new StringReader(record);
            var results = mapper.Read(reader).ToArray();
            Assert.Equal(1, results.Length);
            var result = results[0];
            Assert.IsType<Person>(result);

            var expected = new Person()
            {
                Id = 123,
                Name = "Bob",
                Address = new Address()
                {
                    Street = "Test Street 1",
                    City = "Test City",
                    State = "PA",
                    Zip = "55555"
                },
                IsActive = true,
                CreatedOn = new DateTime(2017, 11, 05)
            };
            assertEqual(expected, (Person)result);
        }

        [Fact]
        public void TestRoundTrip_FixedLength()
        {
            var mapper = FixedLengthTypeMapper.Define(() => new Person());
            mapper.Property(x => x.Id, 10).ColumnName("Id");
            mapper.Property(x => x.Name, 25).ColumnName("Name");
            mapper.Property(x => x.Address.Street, 50).ColumnName("Street");
            mapper.Property(x => x.Address.City, 50).ColumnName("City");
            mapper.Property(x => x.Address.State, 2).ColumnName("State");
            mapper.Property(x => x.Address.Zip, 5).ColumnName("Zip");
            mapper.Property(x => x.IsActive, 5).ColumnName("IsActive");
            mapper.Property(x => x.CreatedOn, 10).ColumnName("CreatedOn").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.UseFactory(() => new Address());

            var expected = new Person()
            {
                Id = 123,
                Name = "Bob",
                Address = new Address()
                {
                    Street = "Test Street 1",
                    City = "Test City",
                    State = "PA",
                    Zip = "55555"
                },
                IsActive = true,
                CreatedOn = new DateTime(2017, 11, 05)
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, new[] { expected });
            
            StringReader reader = new StringReader(writer.ToString());
            var results = mapper.Read(reader).ToArray();
            Assert.Equal(1, results.Length);
            var result = results[0];

            assertEqual(expected, result);
        }

        [Fact]
        public void TestRoundTrip_FixedLength_Dynamic()
        {
            var mapper = FixedLengthTypeMapper.DefineDynamic(typeof(Person), () => new Person());
            mapper.Int32Property("Id", 10).ColumnName("Id");
            mapper.StringProperty("Name", 25).ColumnName("Name");
            mapper.StringProperty("Address.Street", 50).ColumnName("Street");
            mapper.StringProperty("Address.City", 50).ColumnName("City");
            mapper.StringProperty("Address.State", 2).ColumnName("State");
            mapper.StringProperty("Address.Zip", 5).ColumnName("Zip");
            mapper.BooleanProperty("IsActive", 5).ColumnName("IsActive");
            mapper.DateTimeProperty("CreatedOn", 10).ColumnName("CreatedOn").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.UseFactory(typeof(Address), () => new Address());

            var expected = new Person()
            {
                Id = 123,
                Name = "Bob",
                Address = new Address()
                {
                    Street = "Test Street 1",
                    City = "Test City",
                    State = "PA",
                    Zip = "55555"
                },
                IsActive = true,
                CreatedOn = new DateTime(2017, 11, 05)
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, new[] { expected });

            StringReader reader = new StringReader(writer.ToString());
            var results = mapper.Read(reader).ToArray();
            Assert.Equal(1, results.Length);
            var result = results[0];
            Assert.IsType<Person>(result);

            assertEqual(expected, (Person)result);
        }

        private static void assertEqual(Person expected, Person actual)
        {
            Assert.NotNull(actual);
            Assert.NotNull(actual.Address);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Address.Street, actual.Address.Street);
            Assert.Equal(expected.Address.City, actual.Address.City);
            Assert.Equal(expected.Address.State, actual.Address.State);
            Assert.Equal(expected.Address.Zip, actual.Address.Zip);
            Assert.Equal(expected.IsActive, actual.IsActive);
            Assert.Equal(expected.CreatedOn, actual.CreatedOn);
        }

        public class Address
        {
            public string Street { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Zip { get; set; }
        }

        public class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public Address Address { get; set; }

            public bool IsActive { get; set; }

            public DateTime CreatedOn { get; set; }
        }
    }
}
