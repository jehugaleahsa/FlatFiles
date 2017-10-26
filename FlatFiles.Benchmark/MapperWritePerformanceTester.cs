using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class MapperWritePerformanceTester
    {
        private readonly ISeparatedValueTypeMapper<Person> mapper;
        private readonly Person[] people;

        public MapperWritePerformanceTester()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>(() => new Person());
            mapper.Property(x => x.Name).ColumnName("Name");
            mapper.Property(x => x.IQ).ColumnName("IQ");
            mapper.Property(x => x.BirthDate).ColumnName("BirthDate");
            mapper.Property(x => x.TopSpeed).ColumnName("TopSpeed");
            this.mapper = mapper;

            this.people = Enumerable.Range(0, 10000).Select(i => new Person()
            {
                Name = "Susan",
                IQ = 132,
                BirthDate = new DateTime(1984, 3, 15),
                TopSpeed = 10.1m
            }).ToArray();
        }

        [Benchmark(Description = "SerializeEmit")]
        public string SerializeEmit()
        {
            mapper.OptimizeMapping(true);
            StringWriter writer = new StringWriter();
            mapper.Write(writer, people);
            return writer.ToString();
        }

        [Benchmark(Description = "SerializeReflection")]
        public string SerializeReflection()
        {
            mapper.OptimizeMapping(false);
            StringWriter writer = new StringWriter();
            mapper.Write(writer, people);
            return writer.ToString();
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
