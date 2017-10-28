using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using CsvHelper.Configuration;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class PreconfiguredCsvTester
    {
        private readonly string data;
        private readonly Configuration csvHelperConfig;
        private readonly ISeparatedValueTypeMapper<Person> typeMapper;

        public PreconfiguredCsvTester()
        {
            string[] values = new string[]
            {
                "Joe", "Smith", "29", "\"West Street Rd, Apt. 23\"", "ATTN: Will Smith", "Lexington", "DE", "001569", "Blue", "\"Cheese, and Crackers\"", "Soccer", "2017-01-01", "true"
            };
            string record = String.Join(",", values);
            data = String.Join(Environment.NewLine, Enumerable.Repeat(0, 1000).Select(i => record));

            csvHelperConfig = new Configuration();
            csvHelperConfig.HasHeaderRecord = false;
            csvHelperConfig.HeaderValidated = null;
            csvHelperConfig.AutoMap<Person>().Map(x => x.FirstName).Index(0);
            csvHelperConfig.AutoMap<Person>().Map(x => x.LastName).Index(1);
            csvHelperConfig.AutoMap<Person>().Map(x => x.Age).Index(2);
            csvHelperConfig.AutoMap<Person>().Map(x => x.Street1).Index(3);
            csvHelperConfig.AutoMap<Person>().Map(x => x.Street2).Index(4);
            csvHelperConfig.AutoMap<Person>().Map(x => x.City).Index(5);
            csvHelperConfig.AutoMap<Person>().Map(x => x.State).Index(6);
            csvHelperConfig.AutoMap<Person>().Map(x => x.Zip).Index(7);
            csvHelperConfig.AutoMap<Person>().Map(x => x.FavoriteColor).Index(8);
            csvHelperConfig.AutoMap<Person>().Map(x => x.FavoriteFood).Index(9);
            csvHelperConfig.AutoMap<Person>().Map(x => x.FavoriteSport).Index(10);
            csvHelperConfig.AutoMap<Person>().Map(x => x.CreatedOn).Index(11);
            csvHelperConfig.AutoMap<Person>().Map(x => x.IsActive).Index(12);

            var mapper = SeparatedValueTypeMapper.Define<Person>(() => new Person());
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
            this.typeMapper = mapper;
        }

        [Benchmark]
        public void RunCsvHelper()
        {
            StringReader reader = new StringReader(data);
            var csvReader = new CsvHelper.CsvReader(reader, csvHelperConfig);
            var people = csvReader.GetRecords<Person>().ToArray();
        }

        [Benchmark]
        public void RunFlatFiles()
        {
            StringReader reader = new StringReader(data);
            var people = typeMapper.Read(reader).ToArray();
        }

        public class Person
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
    }
}
