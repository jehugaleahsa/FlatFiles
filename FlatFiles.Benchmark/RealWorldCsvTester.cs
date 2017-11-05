using System.IO;
using System.Linq;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using CsvHelper.Configuration;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class RealWorldCsvTester
    {
        [Benchmark]
        public void RunCsvHelper()
        {
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(directory, "TestFiles", "SampleData.csv");
            using (var stream = File.OpenRead(path))
            using (var textReader = new StreamReader(stream))
            {
                var configuration = new Configuration()
                {
                    HasHeaderRecord = true,
                    HeaderValidated = (isValid, names, index, context) => { }
                };
                var map = configuration.AutoMap<SampleData>();
                map.Map(x => x.YearStart).Name("YearStart").Index(0);
                map.Map(x => x.YearEnd).Name("YearEnd").Index(1);
                map.Map(x => x.LocationAbbreviation).Name("LocationAbbr").Index(2);
                map.Map(x => x.LocationDescription).Name("LocationDesc").Index(3);
                map.Map(x => x.DataSource).Name("DataSource").Index(4);
                map.Map(x => x.Topic).Name("Topic").Index(5);
                map.Map(x => x.Question).Name("Question").Index(6);
                map.Map(x => x.Response).Name("Response").Index(7);
                map.Map(x => x.DataValueUnit).Name("DataValueUnit").Index(8);
                map.Map(x => x.DataValueType).Name("DataValueType").Index(9);
                map.Map(x => x.DataValue).Name("DataValue").Index(10);
                map.Map(x => x.AlternativeDataValue).Name("DataValueAlt").Index(11);
                map.Map(x => x.DataValueFootnoteSymbol).Name("DataValueFootnoteSymbol").Index(12);
                map.Map(x => x.DataValueFootnote).Name("DatavalueFootnote").Index(13);
                map.Map(x => x.LowConfidenceLimit).Name("LowConfidenceLimit").Index(14);
                map.Map(x => x.HighConfidenceLimit).Name("HighConfidenceLimit").Index(15);
                map.Map(x => x.StratificationCategory1).Name("StratificationCategory1").Index(16);
                map.Map(x => x.Stratification1).Name("Stratification1").Index(17);
                map.Map(x => x.StratificationCategory2).Name("StratificationCategory2").Index(18);
                map.Map(x => x.Stratification2).Name("Stratification2").Index(19);
                map.Map(x => x.StratificationCategory3).Name("StratificationCategory3").Index(20);
                map.Map(x => x.Stratification3).Name("Stratification3").Index(21);
                map.Map(x => x.GeoLocation).Name("GeoLocation").Index(22);
                map.Map(x => x.ResponseId).Name("ResponseID").Index(23);
                map.Map(x => x.LocationId).Name("LocationID").Index(24);
                map.Map(x => x.TopicId).Name("TopicID").Index(25);
                map.Map(x => x.QuestionId).Name("QuestionID").Index(26);
                map.Map(x => x.DataValueTypeId).Name("DataValueTypeID").Index(27);
                map.Map(x => x.StratificationCategoryId1).Name("StratificationCategoryID1").Index(28);
                map.Map(x => x.StratificationId1).Name("StratificationID1").Index(29);
                map.Map(x => x.StratificationCategoryId2).Name("StratificationCategoryID2").Index(30);
                map.Map(x => x.StratificationId2).Name("StratificationID2").Index(31);
                map.Map(x => x.StratificationCategoryId3).Name("StratificationCategoryID3").Index(32);
                map.Map(x => x.StratificationId3).Name("StratificationID3").Index(33);
                configuration.RegisterClassMap(map);
                var csvReader = new CsvHelper.CsvReader(textReader, configuration);
                csvReader.Read();
                csvReader.ReadHeader();
                var people = csvReader.GetRecords<SampleData>().ToArray();
            }
        }

        [Benchmark]
        public void RunFlatFiles()
        {
            var mapper = SeparatedValueTypeMapper.Define<SampleData>();
            mapper.Property(x => x.YearStart).ColumnName("YearStart");
            mapper.Property(x => x.YearEnd).ColumnName("YearEnd");
            mapper.Property(x => x.LocationAbbreviation).ColumnName("LocationAbbr");
            mapper.Property(x => x.LocationDescription).ColumnName("LocationDesc");
            mapper.Property(x => x.DataSource).ColumnName("DataSource");
            mapper.Property(x => x.Topic).ColumnName("Topic");
            mapper.Property(x => x.Question).ColumnName("Question");
            mapper.Property(x => x.Response).ColumnName("Response");
            mapper.Property(x => x.DataValueUnit).ColumnName("DataValueUnit");
            mapper.Property(x => x.DataValueType).ColumnName("DataValueType");
            mapper.Property(x => x.DataValue).ColumnName("DataValue");
            mapper.Property(x => x.AlternativeDataValue).ColumnName("DataValueAlt");
            mapper.Property(x => x.DataValueFootnoteSymbol).ColumnName("DataValueFootnoteSymbol");
            mapper.Property(x => x.DataValueFootnote).ColumnName("DatavalueFootnote");
            mapper.Property(x => x.LowConfidenceLimit).ColumnName("LowConfidenceLimit");
            mapper.Property(x => x.HighConfidenceLimit).ColumnName("HighConfidenceLimit");
            mapper.Property(x => x.StratificationCategory1).ColumnName("StratificationCategory1");
            mapper.Property(x => x.Stratification1).ColumnName("Stratification1");
            mapper.Property(x => x.StratificationCategory2).ColumnName("StratificationCategory2");
            mapper.Property(x => x.Stratification2).ColumnName("Stratification2");
            mapper.Property(x => x.StratificationCategory3).ColumnName("StratificationCategory3");
            mapper.Property(x => x.Stratification3).ColumnName("Stratification3");
            mapper.Property(x => x.GeoLocation).ColumnName("GeoLocation");
            mapper.Property(x => x.ResponseId).ColumnName("ResponseID");
            mapper.Property(x => x.LocationId).ColumnName("LocationID");
            mapper.Property(x => x.TopicId).ColumnName("TopicID");
            mapper.Property(x => x.QuestionId).ColumnName("QuestionID");
            mapper.Property(x => x.DataValueTypeId).ColumnName("DataValueTypeID");
            mapper.Property(x => x.StratificationCategoryId1).ColumnName("StratificationCategoryID1");
            mapper.Property(x => x.StratificationId1).ColumnName("StratificationID1");
            mapper.Property(x => x.StratificationCategoryId2).ColumnName("StratificationCategoryID2");
            mapper.Property(x => x.StratificationId2).ColumnName("StratificationID2");
            mapper.Property(x => x.StratificationCategoryId3).ColumnName("StratificationCategoryID3");
            mapper.Property(x => x.StratificationId3).ColumnName("StratificationID3");

            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(directory, "TestFiles", "SampleData.csv");
            using (var stream = File.OpenRead(path))
            using (var textReader = new StreamReader(stream))
            {
                var people = mapper.Read(textReader, new SeparatedValueOptions() { IsFirstRecordSchema = true }).ToArray();
            }
        }

        private class SampleData
        {
            public int YearStart { get; set; }

            public int YearEnd { get; set; }

            public string LocationAbbreviation { get; set; }

            public string LocationDescription { get; set; }

            public string DataSource { get; set; }

            public string Topic { get; set; }

            public string Question { get; set; }

            public string Response { get; set; }

            public string DataValueUnit { get; set; }

            public string DataValueType { get; set; }

            public string DataValue { get; set; }

            public decimal? AlternativeDataValue { get; set; }

            public string DataValueFootnoteSymbol { get; set; }

            public string DataValueFootnote { get; set; }

            public decimal? LowConfidenceLimit { get; set; }

            public decimal? HighConfidenceLimit { get; set; }

            public string StratificationCategory1 { get; set; }

            public string Stratification1 { get; set; }

            public string StratificationCategory2 { get; set; }

            public string Stratification2 { get; set; }

            public string StratificationCategory3 { get; set; }

            public string Stratification3 { get; set; }

            public string GeoLocation { get; set; }

            public string ResponseId { get; set; }

            public string LocationId { get; set; }

            public string TopicId { get; set; }

            public string QuestionId { get; set; }

            public string DataValueTypeId { get; set; }

            public string StratificationCategoryId1 { get; set; }

            public string StratificationId1 { get; set; }

            public string StratificationCategoryId2 { get; set; }

            public string StratificationId2 { get; set; }

            public string StratificationCategoryId3 { get; set; }

            public string StratificationId3 { get; set; }
        }
    }
}
