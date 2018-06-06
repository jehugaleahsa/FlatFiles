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
            mapper.Property(x => x.Address1.Street).ColumnName("Street");
            mapper.Property(x => x.Address1.City).ColumnName("City");
            mapper.Property(x => x.Address1.State).ColumnName("State");
            mapper.Property(x => x.Address1.Zip).ColumnName("Zip");
            mapper.Property(x => x.IsActive).ColumnName("IsActive");
            mapper.Property(x => x.CreatedOn).ColumnName("CreatedOn");

            var recordValues = new[] { "123", "Bob", "Test Street 1", "Test City", "PA", "55555", "true", "2017-11-05" };
            var record = String.Join(",", recordValues);
            StringReader reader = new StringReader(record);
            var results = mapper.Read(reader).ToArray();
            Assert.Single(results);
            var result = results[0];

            var expected = new Person()
            {
                Id = 123,
                Name = "Bob",
                Address1 = new Address()
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
            assertEqual(expected.Address1, result.Address1);
        }

        [Fact]
        public void TestMappingNestedMemberDynamically()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(Person));
            mapper.Int32Property("Id").ColumnName("Id");
            mapper.StringProperty("Name").ColumnName("Name");
            mapper.StringProperty("Address1.Street").ColumnName("Street");
            mapper.StringProperty("Address1.City").ColumnName("City");
            mapper.StringProperty("Address1.State").ColumnName("State");
            mapper.StringProperty("Address1.Zip").ColumnName("Zip");
            mapper.BooleanProperty("IsActive").ColumnName("IsActive");
            mapper.DateTimeProperty("CreatedOn").ColumnName("CreatedOn");

            var recordValues = new[] { "123", "Bob", "Test Street 1", "Test City", "PA", "55555", "true", "2017-11-05" };
            var record = String.Join(",", recordValues);
            StringReader reader = new StringReader(record);
            var results = mapper.Read(reader).ToArray();
            Assert.Single(results);
            var result = results[0];
            Assert.IsType<Person>(result);

            var expected = new Person()
            {
                Id = 123,
                Name = "Bob",
                Address1 = new Address()
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
            assertEqual(expected.Address1, ((Person)result).Address1);
        }

        [Fact]
        public void TestRoundTrip_FixedLength()
        {
            var mapper = FixedLengthTypeMapper.Define(() => new Person());
            mapper.Property(x => x.Id, 10).ColumnName("Id");
            mapper.Property(x => x.Name, 25).ColumnName("Name");
            mapper.Property(x => x.Address1.Street, 50).ColumnName("Street");
            mapper.Property(x => x.Address1.City, 50).ColumnName("City");
            mapper.Property(x => x.Address1.State, 2).ColumnName("State");
            mapper.Property(x => x.Address1.Zip, 5).ColumnName("Zip");
            mapper.Property(x => x.IsActive, 5).ColumnName("IsActive");
            mapper.Property(x => x.CreatedOn, 10).ColumnName("CreatedOn").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.UseFactory(() => new Address());

            var expected = new Person()
            {
                Id = 123,
                Name = "Bob",
                Address1 = new Address()
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
            Assert.Single(results);
            var result = results[0];

            assertEqual(expected, result);
            assertEqual(expected.Address1, result.Address1);
        }

        [Fact]
        public void TestRoundTrip_FixedLength_Dynamic()
        {
            var mapper = FixedLengthTypeMapper.DefineDynamic(typeof(Person), () => new Person());
            mapper.Int32Property("Id", 10).ColumnName("Id");
            mapper.StringProperty("Name", 25).ColumnName("Name");
            mapper.StringProperty("Address1.Street", 50).ColumnName("Street");
            mapper.StringProperty("Address1.City", 50).ColumnName("City");
            mapper.StringProperty("Address1.State", 2).ColumnName("State");
            mapper.StringProperty("Address1.Zip", 5).ColumnName("Zip");
            mapper.BooleanProperty("IsActive", 5).ColumnName("IsActive");
            mapper.DateTimeProperty("CreatedOn", 10).ColumnName("CreatedOn").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.UseFactory(typeof(Address), () => new Address());

            var expected = new Person()
            {
                Id = 123,
                Name = "Bob",
                Address1 = new Address()
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
            Assert.Single(results);
            var result = results[0];
            Assert.IsType<Person>(result);

            assertEqual(expected, (Person)result);
            assertEqual(expected.Address1, ((Person)result).Address1);
        }

        [Fact]
        public void TestMappingNestedMembers_MultipleSameType()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>();
            mapper.Property(x => x.Id).ColumnName("Id");
            mapper.Property(x => x.Name).ColumnName("Name");
            mapper.Property(x => x.Address1.Street).ColumnName("Street1");
            mapper.Property(x => x.Address1.City).ColumnName("City1");
            mapper.Property(x => x.Address1.State).ColumnName("State1");
            mapper.Property(x => x.Address1.Zip).ColumnName("Zip1");
            mapper.Property(x => x.Address2.Street).ColumnName("Street2");
            mapper.Property(x => x.Address2.City).ColumnName("City2");
            mapper.Property(x => x.Address2.State).ColumnName("State2");
            mapper.Property(x => x.Address2.Zip).ColumnName("Zip2");
            mapper.Property(x => x.IsActive).ColumnName("IsActive");
            mapper.Property(x => x.CreatedOn).ColumnName("CreatedOn");

            var expected = new Person()
            {
                Id = 123,
                Name = "Bob",
                Address1 = new Address()
                {
                    Street = "Test Street 1",
                    City = "Test City",
                    State = "PA",
                    Zip = "55555"
                },
                Address2 = new Address()
                {
                    Street = "Test Street 2",
                    City = "Test City 2",
                    State = "WA",
                    Zip = "44444"
                },
                IsActive = true,
                CreatedOn = new DateTime(2017, 11, 05)
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, new[] { expected });

            StringReader reader = new StringReader(writer.ToString());
            var results = mapper.Read(reader).ToArray();
            Assert.Single(results);
            var result = results[0];

            assertEqual(expected, result);
            assertEqual(expected.Address1, result.Address1);
            assertEqual(expected.Address2, result.Address2);
        }

        private static void assertEqual(Person expected, Person actual)
        {
            Assert.NotNull(actual);
            Assert.NotNull(actual.Address1);
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.IsActive, actual.IsActive);
            Assert.Equal(expected.CreatedOn, actual.CreatedOn);
        }

        private static void assertEqual(Address expected, Address actual)
        {
            Assert.Equal(expected.Street, actual.Street);
            Assert.Equal(expected.City, actual.City);
            Assert.Equal(expected.State, actual.State);
            Assert.Equal(expected.Zip, actual.Zip);
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

            public Address Address1 { get; set; }

            public Address Address2 { get; set; }

            public bool IsActive { get; set; }

            public DateTime CreatedOn { get; set; }
        }

        [Fact]
        public void TestMappingNestedMembers_DeepNesting()
        {
            var mapper = SeparatedValueTypeMapper.Define<Level1>();
            mapper.Property(x => x.Id).ColumnName("Id");
            mapper.Property(x => x.Level2.Name).ColumnName("Name");
            mapper.Property(x => x.Level2.Level3.Level4.Address.Street).ColumnName("Street1");
            mapper.Property(x => x.Level2.Level3.Level4.Address.City).ColumnName("City1");
            mapper.Property(x => x.Level2.Level3.Level4.Address.State).ColumnName("State1");
            mapper.Property(x => x.Level2.Level3.Level4.Address.Zip).ColumnName("Zip1");
            mapper.Property(x => x.Level2.Level3.Level4.IsActive).ColumnName("IsActive");
            mapper.Property(x => x.Level2.Level3.CreatedOn).ColumnName("CreatedOn");

            var expected = new Level1()
            {
                Id = 123,
                Level2 = new Level2()
                {
                    Name = "Bob",
                    Level3 = new Level3()
                    {
                        CreatedOn = new DateTime(2017, 11, 05),
                        Level4 = new Level4()
                        {
                            IsActive = true,
                            Address = new Address()
                            {
                                Street = "Test Street 1",
                                City = "Test City",
                                State = "PA",
                                Zip = "55555"
                            }
                        }
                    }
                }                
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, new[] { expected });

            StringReader reader = new StringReader(writer.ToString());
            var results = mapper.Read(reader).ToArray();
            Assert.Single(results);
            var result = results[0];

            Assert.NotNull(result);
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Level2.Name, result.Level2.Name);
            Assert.Equal(expected.Level2.Level3.Level4.IsActive, result.Level2.Level3.Level4.IsActive);
            Assert.Equal(expected.Level2.Level3.CreatedOn, result.Level2.Level3.CreatedOn);
            assertEqual(expected.Level2.Level3.Level4.Address, result.Level2.Level3.Level4.Address);
        }

        [Fact]
        public void TestMappingNestedMembers_DeepNesting_RecurringMemberNames()
        {
            var mapper = SeparatedValueTypeMapper.Define<Level1>();
            mapper.Property(x => x.Id).ColumnName("Id");
            mapper.Property(x => x.Level2.Name).ColumnName("Name");
            mapper.Property(x => x.Level2.Level3.Level4.Address.Street).ColumnName("Street1");
            mapper.Property(x => x.Level2.Level3.Level4.Address.City).ColumnName("City1");
            mapper.Property(x => x.Level2.Level3.Level4.Address.State).ColumnName("State1");
            mapper.Property(x => x.Level2.Level3.Level4.Address.Zip).ColumnName("Zip1");
            mapper.Property(x => x.Address.Street).ColumnName("Street2");
            mapper.Property(x => x.Address.City).ColumnName("City2");
            mapper.Property(x => x.Address.State).ColumnName("State2");
            mapper.Property(x => x.Address.Zip).ColumnName("Zip2");
            mapper.Property(x => x.Level2.Level3.Level4.IsActive).ColumnName("IsActive");
            mapper.Property(x => x.Level2.Level3.CreatedOn).ColumnName("CreatedOn");

            var expected = new Level1()
            {
                Id = 123,
                Address = new Address()
                {
                    Street = "Test Street 2",
                    City = "Test City 2",
                    State = "WA",
                    Zip = "44444"
                },
                Level2 = new Level2()
                {
                    Name = "Bob",
                    Level3 = new Level3()
                    {
                        CreatedOn = new DateTime(2017, 11, 05),
                        Level4 = new Level4()
                        {
                            IsActive = true,
                            Address = new Address()
                            {
                                Street = "Test Street 1",
                                City = "Test City",
                                State = "PA",
                                Zip = "55555"
                            }
                        }
                    }
                }
            };

            StringWriter writer = new StringWriter();
            mapper.Write(writer, new[] { expected });

            StringReader reader = new StringReader(writer.ToString());
            var results = mapper.Read(reader).ToArray();
            Assert.Single(results);
            var result = results[0];

            Assert.NotNull(result);
            Assert.Equal(expected.Id, result.Id);
            Assert.Equal(expected.Level2.Name, result.Level2.Name);
            Assert.Equal(expected.Level2.Level3.Level4.IsActive, result.Level2.Level3.Level4.IsActive);
            Assert.Equal(expected.Level2.Level3.CreatedOn, result.Level2.Level3.CreatedOn);
            assertEqual(expected.Address, result.Address);
            assertEqual(expected.Level2.Level3.Level4.Address, result.Level2.Level3.Level4.Address);
        }


        public class Level1
        {
            public int Id { get; set; }

            public Address Address { get; set; }

            public Level2 Level2 { get; set; }
        }

        public class Level2
        {
            public string Name { get; set; }

            public Level3 Level3 { get; set; }
        }

        public class Level3
        {
            public DateTime CreatedOn { get; set; }

            public Level4 Level4 { get; set; }
        }

        public class Level4
        {
            public bool IsActive { get; set; }

            public Address Address { get; set; }
        }
    }
}
