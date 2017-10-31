using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using FlatFiles.TypeMapping;

namespace FlatFiles.Benchmark
{
    public class AsyncVsSyncTest
    {
        [Benchmark]
        public string SyncTest()
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

            StringWriter textWriter = new StringWriter();
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(directory, "TestFiles", "SampleData.csv");
            using (var stream = File.OpenRead(path))
            using (var textReader = new StreamReader(stream))
            {
                var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
                var reader = mapper.GetReader(textReader, options);
                var writer = mapper.GetWriter(textWriter, options);
                for (int i = 0; i < 100_000 && reader.Read(); ++i)
                {
                    writer.Write(reader.Current);
                }
            }
            return textWriter.ToString();
        }

        [Benchmark]
        public async Task<string> AsyncTest()
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

            StringWriter textWriter = new StringWriter();
            string directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string path = Path.Combine(directory, "TestFiles", "SampleData.csv");
            using (var stream = File.OpenRead(path))
            using (var textReader = new StreamReader(stream))
            {
                var options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
                var reader = mapper.GetReader(textReader, options);
                var writer = mapper.GetWriter(textWriter, options);
                while (await reader.ReadAsync())
                {
                    await writer.WriteAsync(reader.Current);
                }
            }
            return textWriter.ToString();
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
