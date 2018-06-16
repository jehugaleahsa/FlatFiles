using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using CsvHelper.Configuration;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class SimpleCsvTester
    {
        private readonly string data;

        public SimpleCsvTester()
        {
            string[] headers = new string[]
            {
                "FirstName", "LastName", "Age", "Street1", "Street2", "City", "State", "Zip", "FavoriteColor", "FavoriteFood", "FavoriteSport", "CreatedOn", "IsActive"
            };
            string[] values = new string[]
            {
                "John", "Smith", "29", "West Street Rd", "Apt 23", "Lexington", "DE", "001569", "Blue", "Cheese and Crackers", "Soccer", "2017-01-01", "true"
            };
            string header = String.Join(",", headers);
            string record = String.Join(",", values);
            data = String.Join(Environment.NewLine, (new[] { header }).Concat(Enumerable.Repeat(0, 10000).Select(i => record)));
        }

        [Benchmark]
        public void RunCsvHelper()
        {
            StringReader reader = new StringReader(data);
            var csvReader = new CsvHelper.CsvReader(reader, new Configuration() {  });
            var people = csvReader.GetRecords<Person>().ToArray();
        }

        [Benchmark]
        public async Task RunCsvHelperAsync()
        {
            StringReader reader = new StringReader(data);
            var csvReader = new CsvHelper.CsvReader(reader, new Configuration() { });
            List<Person> people = new List<Person>();
            await csvReader.ReadAsync().ConfigureAwait(false);
            csvReader.ReadHeader();
            while (await csvReader.ReadAsync().ConfigureAwait(false))
            {
                people.Add(csvReader.GetRecord<Person>());
            }
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
            mapper.Property(x => x.CreatedOn);
            mapper.Property(x => x.IsActive);

            StringReader reader = new StringReader(data);
            var people = mapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
        }

        [Benchmark]
        public async Task RunFlatFilesAsync()
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
            mapper.Property(x => x.CreatedOn);
            mapper.Property(x => x.IsActive);

            StringReader textReader = new StringReader(data);
            var people = new List<Person>();
            var reader = mapper.GetReader(textReader, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            while (await reader.ReadAsync().ConfigureAwait(false))
            {
                people.Add(reader.Current);
            }
        }

        [Benchmark]
        public void RunStringSplit()
        {
            var lines = data.Split(Environment.NewLine);
            var records = lines.Skip(1).Select(l => l.Split(",").ToArray());
            var people = new List<Person>();
            foreach (var record in records)
            {
                Person person = new Person();
                person.FirstName = record[0];
                person.LastName = record[1];
                person.Age = Int32.Parse(record[2]);
                person.Street1 = record[3];
                person.Street2 = record[4];
                person.City = record[5];
                person.State = record[6];
                person.Zip = record[7];
                person.FavoriteColor = record[8];
                person.FavoriteFood = record[9];
                person.FavoriteSport = record[10];
                person.CreatedOn = DateTime.Parse(record[11]);
                person.IsActive = Boolean.Parse(record[12]);
                people.Add(person);
            }
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
