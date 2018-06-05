using System;
using System.Globalization;
using System.IO;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class SeparatedValueMultipleSchemaTester
    {
        public SeparatedValueMultipleSchemaTester()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        [Fact]
        public void TestReader_ReadThreeTypes()
        {
            StringWriter stringWriter = new StringWriter();
            var injector = getSchemaInjector();
            var writer = new SeparatedValueWriter(stringWriter, injector);
            writer.Write(new object[] { "First Batch", 2 });
            writer.Write(new object[] { 1, "Bob Smith", new DateTime(2018, 06, 04), 12.34m });
            writer.Write(new object[] { 2, "Jane Doe", new DateTime(2018, 06, 05), 34.56m });
            writer.Write(new object[] { 46.9m, 23.45m, true });
            string output = stringWriter.ToString();
            Assert.Equal(@"First Batch,2
1,Bob Smith,20180604,12.34
2,Jane Doe,20180605,34.56
46.9,23.45,True
", output);

            var stringReader = new StringReader(output);
            var selector = getSchemaSelector();
            var reader = new SeparatedValueReader(stringReader, selector);

            Assert.True(reader.Read(), "The header record could not be read.");            
            var headerValues = reader.GetValues();
            Assert.Equal(2, headerValues.Length);
            Assert.Equal("First Batch", headerValues[0]);
            Assert.Equal(2, headerValues[1]);

            Assert.True(reader.Read(), "The first data record could not be read.");
            var dataValues1 = reader.GetValues();
            Assert.Equal(4, dataValues1.Length);
            Assert.Equal(1, dataValues1[0]);
            Assert.Equal("Bob Smith", dataValues1[1]);
            Assert.Equal(new DateTime(2018, 6, 4), dataValues1[2]);
            Assert.Equal(12.34m, dataValues1[3]);

            Assert.True(reader.Read(), "The second data record could not be read.");
            var dataValues2 = reader.GetValues();
            Assert.Equal(4, dataValues2.Length);
            Assert.Equal(2, dataValues2[0]);
            Assert.Equal("Jane Doe", dataValues2[1]);
            Assert.Equal(new DateTime(2018, 6, 5), dataValues2[2]);
            Assert.Equal(34.56m, dataValues2[3]);

            Assert.True(reader.Read(), "The footer record could not be read.");
            var footerValues = reader.GetValues();
            Assert.Equal(3, footerValues.Length);
            Assert.Equal(46.9m, footerValues[0]);
            Assert.Equal(23.45m, footerValues[1]);
            Assert.IsType<bool>(footerValues[2]);
            Assert.True((bool)footerValues[2]);

            Assert.False(reader.Read());
        }

        private SeparatedValueSchemaInjector getSchemaInjector()
        {
            var injector = new SeparatedValueSchemaInjector();
            injector.When(values => values.Length == 2).Use(getHeaderSchema());
            injector.When(values => values.Length == 3).Use(getFooterSchema());
            injector.WithDefault(getRecordSchema());
            return injector;
        }

        private SeparatedValueSchemaSelector getSchemaSelector()
        {
            var selector = new SeparatedValueSchemaSelector();
            selector.When(values => values.Length == 2).Use(getHeaderSchema());
            selector.When(values => values.Length == 3).Use(getFooterSchema());
            selector.WithDefault(getRecordSchema());
            return selector;
        }

        private SeparatedValueSchema getHeaderSchema()
        {
            var mapper = getHeaderTypeMapper();
            return mapper.GetSchema();
        }

        private SeparatedValueSchema getRecordSchema()
        {
            var mapper = getRecordTypeMapper();
            return mapper.GetSchema();
        }

        private SeparatedValueSchema getFooterSchema()
        {
            var mapper = getFooterTypeMapper();
            return mapper.GetSchema();
        }

        [Fact]
        public void TestTypeMapper_ReadThreeTypes()
        {
            StringWriter stringWriter = new StringWriter();
            var injector = getTypeMapperInjector();
            var writer = injector.GetWriter(stringWriter);
            writer.Write(new HeaderRecord { BatchName = "First Batch", RecordCount = 2 });
            writer.Write(new DataRecord { Id = 1, Name = "Bob Smith", CreatedOn = new DateTime(2018, 06, 04), TotalAmount = 12.34m });
            writer.Write(new DataRecord { Id = 2, Name = "Jane Doe", CreatedOn = new DateTime(2018, 06, 05), TotalAmount = 34.56m });
            writer.Write(new FooterRecord { TotalAmount = 46.9m, AverageAmount = 23.45m, IsCriteriaMet = true });
            string output = stringWriter.ToString();
            Assert.Equal(@"First Batch,2
1,Bob Smith,20180604,12.34
2,Jane Doe,20180605,34.56
46.9,23.45,True
", output);

            var selector = getTypeMapperSelector();
            var stringReader = new StringReader(output);
            var reader = selector.GetReader(stringReader);

            Assert.True(reader.Read(), "The header record could not be read.");
            Assert.IsType<HeaderRecord>(reader.Current);
            Assert.Equal("First Batch", ((HeaderRecord)reader.Current).BatchName);
            Assert.Equal(2, ((HeaderRecord)reader.Current).RecordCount);

            Assert.True(reader.Read(), "The first data record could not be read.");
            Assert.IsType<DataRecord>(reader.Current);
            Assert.Equal(1, ((DataRecord)reader.Current).Id);
            Assert.Equal("Bob Smith", ((DataRecord)reader.Current).Name);
            Assert.Equal(new DateTime(2018, 6, 4), ((DataRecord)reader.Current).CreatedOn);
            Assert.Equal(12.34m, ((DataRecord)reader.Current).TotalAmount);

            Assert.True(reader.Read(), "The second data record could not be read.");
            Assert.IsType<DataRecord>(reader.Current);
            Assert.Equal(2, ((DataRecord)reader.Current).Id);
            Assert.Equal("Jane Doe", ((DataRecord)reader.Current).Name);
            Assert.Equal(new DateTime(2018, 6, 5), ((DataRecord)reader.Current).CreatedOn);
            Assert.Equal(34.56m, ((DataRecord)reader.Current).TotalAmount);

            Assert.True(reader.Read(), "The footer record could not be read.");
            Assert.IsType<FooterRecord>(reader.Current);
            Assert.Equal(46.9m, ((FooterRecord)reader.Current).TotalAmount);
            Assert.Equal(23.45m, ((FooterRecord)reader.Current).AverageAmount);
            Assert.True(((FooterRecord)reader.Current).IsCriteriaMet);

            Assert.False(reader.Read());
        }

        private SeparatedValueTypeMapperSelector getTypeMapperSelector()
        {
            var selector = new SeparatedValueTypeMapperSelector();
            selector.WithDefault(getRecordTypeMapper());
            selector.When(x => x.Length == 2).Use(getHeaderTypeMapper());
            selector.When(x => x.Length == 3).Use(getFooterTypeMapper());
            return selector;
        }

        private SeparatedValueTypeMapperInjector getTypeMapperInjector()
        {
            var selector = new SeparatedValueTypeMapperInjector();
            selector.WithDefault(getRecordTypeMapper());
            selector.When<HeaderRecord>().Use(getHeaderTypeMapper());
            selector.When<FooterRecord>().Use(getFooterTypeMapper());
            return selector;
        }

        private static ISeparatedValueTypeMapper<HeaderRecord> getHeaderTypeMapper()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new HeaderRecord());
            mapper.Property(x => x.BatchName);
            mapper.Property(x => x.RecordCount);
            return mapper;
        }

        private static ISeparatedValueTypeMapper<DataRecord> getRecordTypeMapper()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new DataRecord());
            mapper.Property(x => x.Id);
            mapper.Property(x => x.Name);
            mapper.Property(x => x.CreatedOn).InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(x => x.TotalAmount);
            return mapper;
        }

        private ISeparatedValueTypeMapper<FooterRecord> getFooterTypeMapper()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new FooterRecord());
            mapper.Property(x => x.TotalAmount);
            mapper.Property(x => x.AverageAmount);
            mapper.Property(x => x.IsCriteriaMet);
            return mapper;
        }

        public class HeaderRecord
        {
            public string BatchName { get; set; }

            public int RecordCount { get; set; }
        }

        public class FooterRecord
        {
            public decimal? TotalAmount { get; set; }

            public decimal? AverageAmount { get; set; }

            public bool IsCriteriaMet { get; set; }
        }

        public class DataRecord
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime? CreatedOn { get; set; }

            public decimal TotalAmount { get; set; }
        }
    }
}
