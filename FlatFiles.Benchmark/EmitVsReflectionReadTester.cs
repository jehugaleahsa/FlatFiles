using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class EmitVsReflectionReadTester
    {
        private readonly ISeparatedValueTypeMapper<Person> mapper;
        private readonly string peopleData;

        public EmitVsReflectionReadTester()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>(() => new Person());
            mapper.Property(x => x.Name).ColumnName("Name");
            mapper.Property(x => x.IQ).ColumnName("IQ");
            mapper.Property(x => x.BirthDate).ColumnName("BirthDate");
            mapper.Property(x => x.TopSpeed).ColumnName("TopSpeed");
            this.mapper = mapper;

            var people = Enumerable.Range(0, 10000).Select(i => new Person()
            {
                Name = "Susan",
                IQ = 132,
                BirthDate = new DateTime(1984, 3, 15),
                TopSpeed = 10.1m
            }).ToArray();
            StringWriter writer = new StringWriter();
            mapper.Write(writer, people);
            peopleData = writer.ToString();
        }

        [Benchmark(Description = "DeserializeEmit")]
        public void DeserializeEmit()
        {
            mapper.OptimizeMapping(true);
            StringReader reader = new StringReader(peopleData);
            mapper.Read(reader).ToList();
        }

        [Benchmark(Description = "DeserializeReflection")]
        public void DeserializeReflection()
        {
            mapper.OptimizeMapping(false);
            StringReader reader = new StringReader(peopleData);
            mapper.Read(reader).ToList();
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
