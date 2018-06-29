using System;
using System.IO;
using System.Linq;
using System.Globalization;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the SeparatedValueParser class.
    /// </summary>
    [TestClass]
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
        [TestMethod]
        public void TestCtor_NullWriter_NoSchema_Throws()
        {
            TextReader reader = null;
            Assert.ThrowsException<ArgumentNullException>(() => new SeparatedValueReader(reader));
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        public void TestCtor_NullWriter_WithSchema_Throws()
        {
            TextReader reader = null;
            SeparatedValueSchema schema = new SeparatedValueSchema();
            Assert.ThrowsException<ArgumentNullException>(() => new SeparatedValueReader(reader, schema));
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        public void TestCtor_SchemaNull_Throws()
        {
            TextReader reader = new StringReader(String.Empty);
            SeparatedValueSchema schema = null;
            Assert.ThrowsException<ArgumentNullException>(() => new SeparatedValueReader(reader, schema));
        }

        /// <summary>
        /// If we pass a single record, Read should return true once.
        /// </summary>
        [TestMethod]
        public void TestRead_SingleRecord_ReturnsTrueOnce()
        {
            const string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader);
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            object[] expected = new object[] { "a", "b", "c" };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual);
            canRead = parser.Read();
            Assert.IsFalse(canRead, "No more records should have been read.");
        }

        [TestMethod]
        public void TestRead_InvalidConversion_Throws()
        {
            const string text = "a";
            StringReader stringReader = new StringReader(text);
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("First"));
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema);
            Assert.ThrowsException<RecordProcessingException>(() => parser.Read());
        }

        /// <summary>
        /// If we skip a bad record, it should not result in a parsing error.
        /// </summary>
        [TestMethod]
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
            Assert.IsTrue(canRead, "Could not skip the record.");
            canRead = parser.Read();
            Assert.IsFalse(canRead, "No more records should have been read.");
        }

        /// <summary>
        /// If we try to get the values before calling Read, an exception should be thrown.
        /// </summary>
        [TestMethod]
        public void TestRead_GetValuesWithoutReading_Throws()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader);
            Assert.ThrowsException<InvalidOperationException>(() => parser.GetValues());
        }

        /// <summary>
        /// We should be able to read the same values as many times as we want.
        /// </summary>
        [TestMethod]
        public void TestRead_MultipleCallsToValues_ReturnsSameValues()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader);
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            object[] expected = new object[] { "a", "b", "c" };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual);
            actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If Read returns false, requesting the Values will cause an exception to be thrown.
        /// </summary>
        [TestMethod]
        public void TestRead_ValuesAfterEndOfFile_Throws()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader);
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            canRead = parser.Read();
            Assert.IsFalse(canRead, "We should have reached the end of the file.");
            Assert.ThrowsException<InvalidOperationException>(() => parser.GetValues());
        }

        /// <summary>
        /// If a record contains a quote, it should still parse correctly.
        /// </summary>
        [TestMethod]
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

            Assert.IsTrue(result, "Could not read the record.");
            object[] expected = { 123, "Todd's Bait Shop", new DateTime(2014, 1, 17) };
            object[] actual = reader.GetValues();
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If we do not explicitly say that the first record is the schema, we cannot retrieve it later.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_NotExtracted_Throws()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            IReader parser = new SeparatedValueReader(stringReader, options);
            Assert.ThrowsException<InvalidOperationException>(() => parser.GetSchema());
        }

        /// <summary>
        /// If we say that the first record is the schema, we can retrieve it later on.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_Extracted_ReturnsColumnNames()
        {
            string text = "a,b,c";
            StringReader stringReader = new StringReader(text);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            IReader parser = new SeparatedValueReader(stringReader, options);
            ISchema schema = parser.GetSchema();
            Assert.IsTrue(schema.ColumnDefinitions.All(d => d is StringColumn), "Not all of the columns were treated as strings.");
            string[] actual = schema.ColumnDefinitions.Select(d => d.ColumnName).ToArray();
            string[] expected = new string[] { "a", "b", "c" };
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If we provide a schema and say the first record is the schema, our schema takes priority
        /// and we throw away the first record.
        /// </summary>
        [TestMethod]
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
            Assert.AreSame(schema, actual);
            Assert.IsFalse(parser.Read(), "The schema record was not skipped.");
        }

        /// <summary>
        /// If we provide a record filter, those records should be skipped while processing the file.
        /// </summary>
        [TestMethod]
        public void TestRead_WithSeparatedRecordFilter_SkipsRecordsMatchingCriteria()
        {
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            const string text = @"123,Bob Smith,4/21/2017
This is not a real record
234,Jay Smith,5/21/2017";
            StringReader stringReader = new StringReader(text);
            var parser = new SeparatedValueReader(stringReader, schema);
            parser.RecordRead += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length < 3;
            };

            Assert.IsTrue(parser.Read(), "Could not read the first record.");
            object[] actual1 = parser.GetValues();
            CollectionAssert.AreEqual(new object[] { 123, "Bob Smith", new DateTime(2017, 04, 21) }, actual1);

            Assert.IsTrue(parser.Read(), "Could not read the second record.");
            object[] actual2 = parser.GetValues();
            CollectionAssert.AreEqual(new object[] { 234, "Jay Smith", new DateTime(2017, 05, 21) }, actual2);

            Assert.IsFalse(parser.Read(), "There should not be any more records.");
        }

        internal class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Created { get; set; }

            public int? ParentId { get; set; }

            public bool? IsActive { get; set; }
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_ParsesValues()
        {
            const string text = @"123,Bob,1/19/2013";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema);
            Assert.IsTrue(parser.Read(), "The first record was skipped.");
            object[] actual = parser.GetValues();
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values, also when columns are quoted.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_ParsesValues_Quoted()
        {
            const string text = "123,\"Bob\",1/19/2013";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema);
            Assert.IsTrue(parser.Read(), "The first record was skipped.");
            object[] actual = parser.GetValues();
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            CollectionAssert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If we provide a schema, the records in the file must have a value for each column.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_WrongNumberOfColumns_Throws()
        {
            const string text = @"123,Bob";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));

            StringReader stringReader = new StringReader(text);
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, schema);
            Assert.ThrowsException<RecordProcessingException>(() => parser.Read());
        }

        /// <summary>
        /// If the first record is the schema, the records in the file must have the
        /// same number of columns.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_FirstRecordSchema_TooFewColumns_Throws()
        {
            const string text = @"id,name,created
123,Bob";
            StringReader stringReader = new StringReader(text);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, options);
            Assert.ThrowsException<RecordProcessingException>(() => parser.Read());
        }

        /// <summary>
        /// If the first record is the schema, the records in the file can have more columns that are ignored.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_FirstRecordSchema_TooManyColumns_IgnoresTrailing()
        {
            const string text = @"id,name,created
123,Bob,1/19/2013,Hello";
            StringReader stringReader = new StringReader(text);
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            SeparatedValueReader parser = new SeparatedValueReader(stringReader, options);
            Assert.IsTrue(parser.Read(), "The record could not be read.");
            Assert.AreEqual(parser.GetSchema().ColumnDefinitions.Count, parser.GetValues().Length);
            ;
        }

        /// <summary>
        /// If a record has a blank value at the end of a line, null should be
        /// returned for the last column.
        /// </summary>
        [TestMethod]
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
            Assert.IsTrue(parser.Read(), "No records were found.");
            object[] values = parser.GetValues();
            Assert.AreEqual(schema.ColumnDefinitions.Count, values.Length);
            Assert.AreEqual(sources[0], values[0]);
            Assert.AreEqual(sources[1], values[1]);
            Assert.AreEqual(sources[2], values[2]);
            Assert.IsNull(values[3]);
            Assert.IsFalse(parser.Read(), "Too many records were found.");
        }

        /// <summary>
        /// If a record has a blank value in the middle of a line, null should be
        /// returned for that column.
        /// </summary>
        [TestMethod]
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
                Assert.IsTrue(parser.Read(), "No records were found.");
                object[] values = parser.GetValues();
                Assert.AreEqual(schema.ColumnDefinitions.Count, values.Length);
                Assert.AreEqual(sources[0], values[0]);
                Assert.AreEqual(sources[1], values[1]);
                Assert.IsNull(values[2]);
                Assert.AreEqual(sources[3], values[3]);
                Assert.IsFalse(parser.Read(), "Too many records were found.");
            }
        }

        /// <summary>
        /// If a record has a blank value in the front of a line, null should be
        /// returned for that column.
        /// </summary>
        [TestMethod]
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
                Assert.IsTrue(parser.Read(), "No records were found.");
                object[] values = parser.GetValues();
                Assert.AreEqual(schema.ColumnDefinitions.Count, values.Length);
                Assert.IsNull(values[0]);
                Assert.AreEqual(sources[1], values[1]);
                Assert.AreEqual(sources[2], values[2]);
                Assert.AreEqual(sources[3], values[3]);
                Assert.IsFalse(parser.Read(), "Too many records were found.");
            }
        }

        [TestMethod]
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
            Assert.IsTrue(result);
            Assert.AreEqual(schema.ColumnDefinitions.Count, testee.GetValues().Count());
        }

        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [TestMethod]
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
            Assert.AreEqual(1, people.Length);
            var person = people.SingleOrDefault();
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
            Assert.AreEqual(bob.ParentId, person.ParentId);
        }

        /// <summary>
        /// We should be able to write and read values using a type mapper with a null value.
        /// </summary>
        [TestMethod]
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
            Assert.AreEqual(1, people.Length);
            var person = people.SingleOrDefault();
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
        }

        /// <summary>
        /// We should be able to write and read values using a type mapper with a null value.
        /// </summary>
        [TestMethod]
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
            Assert.AreEqual(1, people.Length);
            var person = people.SingleOrDefault();
            Assert.AreEqual(bob.Id, person.Id);
            Assert.AreEqual(bob.Name, person.Name);
            Assert.AreEqual(bob.Created, person.Created);
        }

        /// <summary>
        /// Test to make sure the sample CSV from http://www.creativyst.com/Doc/Articles/CSV/CSV01.htm works.
        /// </summary>
        [TestMethod]
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
            Assert.IsTrue(reader.Read(), "Could not read the first record.");
            assertValues(reader, "John", "Doe", "120 jefferson st.", "Riverside", "NJ", "08075");
            Assert.IsTrue(reader.Read(), "Could not read the second record.");
            assertValues(reader, "Jack", "McGinnis", "220 hobo Av.", "Phila", "PA", "09119");
            Assert.IsTrue(reader.Read(), "Could not read the third record.");
            assertValues(reader, "John \"Da Man\"", "Repici", "120 Jefferson St.", "Riverside", "NJ", "08075");
            Assert.IsTrue(reader.Read(), "Could not read the fourth record.");
            assertValues(reader, "Stephen", "Tyler", "7452 Terrace \"At the Plaza\" road", "SomeTown", "SD", "91234");
            Assert.IsTrue(reader.Read(), "Could not read the fifth record.");
            assertValues(reader, "", "Blankman","", "SomeTown", "SD", "00298");
            Assert.IsTrue(reader.Read(), "Could not read the sixth record.");
            assertValues(reader, "Joan \"the bone\", Anne", "Jet", "9th, at Terrace plc", "Desert City", "CO", "00123");
            Assert.IsFalse(reader.Read(), "Read too many records.");
        }

        private static void assertValues(SeparatedValueReader reader, string firstName, string lastName, string street, string city, string state, string zip)
        {
            object[] values = reader.GetValues();
            Assert.AreEqual(6, values.Length);
            Assert.AreEqual(firstName, values[0]);
            Assert.AreEqual(lastName, values[1]);
            Assert.AreEqual(street, values[2]);
            Assert.AreEqual(city, values[3]);
            Assert.AreEqual(state, values[4]);
            Assert.AreEqual(zip, values[5]);
        }

        [TestMethod]
        public void TestTypeMapper_NullableBoolean_RoundTripsNull()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>();
            mapper.Property(x => x.IsActive).ColumnName("is_active");

            Person person = new Person() { IsActive = null };

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Person[] { person });

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, people.Length);
            var first = people.SingleOrDefault();
            Assert.IsNull(first.IsActive);
        }

        [TestMethod]
        public void TestTypeMapper_NullableBoolean_RoundTripsFalse()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>();
            mapper.Property(x => x.IsActive).ColumnName("is_active");

            Person person = new Person() { IsActive = false };

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Person[] { person });

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, people.Length);
            var first = people.SingleOrDefault();
            Assert.AreNotEqual(true, first.IsActive);
        }

        [TestMethod]
        public void TestTypeMapper_NullableBoolean_RoundTripsTrue()
        {
            var mapper = SeparatedValueTypeMapper.Define<Person>();
            mapper.Property(x => x.IsActive).ColumnName("is_active");

            Person person = new Person() { IsActive = true };

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Person[] { person });

            StringReader stringReader = new StringReader(stringWriter.ToString());
            var people = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, people.Length);
            var first = people.SingleOrDefault();
            Assert.AreEqual(true, first.IsActive);
        }

        [TestMethod]
        public void TestTypeMapper_BadRecordColumn_SkipError()
        {
            const string data = @"1,2017-06-11,John Smith
2,2017-12-32,Tom Stallon
3,2017-08-13,Walter Kay";
            var mapper = SeparatedValueTypeMapper.Define<Person>();
            mapper.Property(x => x.Id);
            mapper.Property(x => x.Created);
            mapper.Property(x => x.Name);

            StringReader stringReader = new StringReader(data);
            List<int> errorRecords = new List<int>();
            var reader = mapper.GetReader(stringReader);
            reader.Error += (sender, e) =>
            {
                errorRecords.Add(e.RecordContext.PhysicalRecordNumber);
                e.IsHandled = true;
            };
            var people = reader.ReadAll().ToArray();
            Assert.AreEqual(2, people.Count());
            Assert.AreEqual(1, errorRecords.Count);
            Assert.AreEqual(2, errorRecords[0]);
        }

        [TestMethod]
        public void TestReader_NullToDateTime_ProvidesUsefulErrorMessage()
        {
            const string rawData = "Hello,,Goodbye";
            StringReader reader = new StringReader(rawData);

            var mapper = SeparatedValueTypeMapper.Define<ClassWithDate>();
            mapper.Ignored();
            mapper.Property(x => x.DateTime);
            mapper.Ignored();

            try
            {
                var records = mapper.Read(reader).ToArray();
                Assert.IsTrue(false); // The line above should always fail.
            }
            catch (FlatFileException)
            {                
            }
        }

        internal class ClassWithDate
        {
            public DateTime DateTime { get; set; }
        }
    }
}
