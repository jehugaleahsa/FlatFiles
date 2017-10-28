using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class QuotedCsvTester
    {
        private readonly string data;

        public QuotedCsvTester()
        {
            string[] headers = new string[]
            {
                "FirstName", "LastName", "Age", "Street1", "Street2", "City", "State", "Zip", "FavoriteColor", "FavoriteFood", "FavoriteSport", "CreatedOn", "IsActive"
            };
            string[] values = new string[]
            {
                "Joe", "Smith", "29", "\"West Street Rd, Apt. 23\"", "ATTN: Will Smith", "Lexington", "DE", "001569", "Blue", "\"Cheese, and Crackers\"", "Soccer", "2017-01-01", "true"
            };
            string header = String.Join(",", headers);
            string record = String.Join(",", values);
            data = String.Join(Environment.NewLine, (new[] { header }).Concat(Enumerable.Repeat(0, 1000).Select(i => record)));
        }

        [Benchmark]
        public void RunCsvHelper()
        {
            StringReader reader = new StringReader(data);
            var csvReader = new CsvHelper.CsvReader(reader);
            var people = csvReader.GetRecords<Person>().ToArray();
        }

        [Benchmark]
        public void RunFlatFiles()
        {
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

            StringReader reader = new StringReader(data);
            var people = mapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
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
