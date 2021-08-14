using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class DirectVsDynamicTester
    {
        private readonly ISeparatedValueTypeMapper<Person> directMapper;
        private readonly IDynamicSeparatedValueTypeMapper dynamicMapper;
        private readonly Person[] people;

        public DirectVsDynamicTester()
        {
            var directMapper = SeparatedValueTypeMapper.Define<Person>(() => new Person());
            directMapper.Property(x => x.Name).ColumnName("Name");
            directMapper.Property(x => x.IQ).ColumnName("IQ");
            directMapper.Property(x => x.BirthDate).ColumnName("BirthDate");
            directMapper.Property(x => x.TopSpeed).ColumnName("TopSpeed");
            directMapper.Property(x => x.IsActive).ColumnName("IsActive");
            this.directMapper = directMapper;

            var dynamicMapper = SeparatedValueTypeMapper.DefineDynamic(typeof(Person));
            dynamicMapper.StringProperty("Name").ColumnName("Name");
            dynamicMapper.Int32Property("IQ").ColumnName("IQ");
            dynamicMapper.DateTimeProperty("BirthDate").ColumnName("BirthDate");
            dynamicMapper.DecimalProperty("TopSpeed").ColumnName("TopSpeed");
            dynamicMapper.BooleanProperty("IsActive").ColumnName("IsActive");
            this.dynamicMapper = dynamicMapper;

            people = Enumerable.Range(0, 10000).Select(i => new Person
                                                            {
                Name = "Susan",
                IQ = 132,
                BirthDate = new DateTime(1984, 3, 15),
                TopSpeed = 10.1m
            }).ToArray();
        }

        [Benchmark]
        public void Direct()
        {
            StringWriter writer = new StringWriter();
            directMapper.Write(writer, people);
            string peopleData = writer.ToString();

            StringReader reader = new StringReader(peopleData);
            directMapper.Read(reader).ToList();
        }

        [Benchmark]
        public void Dynamic()
        {
            StringWriter writer = new StringWriter();
            dynamicMapper.Write(writer, people);
            string peopleData = writer.ToString();
            
            StringReader reader = new StringReader(peopleData);
            dynamicMapper.Read(reader).ToList();
        }

        public class Person
        {
            public string Name { get; set; }

            public int? IQ { get; set; }

            public DateTime BirthDate { get; set; }

            public decimal TopSpeed { get; set; }

            public bool IsActive { get; set; }
        }
    }
}
