using System;
using System.IO;
using System.Linq;
using System.Globalization;
using System.Threading;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the SeparatedValueParser class.
    /// </summary>
    public class SeparatedValueReaderTester
    {
        /// <summary>
        /// Setup for tests.
        /// </summary>
        public SeparatedValueReaderTester()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [Fact]
        public void TestCtor_NullWriter_NoSchema_Throws()
        {
            TextReader reader = null;
            Assert.Throws<ArgumentNullException>(() => new SeparatedValueReader(reader));
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [Fact]
        public void TestCtor_NullWriter_WithSchema_Throws()
        {
            TextReader reader = null;
            SeparatedValueSchema schema = new SeparatedValueSchema();
            Assert.Throws<ArgumentNullException>(() => new SeparatedValueReader(reader, schema));
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [Fact]
        public void TestCtor_SchemaNull_Throws()
        {
            TextReader reader = new StringReader(String.Empty);
            SeparatedValueSchema schema = null;
            Assert.Throws<ArgumentNullException>(() => new SeparatedValueReader(reader, schema));
        }

        /// <summary>
        /// If we pass a single record, Read should return true once.
        /// </summary>
        [Fact]
        public void TestRead_SingleRecord_ReturnsTrueOnce()
        {
            const string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader);
            bool canRead = parser.Read();
            Assert.True(canRead, "Could not read the record.");
            object[] expected = new object[] { "a", "b", "c" };
            object[] actual = parser.GetValues();
            Assert.Equal(expected, actual);
            canRead = parser.Read();
            Assert.False(canRead, "No more records should have been read.");
        }

        [Fact]
        public void TestRead_InvalidConversion_Throws()
        {
            const string text = "a";
            StringReader stringReader = new StringReader(text);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("First"));
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema);
            Assert.Throws<FlatFileException>(() => parser.Read());
        }

        /// <summary>
        /// If we skip a bad record, it should not result in a parsing error.
        /// </summary>
        [Fact]
        public void TestRead_SkipRecord_NoParsingError()
        {
            const string text = "a,b,c";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("A"));
            schema.AddColumn(new DateTimeColumn("B"));
            schema.AddColumn(new GuidColumn("C"));

            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema);
            bool canRead = parser.Skip();
            Assert.True(canRead, "Could not skip the record.");
            canRead = parser.Read();
            Assert.False(canRead, "No more records should have been read.");
        }

        /// <summary>
        /// If we try to get the values before calling Read, an exception should be thrown.
        /// </summary>
        [Fact]
        public void TestRead_GetValuesWithoutReading_Throws()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader);
            Assert.Throws<InvalidOperationException>(() => parser.GetValues());
        }

        /// <summary>
        /// We should be able to read the same values as many times as we want.
        /// </summary>
        [Fact]
        public void TestRead_MultipleCallsToValues_ReturnsSameValues()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader);
            bool canRead = parser.Read();
            Assert.True(canRead, "Could not read the record.");
            object[] expected = new object[] { "a", "b", "c" };
            object[] actual = parser.GetValues();
            Assert.Equal(expected, actual);
            actual = parser.GetValues();
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If Read returns false, requesting the Values will cause an exception to be thrown.
        /// </summary>
        [Fact]
        public void TestRead_ValuesAfterEndOfFile_Throws()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader);
            bool canRead = parser.Read();
            Assert.True(canRead, "Could not read the record.");
            canRead = parser.Read();
            Assert.False(canRead, "We should have reached the end of the file.");
            Assert.Throws<InvalidOperationException>(() => parser.GetValues());
        }

        /// <summary>
        /// If a record contains a quote, it should still parse correctly.
        /// </summary>
        [Fact]
        public void TestRead_EmbeddedQuote_ParsesCorrectly()
        {
            var text = @"123;Todd's Bait Shop;1/17/2014";
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"));
            schema.AddColumn(new StringColumn("name"));
            schema.AddColumn(new DateTimeColumn("created"));
            var options = new SeparatedValueOptions
            {
                IsFirstRecordSchema = false,
                Separator = ";"
            };

            StringReader stringReader = new StringReader(text);
            var reader = new SeparatedValueReader(stringReader, schema, options);

            var result = reader.Read();

            Assert.True(result, "Could not read the record.");
            object[] expected = { 123, "Todd's Bait Shop", new DateTime(2014, 1, 17) };
            object[] actual = reader.GetValues();
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If we do not explicitly say that the first record is the schema, we cannot retrieve it later.
        /// </summary>
        [Fact]
        public void TestGetSchema_NotExtracted_Throws()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            IReader parser = new SeparatedValueReader(stringReader, options);
            Assert.Throws<InvalidOperationException>(() => parser.GetSchema());
        }

        /// <summary>
        /// If we say that the first record is the schema, we can retrieve it later on.
        /// </summary>
        [Fact]
        public void TestGetSchema_Extracted_ReturnsColumnNames()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            IReader parser = new SeparatedValueReader(stringReader, options);
            ISchema schema = parser.GetSchema();
            Assert.True(schema.ColumnDefinitions.All(d => d is StringColumn), "Not all of the columns were treated as strings.");
            string[] actual = schema.ColumnDefinitions.Select(d => d.ColumnName).ToArray();
            string[] expected = new string[] { "a", "b", "c" };
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If we provide a schema and say the first record is the schema, our schema takes priority
        /// and we throw away the first record.
        /// </summary>
        [Fact]
        public void TestGetSchema_SchemaProvided_FirstRecordSchema_SkipsFirstRecord()
        {
            const string text = @"id,name,created";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            StringReader stringReader = new StringReader(text);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            IReader parser = new SeparatedValueReader(stringReader, schema, options);
            ISchema actual = parser.GetSchema();
            Assert.Same(schema, actual);
            Assert.False(parser.Read(), "The schema record was not skipped.");
        }

        /// <summary>
        /// If we provide a record filter, those records should be skipped while processing the file.
        /// </summary>
        [Fact]
        public void TestRead_WithSeparatedRecordFilter_SkipsRecordsMatchingCriteria()
        {
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));
            SeparatedValueOptions options = new SeparatedValueOptions()
            {
                PartitionedRecordFilter = (record) => record.Length < 3
            };

            const string text = @"123,Bob Smith,4/21/2017
This is not a real record
234,Jay Smith,5/21/2017";
            StringReader stringReader = new StringReader(text);
            IReader parser = new SeparatedValueReader(stringReader, schema, options);

            Assert.True(parser.Read(), "Could not read the first record.");
            object[] actual1 = parser.GetValues();
            Assert.Equal(new object[] { 123, "Bob Smith", new DateTime(2017, 04, 21) }, actual1);

            Assert.True(parser.Read(), "Could not read the second record.");
            object[] actual2 = parser.GetValues();
            Assert.Equal(new object[] { 234, "Jay Smith", new DateTime(2017, 05, 21) }, actual2);

            Assert.False(parser.Read(), "There should not be any more records.");
        }

        private class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Created { get; set; }

            public int? ParentId { get; set; }
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values.
        /// </summary>
        [Fact]
        public void TestGetSchema_SchemaProvided_ParsesValues()
        {
            const string text = @"123,Bob,1/19/2013";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema);
            Assert.True(parser.Read(), "The first record was skipped.");
            object[] actual = parser.GetValues();
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values, also when columns are quoted.
        /// </summary>
        [Fact]
        public void TestGetSchema_SchemaProvided_ParsesValues_Quoted()
        {
            const string text = "123,\"Bob\",1/19/2013";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema);
            Assert.True(parser.Read(), "The first record was skipped.");
            object[] actual = parser.GetValues();
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If we provide a schema, the records in the file must have a value for each column.
        /// </summary>
        [Fact]
        public void TestGetSchema_SchemaProvided_WrongNumberOfColumns_Throws()
        {
            const string text = @"123,Bob";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema);
            Assert.Throws<FlatFileException>(() => parser.Read());
        }

        /// <summary>
        /// If the first record is the schema, the records in the file must have the
        /// same number of columns.
        /// </summary>
        [Fact]
        public void TestGetSchema_FirstRecordSchema_TooFewColumns_Throws()
        {
            const string text = @"id,name,created
123,Bob";
            StringReader stringReader = new StringReader(text);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, options);
            Assert.Throws<FlatFileException>(() => parser.Read());
        }

        /// <summary>
        /// If the first record is the schema, the records in the file can have more columns that are ignored.
        /// </summary>
        [Fact]
        public void TestGetSchema_FirstRecordSchema_TooManyColumns_IgnoresTrailing()
        {
            const string text = @"id,name,created
123,Bob,1/19/2013,Hello";
            StringReader stringReader = new StringReader(text);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, options);
            Assert.True(parser.Read(), "The record could not be read.");
            Assert.Equal(parser.GetSchema().ColumnDefinitions.Count, parser.GetValues().Length);
            ;
        }

        /// <summary>
        /// If a record has a blank value at the end of a line, null should be
        /// returned for the last column.
        /// </summary>
        [Fact]
        public void TestGetValues_BlankTrailingSection_ReturnsNull()
        {
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                .AddColumn(new StringColumn("name"))
                .AddColumn(new DateTimeColumn("created") { InputFormat = "M/d/yyyy", OutputFormat = "M/d/yyyy" })
                .AddColumn(new StringColumn("trailing"));
            object[] sources = new object[] { 123, "Bob", new DateTime(2013, 1, 19), "" };

            StringWriter stringWriter = new StringWriter();
            SeparatedValueWriter builder = new SeparatedValueWriter(stringWriter, schema, options);
            builder.Write(sources);

            StringReader stringReader = new StringReader(stringWriter.ToString());
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema, options);
            Assert.True(parser.Read(), "No records were found.");
            object[] values = parser.GetValues();
            Assert.Equal(schema.ColumnDefinitions.Count, values.Length);
            Assert.Equal(sources[0], values[0]);
            Assert.Equal(sources[1], values[1]);
            Assert.Equal(sources[2], values[2]);
            Assert.Equal(null, values[3]);
            Assert.False(parser.Read(), "Too many records were found.");
        }

        /// <summary>
        /// If a record has a blank value in the middle of a line, null should be
        /// returned for that column.
        /// </summary>
        [Fact]
        public void TestGetValues_BlankMiddleSection_ReturnsNull()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
                SeparatedValueSchema schema = new SeparatedValueSchema();
                schema.AddColumn(new Int32Column("id"))
                    .AddColumn(new StringColumn("name"))
                    .AddColumn(new StringColumn("middle"))
                    .AddColumn(new DateTimeColumn("created") { InputFormat = "M/d/yyyy", OutputFormat = "M/d/yyyy" });
                object[] sources = new object[] { 123, "Bob", "", new DateTime(2013, 1, 19) };

                StringWriter stringWriter = new StringWriter();
                SeparatedValueWriter builder = new SeparatedValueWriter(stringWriter, schema, options);
                builder.Write(sources);

                StringReader stringReader = new StringReader(stringWriter.ToString());
                SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema, options);
                Assert.True(parser.Read(), "No records were found.");
                object[] values = parser.GetValues();
                Assert.Equal(schema.ColumnDefinitions.Count, values.Length);
                Assert.Equal(sources[0], values[0]);
                Assert.Equal(sources[1], values[1]);
                Assert.Equal(null, values[2]);
                Assert.Equal(sources[3], values[3]);
                Assert.False(parser.Read(), "Too many records were found.");
            }
        }

        /// <summary>
        /// If a record has a blank value in the front of a line, null should be
        /// returned for that column.
        /// </summary>
        [Fact]
        public void TestGetValues_BlankLeadingSection_ReturnsNull()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
                SeparatedValueSchema schema = new SeparatedValueSchema();
                schema.AddColumn(new StringColumn("leading"))
                    .AddColumn(new Int32Column("id"))
                    .AddColumn(new StringColumn("name"))
                    .AddColumn(new DateTimeColumn("created") { InputFormat = "M/d/yyyy", OutputFormat = "M/d/yyyy" });
                object[] sources = new object[] { "", 123, "Bob", new DateTime(2013, 1, 19) };

                StringWriter stringWriter = new StringWriter();
                SeparatedValueWriter builder = new SeparatedValueWriter(stringWriter, schema, options);
                builder.Write(sources);

                StringReader stringReader = new StringReader(stringWriter.ToString());
                SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema, options);
                Assert.True(parser.Read(), "No records were found.");
                object[] values = parser.GetValues();
                Assert.Equal(schema.ColumnDefinitions.Count, values.Length);
                Assert.Equal(null, values[0]);
                Assert.Equal(sources[1], values[1]);
                Assert.Equal(sources[2], values[2]);
                Assert.Equal(sources[3], values[3]);
                Assert.False(parser.Read(), "Too many records were found.");
            }
        }

        [Fact]
        public void TestRead_ZeroLengthColumn()
        {
            //---- Arrange -----------------------------------------------------
            var text = "104\t20\t1000\t00\tLausanne\tLausanne\tVD\t2\t\t0\t130\t5586\t19880301";
            var options = new SeparatedValueOptions { IsFirstRecordSchema = false, Separator = "\t" };
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("OnrpId"))
                .AddColumn(new Int32Column("Type"))
                .AddColumn(new Int32Column("ZipCode"))
                .AddColumn(new StringColumn("ZipCodeAddOn"))
                .AddColumn(new StringColumn("TownShortName"))
                .AddColumn(new StringColumn("TownOfficialName"))
                .AddColumn(new StringColumn("CantonAbbreviation"))
                .AddColumn(new Int16Column("MainLanguageCode"))
                .AddColumn(new Int16Column("OtherLanguageCode"))
                .AddColumn(new ByteColumn("HasSortfileData"))
                .AddColumn(new Int32Column("LetterServiceOnrpId"))
                .AddColumn(new Int32Column("MunicipalityId"))
                .AddColumn(new StringColumn("ValidFrom"));

            StringReader stringReader = new StringReader(text);
            var testee = new SeparatedValueReader(stringReader, options);

            //---- Act ---------------------------------------------------------
            var result = testee.Read();

            //---- Assert ------------------------------------------------------
            Assert.True(result);
            Assert.Equal(schema.ColumnDefinitions.Count, testee.GetValues().Count());
        }

        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [Fact]
        public void TestTypeMapper_Roundtrip()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>();
            mapper.Property(p => p.Id).ColumnName("id");
            mapper.Property(p => p.Name).ColumnName("name");
            mapper.Property(p => p.Created).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");
            mapper.Property(p => p.ParentId).ColumnName("parent_id");

            var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19), ParentId = null };
            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true, Separator = "\t" };

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Person[] { bob }, options);

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader, options).ToArray();
            Assert.Equal(1, people.Count());
            var person = people.SingleOrDefault();
            Assert.Equal(bob.Id, person.Id);
            Assert.Equal(bob.Name, person.Name);
            Assert.Equal(bob.Created, person.Created);
            Assert.Equal(bob.ParentId, person.ParentId);
        }

        /// <summary>
        /// We should be able to write and read values using a type mapper with a null value.
        /// </summary>
        [Fact]
        public void TestTypeMapper_RoundtripWithNull()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>();
            mapper.Property(p => p.Id).ColumnName("id");
            mapper.Property(p => p.Name).ColumnName("name");
            mapper.Property(p => p.Created).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            var bob = new Person() { Id = 123, Name = null, Created = new DateTime(2013, 1, 19) };
            var options = new SeparatedValueOptions() { IsFirstRecordSchema = true, Separator = "\t" };

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Person[] { bob }, options);

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader, options).ToArray();
            Assert.Equal(1, people.Count());
            var person = people.SingleOrDefault();
            Assert.Equal(bob.Id, person.Id);
            Assert.Equal(bob.Name, person.Name);
            Assert.Equal(bob.Created, person.Created);
        }

        /// <summary>
        /// We should be able to write and read values using a type mapper with a null value.
        /// </summary>
        [Fact]
        public void TestTypeMapper_IgnoredColumns_RoundTrips()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>();
            mapper.Property(p => p.Id).ColumnName("id");
            mapper.Ignored();
            mapper.Ignored();
            mapper.Property(p => p.Name).ColumnName("name");
            mapper.Ignored();
            mapper.Property(p => p.Created).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            var bob = new Person() { Id = 123, Name = "Bob Smith", Created = new DateTime(2013, 1, 19) };

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Person[] { bob });

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.Equal(1, people.Count());
            var person = people.SingleOrDefault();
            Assert.Equal(bob.Id, person.Id);
            Assert.Equal(bob.Name, person.Name);
            Assert.Equal(bob.Created, person.Created);
        }

        /// <summary>
        /// Test to make sure the sample CSV from http://www.creativyst.com/Doc/Articles/CSV/CSV01.htm works.
        /// </summary>
        [Fact]
        public void TestReader_creativyst_example()
        {
            const string text = @"John,Doe,120 jefferson st.,Riverside, NJ, 08075
Jack,McGinnis,220 hobo Av.,Phila, PA,09119
""John """"Da Man"""""",Repici,120 Jefferson St.,Riverside, NJ,08075
Stephen,Tyler,""7452 Terrace """"At the Plaza"""" road"",SomeTown,SD, 91234
,Blankman,,SomeTown, SD, 00298
""Joan """"the bone"""", Anne"",Jet,""9th, at Terrace plc"",Desert City, CO,00123
";
            StringReader stringReader = new StringReader(text);
            SeparatedValueReader reader = new SeparatedValueReader(stringReader);
            Assert.True(reader.Read(), "Could not read the first record.");
            assertValues(reader, "John", "Doe", "120 jefferson st.", "Riverside", "NJ", "08075");
            Assert.True(reader.Read(), "Could not read the second record.");
            assertValues(reader, "Jack", "McGinnis", "220 hobo Av.", "Phila", "PA", "09119");
            Assert.True(reader.Read(), "Could not read the third record.");
            assertValues(reader, "John \"Da Man\"", "Repici", "120 Jefferson St.", "Riverside", "NJ", "08075");
            Assert.True(reader.Read(), "Could not read the fourth record.");
            assertValues(reader, "Stephen", "Tyler", "7452 Terrace \"At the Plaza\" road", "SomeTown", "SD", "91234");
            Assert.True(reader.Read(), "Could not read the fifth record.");
            assertValues(reader, "", "Blankman","", "SomeTown", "SD", "00298");
            Assert.True(reader.Read(), "Could not read the sixth record.");
            assertValues(reader, "Joan \"the bone\", Anne", "Jet", "9th, at Terrace plc", "Desert City", "CO", "00123");
            Assert.False(reader.Read(), "Read too many records.");
        }

        private static void assertValues(SeparatedValueReader reader, string firstName, string lastName, string street, string city, string state, string zip)
        {
            object[] values = reader.GetValues();
            Assert.Equal(6, values.Length);
            Assert.Equal(firstName, values[0]);
            Assert.Equal(lastName, values[1]);
            Assert.Equal(street, values[2]);
            Assert.Equal(city, values[3]);
            Assert.Equal(state, values[4]);
            Assert.Equal(zip, values[5]);
        }
    }
}
