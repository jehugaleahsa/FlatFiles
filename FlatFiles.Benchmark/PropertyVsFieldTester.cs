using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class PropertyVsFieldTester
    {
        private readonly PropertyPerson[] propertyPeople;
        private readonly FieldPerson[] fieldPeople;

        public PropertyVsFieldTester()
        {
            PropertyPerson propertyPerson = new PropertyPerson()
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 29,
                Street1 = "West Street Rd",
                Street2 = "Apt 23",
                City = "Lexington",
                State = "DE",
                Zip = "001569",
                FavoriteColor = "Blue",
                FavoriteFood = "Cheese and Crackers",
                FavoriteSport = "Soccer",
                CreatedOn = new DateTime(2017, 01, 01),
                IsActive = true
            };
            propertyPeople = Enumerable.Repeat(0, 10000).Select(i => propertyPerson).ToArray();

            FieldPerson fieldPerson = new FieldPerson()
            {
                FirstName = "John",
                LastName = "Smith",
                Age = 29,
                Street1 = "West Street Rd",
                Street2 = "Apt 23",
                City = "Lexington",
                State = "DE",
                Zip = "001569",
                FavoriteColor = "Blue",
                FavoriteFood = "Cheese and Crackers",
                FavoriteSport = "Soccer",
                CreatedOn = new DateTime(2017, 01, 01),
                IsActive = true
            };
            fieldPeople = Enumerable.Repeat(0, 10000).Select(i => fieldPerson).ToArray();
        }

        [Benchmark]
        public void RunPropertyTest()
        {
            var mapper = DelimitedTypeMapper.Define<PropertyPerson>(() => new PropertyPerson());
            mapper.Property(x => x.FirstName);
            mapper.Property(x => x.LastName);
            mapper.Property(x => x.Age);
            mapper.Property(x => x.Street1);
            mapper.Property(x => x.Street2);
            mapper.Property(x => x.City);
            mapper.Property(x => x.State);
            mapper.Property(x => x.Zip);
            mapper.Property(x => x.FavoriteColor);
            mapper.Property(x => x.FavoriteFood);
            mapper.Property(x => x.FavoriteSport);
            mapper.Property(x => x.CreatedOn);
            mapper.Property(x => x.IsActive);

            StringWriter writer = new StringWriter();
            mapper.Write(writer, propertyPeople);
            string serialized = writer.ToString();

            StringReader reader = new StringReader(serialized);
            var deserialized = mapper.Read(reader).ToArray();
        }

        [Benchmark]
        public void RunFieldTest()
        {
            var mapper = DelimitedTypeMapper.Define<FieldPerson>(() => new FieldPerson());
            mapper.Property(x => x.FirstName);
            mapper.Property(x => x.LastName);
            mapper.Property(x => x.Age);
            mapper.Property(x => x.Street1);
            mapper.Property(x => x.Street2);
            mapper.Property(x => x.City);
            mapper.Property(x => x.State);
            mapper.Property(x => x.Zip);
            mapper.Property(x => x.FavoriteColor);
            mapper.Property(x => x.FavoriteFood);
            mapper.Property(x => x.FavoriteSport);
            mapper.Property(x => x.CreatedOn);
            mapper.Property(x => x.IsActive);

            StringWriter writer = new StringWriter();
            mapper.Write(writer, fieldPeople);
            string serialized = writer.ToString();

            StringReader reader = new StringReader(serialized);
            var deserialized = mapper.Read(reader).ToArray();
        }
        
        public class PropertyPerson
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int Age { get; set; }

            public string Street1 { get; set; }

            public string Street2 { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Zip { get; set; }

            public string FavoriteColor { get; set; }

            public string FavoriteFood { get; set; }

            public string FavoriteSport { get; set; }
            
            public DateTime? CreatedOn { get; set; }

            public bool IsActive { get; set; }
        }

        public class FieldPerson
        {
            public string FirstName;
            public string LastName;
            public int Age;
            public string Street1;
            public string Street2;
            public string City;
            public string State;
            public string Zip;
            public string FavoriteColor;
            public string FavoriteFood;
            public string FavoriteSport;
            public DateTime? CreatedOn;
            public bool IsActive;
        }
    }
}
