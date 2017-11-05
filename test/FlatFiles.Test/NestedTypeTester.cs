using System;
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
            mapper.Property(x => x.Address.Street).ColumnName("Street");
        }

        [Fact]
        public void TestMappingNestedMemberDynamically()
        {
            var mapper = SeparatedValueTypeMapper.DefineDynamic(typeof(Person));
            mapper.StringProperty("Address.Street").ColumnName("Street");
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
