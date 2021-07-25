using System;
using System.Globalization;
using System.IO;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class SeparatedValueMultipleSchemaTester
    {
        public SeparatedValueMultipleSchemaTester()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        [TestMethod]
        public void TestReader_ReadThreeTypes()
        {
            var stringWriter = new StringWriter();
            var injector = GetSchemaInjector();
            var writer = new SeparatedValueWriter(stringWriter, injector);
            writer.Write(new object[] { "First Batch", 2 });
            writer.Write(new object[] { 1, "Bob Smith", new DateTime(2018, 06, 04), 12.34m });
            writer.Write(new object[] { 2, "Jane Doe", new DateTime(2018, 06, 05), 34.56m });
            writer.Write(new object[] { 46.9m, 23.45m, true });
            string output = stringWriter.ToString();
            Assert.AreEqual(@"First Batch,2
1,Bob Smith,20180604,12.34
2,Jane Doe,20180605,34.56
46.9,23.45,True
", output);

            var stringReader = new StringReader(output);
            var selector = GetSchemaSelector();
            var reader = new SeparatedValueReader(stringReader, selector);

            Assert.IsTrue(reader.Read(), "The header record could not be read.");            
            var headerValues = reader.GetValues();
            Assert.AreEqual(2, headerValues.Length);
            Assert.AreEqual("First Batch", headerValues[0]);
            Assert.AreEqual(2, headerValues[1]);

            Assert.IsTrue(reader.Read(), "The first data record could not be read.");
            var dataValues1 = reader.GetValues();
            Assert.AreEqual(4, dataValues1.Length);
            Assert.AreEqual(1, dataValues1[0]);
            Assert.AreEqual("Bob Smith", dataValues1[1]);
            Assert.AreEqual(new DateTime(2018, 6, 4), dataValues1[2]);
            Assert.AreEqual(12.34m, dataValues1[3]);

            Assert.IsTrue(reader.Read(), "The second data record could not be read.");
            var dataValues2 = reader.GetValues();
            Assert.AreEqual(4, dataValues2.Length);
            Assert.AreEqual(2, dataValues2[0]);
            Assert.AreEqual("Jane Doe", dataValues2[1]);
            Assert.AreEqual(new DateTime(2018, 6, 5), dataValues2[2]);
            Assert.AreEqual(34.56m, dataValues2[3]);

            Assert.IsTrue(reader.Read(), "The footer record could not be read.");
            var footerValues = reader.GetValues();
            Assert.AreEqual(3, footerValues.Length);
            Assert.AreEqual(46.9m, footerValues[0]);
            Assert.AreEqual(23.45m, footerValues[1]);
            Assert.AreEqual(true, footerValues[2]);

            Assert.IsFalse(reader.Read());
        }

        private static SeparatedValueSchemaInjector GetSchemaInjector()
        {
            var injector = new SeparatedValueSchemaInjector();
            injector.When(values => values.Length == 2).Use(GetHeaderSchema());
            injector.When(values => values.Length == 3).Use(GetFooterSchema());
            injector.WithDefault(GetRecordSchema());
            return injector;
        }

        private static SeparatedValueSchemaSelector GetSchemaSelector()
        {
            var selector = new SeparatedValueSchemaSelector();
            selector.When(values => values.Length == 2).Use(GetHeaderSchema());
            selector.When(values => values.Length == 3).Use(GetFooterSchema());
            selector.WithDefault(GetRecordSchema());
            return selector;
        }

        private static SeparatedValueSchema GetHeaderSchema()
        {
            var mapper = GetHeaderTypeMapper();
            return mapper.GetSchema();
        }

        private static SeparatedValueSchema GetRecordSchema()
        {
            var mapper = GetRecordTypeMapper();
            return mapper.GetSchema();
        }

        private static SeparatedValueSchema GetFooterSchema()
        {
            var mapper = GetFooterTypeMapper();
            return mapper.GetSchema();
        }

        [TestMethod]
        public void TestTypeMapper_ReadThreeTypes()
        {
            var stringWriter = new StringWriter();
            var injector = GetTypeMapperInjector();
            var writer = injector.GetWriter(stringWriter);
            writer.Write(new HeaderRecord { BatchName = "First Batch", RecordCount = 2 });
            writer.Write(new DataRecord { Id = 1, Name = "Bob Smith", CreatedOn = new DateTime(2018, 06, 04), TotalAmount = 12.34m });
            writer.Write(new DataRecord { Id = 2, Name = "Jane Doe", CreatedOn = new DateTime(2018, 06, 05), TotalAmount = 34.56m });
            writer.Write(new FooterRecord { TotalAmount = 46.9m, AverageAmount = 23.45m, IsCriteriaMet = true });
            string output = stringWriter.ToString();
            Assert.AreEqual(@"First Batch,2
1,Bob Smith,20180604,12.34
2,Jane Doe,20180605,34.56
46.9,23.45,True
", output);

            var selector = GetTypeMapperSelector();
            var stringReader = new StringReader(output);
            var reader = selector.GetReader(stringReader);

            Assert.IsTrue(reader.Read(), "The header record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(HeaderRecord));
            Assert.AreEqual("First Batch", ((HeaderRecord)reader.Current).BatchName);
            Assert.AreEqual(2, ((HeaderRecord)reader.Current).RecordCount);

            Assert.IsTrue(reader.Read(), "The first data record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(DataRecord));
            Assert.AreEqual(1, ((DataRecord)reader.Current).Id);
            Assert.AreEqual("Bob Smith", ((DataRecord)reader.Current).Name);
            Assert.AreEqual(new DateTime(2018, 6, 4), ((DataRecord)reader.Current).CreatedOn);
            Assert.AreEqual(12.34m, ((DataRecord)reader.Current).TotalAmount);

            Assert.IsTrue(reader.Read(), "The second data record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(DataRecord));
            Assert.AreEqual(2, ((DataRecord)reader.Current).Id);
            Assert.AreEqual("Jane Doe", ((DataRecord)reader.Current).Name);
            Assert.AreEqual(new DateTime(2018, 6, 5), ((DataRecord)reader.Current).CreatedOn);
            Assert.AreEqual(34.56m, ((DataRecord)reader.Current).TotalAmount);

            Assert.IsTrue(reader.Read(), "The footer record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(FooterRecord));
            Assert.AreEqual(46.9m, ((FooterRecord)reader.Current).TotalAmount);
            Assert.AreEqual(23.45m, ((FooterRecord)reader.Current).AverageAmount);
            Assert.IsTrue(((FooterRecord)reader.Current).IsCriteriaMet);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public void TestTypeMapper_ReadThreeTypes_WithMetadataRecord()
        {
            var stringWriter = new StringWriter();
            var injector = GetTypeMapperInjector();
            var writer = injector.GetWriter(stringWriter);
            writer.Write(new HeaderRecord { BatchName = "First Batch", RecordCount = 2 });
            writer.Write(new DataRecord { Id = 1, Name = "Bob Smith", CreatedOn = new DateTime(2018, 06, 04), TotalAmount = 12.34m });
            writer.Write(new DataRecord { Id = 2, Name = "Jane Doe", CreatedOn = new DateTime(2018, 06, 05), TotalAmount = 34.56m });
            writer.Write(new FooterRecord { TotalAmount = 46.9m, AverageAmount = 23.45m, IsCriteriaMet = true });
            string output = stringWriter.ToString();
            Assert.AreEqual(@"First Batch,2
1,Bob Smith,20180604,12.34
2,Jane Doe,20180605,34.56
46.9,23.45,True
", output);

            var selector = GetTypeMapperSelector(true);
            var stringReader = new StringReader(output);
            var reader = selector.GetReader(stringReader);

            Assert.IsTrue(reader.Read(), "The header record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(HeaderRecord));
            Assert.AreEqual("First Batch", ((HeaderRecord)reader.Current).BatchName);
            Assert.AreEqual(2, ((HeaderRecord)reader.Current).RecordCount);

            Assert.IsTrue(reader.Read(), "The first data record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(DataRecord));
            Assert.AreEqual(1, ((DataRecord)reader.Current).Id);
            Assert.AreEqual("Bob Smith", ((DataRecord)reader.Current).Name);
            Assert.AreEqual(new DateTime(2018, 6, 4), ((DataRecord)reader.Current).CreatedOn);
            Assert.AreEqual(12.34m, ((DataRecord)reader.Current).TotalAmount);

            Assert.IsTrue(reader.Read(), "The second data record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(DataRecord));
            Assert.AreEqual(2, ((DataRecord)reader.Current).Id);
            Assert.AreEqual("Jane Doe", ((DataRecord)reader.Current).Name);
            Assert.AreEqual(new DateTime(2018, 6, 5), ((DataRecord)reader.Current).CreatedOn);
            Assert.AreEqual(34.56m, ((DataRecord)reader.Current).TotalAmount);

            Assert.IsTrue(reader.Read(), "The footer record could not be read.");
            Assert.IsInstanceOfType(reader.Current, typeof(FooterRecord));
            Assert.AreEqual(46.9m, ((FooterRecord)reader.Current).TotalAmount);
            Assert.AreEqual(23.45m, ((FooterRecord)reader.Current).AverageAmount);
            Assert.IsTrue(((FooterRecord)reader.Current).IsCriteriaMet);

            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        [ExpectedException(typeof(RecordProcessingException))]
        public void TestReader_UnknownType()
        {
            var stringReader = new StringReader("What's this weird thing?");
            var selector = GetSchemaSelector();
            var reader = new SeparatedValueReader(stringReader, selector);

            reader.Read();
        }

        [TestMethod]
        public void TestReader_UnknownType_IgnoreUnknown_SkipsRecord()
        {
            var stringReader = new StringReader("What's this weird thing?");
            var selector = GetSchemaSelector();
            var reader = new SeparatedValueReader(stringReader, selector);
            reader.RecordError += (o, e) => e.IsHandled = true;
            Assert.IsFalse(reader.Read());
        }

        private static SeparatedValueTypeMapperSelector GetTypeMapperSelector(bool hasMetadata = false)
        {
            var selector = new SeparatedValueTypeMapperSelector();
            selector.WithDefault(GetRecordTypeMapper(hasMetadata));
            selector.When(x => x.Length == 2).Use(GetHeaderTypeMapper());
            selector.When(x => x.Length == 3).Use(GetFooterTypeMapper());
            return selector;
        }

        private static SeparatedValueTypeMapperInjector GetTypeMapperInjector()
        {
            var selector = new SeparatedValueTypeMapperInjector();
            selector.WithDefault(GetRecordTypeMapper());
            selector.When<HeaderRecord>().Use(GetHeaderTypeMapper());
            selector.When<FooterRecord>().Use(GetFooterTypeMapper());
            return selector;
        }

        private static ISeparatedValueTypeMapper<HeaderRecord> GetHeaderTypeMapper()
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new HeaderRecord());
            mapper.Property(x => x.BatchName);
            mapper.Property(x => x.RecordCount);
            return mapper;
        }

        private static ISeparatedValueTypeMapper<DataRecord> GetRecordTypeMapper(bool hasMetadata = false)
        {
            var mapper = SeparatedValueTypeMapper.Define(() => new DataRecord());
            mapper.Property(x => x.Id);
            mapper.Property(x => x.Name);
            mapper.Property(x => x.CreatedOn).InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(x => x.TotalAmount);
            if (hasMetadata)
            {
                mapper.CustomMapping(new RecordNumberColumn("row_num")
                {
                    IncludeSchema = true,
                    IncludeSkippedRecords = true
                }).WithReader(r => r.RecordNumber);
            }
            return mapper;
        }

        private static ISeparatedValueTypeMapper<FooterRecord> GetFooterTypeMapper()
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

            public int? RecordNumber { get; set; }
        }
    }
}
