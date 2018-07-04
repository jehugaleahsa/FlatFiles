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
        public void RunFlatFiles_FlatFileReader()
        {
            var reader = new StringReader(data);
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("FirstName"));
            schema.AddColumn(new StringColumn("LastName"));
            schema.AddColumn(new Int32Column("Age"));
            schema.AddColumn(new StringColumn("Street1"));
            schema.AddColumn(new StringColumn("Street2"));
            schema.AddColumn(new StringColumn("City"));
            schema.AddColumn(new StringColumn("State"));
            schema.AddColumn(new StringColumn("Zip"));
            schema.AddColumn(new StringColumn("FavoriteColor"));
            schema.AddColumn(new StringColumn("FavoriteFood"));
            schema.AddColumn(new StringColumn("FavoriteSport"));
            schema.AddColumn(new DateTimeColumn("CreatedOn"));
            schema.AddColumn(new BooleanColumn("IsActive"));
            var csvReader = new SeparatedValueReader(reader, schema, new SeparatedValueOptions() { IsFirstRecordSchema = true });
            var dataReader = new FlatFileDataReader(csvReader);
            var people = new List<Person>();
            while (dataReader.Read())
            {
                var person = new Person()
                {
                    FirstName = dataReader.GetString(0),
                    LastName = dataReader.GetString(1),
                    Age = dataReader.GetInt32(2),
                    Street1 = dataReader.GetString(3),
                    Street2 = dataReader.GetString(4),
                    City = dataReader.GetString(5),
                    State = dataReader.GetString(6),
                    Zip = dataReader.GetString(7),
                    FavoriteColor = dataReader.GetString(8),
                    FavoriteFood = dataReader.GetString(9),
                    FavoriteSport = dataReader.GetString(10),
                    CreatedOn = dataReader.GetDateTime(11),
                    IsActive = dataReader.GetBoolean(12)
                };
                people.Add(person);
            }
        }

        [Benchmark]
        public void RunFlatFiles()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new Person());
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
        public void RunFlatFiles_Unoptimized()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new Person());
            mapper.OptimizeMapping(false);
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
        public void RunFlatFiles_CustomMapping()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new Person());
            mapper.CustomMapping(new StringColumn("FirstName")).WithReader(p => p.FirstName);
            mapper.CustomMapping(new StringColumn("LastName")).WithReader(p => p.LastName);
            mapper.CustomMapping(new Int32Column("Age")).WithReader(p => p.Age);
            mapper.CustomMapping(new StringColumn("Street1")).WithReader(p => p.Street1);
            mapper.CustomMapping(new StringColumn("Street2")).WithReader(p => p.Street2);
            mapper.CustomMapping(new StringColumn("City")).WithReader(p => p.City);
            mapper.CustomMapping(new StringColumn("State")).WithReader(p => p.State);
            mapper.CustomMapping(new StringColumn("Zip")).WithReader(p => p.Zip);
            mapper.CustomMapping(new StringColumn("FavoriteColor")).WithReader(p => p.FavoriteColor);
            mapper.CustomMapping(new StringColumn("FavoriteFood)")).WithReader(p => p.FavoriteFood);
            mapper.CustomMapping(new StringColumn("FavoriteSport")).WithReader(p => p.FavoriteSport);
            mapper.CustomMapping(new DateTimeColumn("CreatedOn")).WithReader(p => p.CreatedOn);
            mapper.CustomMapping(new BooleanColumn("IsActive")).WithReader(p => p.IsActive);

            StringReader reader = new StringReader(data);
            var people = mapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
        }

        [Benchmark]
        public void RunFlatFiles_CustomMapping_Unoptimized()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new Person());
            mapper.OptimizeMapping(false);
            mapper.CustomMapping(new StringColumn("FirstName")).WithReader(p => p.FirstName);
            mapper.CustomMapping(new StringColumn("LastName")).WithReader(p => p.LastName);
            mapper.CustomMapping(new Int32Column("Age")).WithReader(p => p.Age);
            mapper.CustomMapping(new StringColumn("Street1")).WithReader(p => p.Street1);
            mapper.CustomMapping(new StringColumn("Street2")).WithReader(p => p.Street2);
            mapper.CustomMapping(new StringColumn("City")).WithReader(p => p.City);
            mapper.CustomMapping(new StringColumn("State")).WithReader(p => p.State);
            mapper.CustomMapping(new StringColumn("Zip")).WithReader(p => p.Zip);
            mapper.CustomMapping(new StringColumn("FavoriteColor")).WithReader(p => p.FavoriteColor);
            mapper.CustomMapping(new StringColumn("FavoriteFood)")).WithReader(p => p.FavoriteFood);
            mapper.CustomMapping(new StringColumn("FavoriteSport")).WithReader(p => p.FavoriteSport);
            mapper.CustomMapping(new DateTimeColumn("CreatedOn")).WithReader(p => p.CreatedOn);
            mapper.CustomMapping(new BooleanColumn("IsActive")).WithReader(p => p.IsActive);

            StringReader reader = new StringReader(data);
            var people = mapper.Read(reader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
        }

        [Benchmark]
        public void RunFlatFiles_DeducedMapper()
        {
            var reader = new StringReader(data);
            var csvReader = SeparatedValueTypeMapper.GetAutoMappedReader<Person>(reader);
            var people = csvReader.ReadAll().ToArray();
        }

        [Benchmark]
        public async Task RunFlatFiles_DeducedMapperAsync()
        {
            var reader = new StringReader(data);
            var csvReader = await SeparatedValueTypeMapper.GetAutoMappedReaderAsync<Person>(reader);
            var people = new List<Person>();
            while (await csvReader.ReadAsync().ConfigureAwait(false))
            {
                people.Add(csvReader.Current);
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
                Person person = new Person
                {
                    FirstName = record[0],
                    LastName = record[1],
                    Age = Int32.Parse(record[2]),
                    Street1 = record[3],
                    Street2 = record[4],
                    City = record[5],
                    State = record[6],
                    Zip = record[7],
                    FavoriteColor = record[8],
                    FavoriteFood = record[9],
                    FavoriteSport = record[10],
                    CreatedOn = DateTime.Parse(record[11]),
                    IsActive = Boolean.Parse(record[12])
                };
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
