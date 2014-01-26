using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Threading;
    using FlatFiles.TypeMapping;

    /// <summary>
    /// Tests the SeparatedValueParser class.
    /// </summary>
    [TestClass]
    public class SeparatedValueReaderTester
    {
        /// <summary>
        /// Setup for tests.
        /// </summary>
        [TestInitialize]
        public void TestSetup()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_TextNull_Throws()
        {
            string text = null;
            new SeparatedValueReader(text);
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Schema_TextNull_Throws()
        {
            string text = null;
            SeparatedValueSchema schema = new SeparatedValueSchema();
            new SeparatedValueReader(text, schema);
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_TextNull_Throws()
        {
            string text = null;
            SeparatedValueOptions options = new SeparatedValueOptions();
            new SeparatedValueReader(text, options);
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_Schema_TextNull_Throws()
        {
            string text = null;
            SeparatedValueSchema schema = new SeparatedValueSchema();
            SeparatedValueOptions options = new SeparatedValueOptions();
            new SeparatedValueReader(text, schema, options);
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_SchemaNull_Throws()
        {
            string text = String.Empty;
            SeparatedValueSchema schema = null;
            new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_SchemaNull_Throws()
        {
            string text = String.Empty;
            SeparatedValueSchema schema = null;
            SeparatedValueOptions options = new SeparatedValueOptions();
            new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
        }

        /// <summary>
        /// If we try to pass null options to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_OptionsNull_Throws()
        {
            string text = "";
            SeparatedValueOptions options = null;
            new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), options);
        }

        /// <summary>
        /// If we try to pass null options to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Schema_OptionsNull_Throws()
        {
            string text = "";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            SeparatedValueOptions options = null;
            new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
        }

        /// <summary>
        /// If we pass a single record, Read should return true once.
        /// </summary>
        [TestMethod]
        public void TestRead_SingleRecord_ReturnsTrueOnce()
        {
            const string text = "a,b,c";
            SeparatedValueReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)));
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            string[] expected = new string[] { "a", "b", "c" };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
            canRead = parser.Read();
            Assert.IsFalse(canRead, "No more records should have been read.");
        }

        /// <summary>
        /// If we try to get the values before calling Read, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRead_GetValuesWithoutReading_Throws()
        {
            string text = "a,b,c";
            SeparatedValueReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)));
            parser.GetValues();
        }

        /// <summary>
        /// We should be able to read the same values as many times as we want.
        /// </summary>
        [TestMethod]
        public void TestRead_MultipleCallsToValues_ReturnsSameValues()
        {
            string text = "a,b,c";
            SeparatedValueReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)));
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            string[] expected = new string[] { "a", "b", "c" };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
            actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The same values were not returned multiple times.");
        }

        /// <summary>
        /// If Read returns false, requesting the Values will cause an exception to be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestRead_ValuesAfterEndOfFile_Throws()
        {
            string text = "a,b,c";
            SeparatedValueReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)));
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            canRead = parser.Read();
            Assert.IsFalse(canRead, "We should have reached the end of the file.");
            parser.GetValues();
        }

        /// <summary>
        /// If we pass a string with CP1252 characters, it should reflect does characters when returning
        /// </summary>
        [TestMethod]
        public void TestRead_RecordWithCP1252Characters_ReturnsCorrectCharacters()
        {
            //---- Arrange -----------------------------------------------------
            // Need to convert the string to target encoding because otherwise a string declared in VS will always be encoded as UTF-8
            var text = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1252), Encoding.UTF8.GetBytes(@"123;Müller;1/17/2014"));
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id")).AddColumn(new StringColumn("name")).AddColumn(new DateTimeColumn("created"));
            var options = new SeparatedValueOptions 
            { 
                IsFirstRecordSchema = false, 
                Separator = ";" ,
                Encoding = Encoding.GetEncoding(1252)
            };

            var testee = new SeparatedValueReader(new MemoryStream(text), schema, options);

            //---- Act ---------------------------------------------------------
            var result = testee.Read();

            //---- Assert ------------------------------------------------------
            Assert.IsTrue(result, "Could not read the record.");
            object[] expected = { 123, "Müller", new DateTime(2014, 1, 17) };
            object[] actual = testee.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
        }

        /// <summary>
        /// If we pass a string with CP1251 characters, it should reflect does characters when returning
        /// </summary>
        [TestMethod]
        public void TestRead_RecordWithCP1251Characters_ReturnsCorrectCharacters()
        {
            //---- Arrange -----------------------------------------------------
            // Need to convert the string to target encoding because otherwise a string declared in VS will always be encoded as UTF-8
            var text = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1251), Encoding.UTF8.GetBytes(@"123;Лучиано;1/17/2014"));
            var schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id")).AddColumn(new StringColumn("name")).AddColumn(new DateTimeColumn("created"));
            var options = new SeparatedValueOptions 
            { 
                IsFirstRecordSchema = false, 
                Separator = ";", 
                Encoding = Encoding.GetEncoding(1251) 
            };

            var testee = new SeparatedValueReader(new MemoryStream(text), schema, options);

            //---- Act ---------------------------------------------------------
            var result = testee.Read();

            //---- Assert ------------------------------------------------------
            Assert.IsTrue(result, "Could not read the record.");
            object[] expected = { 123, "Лучиано", new DateTime(2014, 1, 17) };
            object[] actual = testee.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
        }

        /// <summary>
        /// If we do not explicitly say that the first record is the schema, we cannot retrieve it later.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestGetSchema_NotExtracted_Throws()
        {
            string text = "a,b,c";
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = false };
            IReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), options);
            parser.GetSchema();
        }

        /// <summary>
        /// If we say that the first record is the schema, we can retrieve it later on.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_Extracted_ReturnsColumnNames()
        {
            string text = "a,b,c";
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            IReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), options);
            ISchema schema = parser.GetSchema();
            Assert.IsTrue(schema.ColumnDefinitions.All(d => d is StringColumn), "Not all of the columns were treated as strings.");
            string[] actual = schema.ColumnDefinitions.Select(d => d.ColumnName).ToArray();
            string[] expected = new string[] { "a", "b", "c" };
            CollectionAssert.AreEqual(expected, actual, "The schema was not extracted as expected.");
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
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            IReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
            ISchema actual = parser.GetSchema();
            Assert.AreSame(schema, actual, "The schema was passed did not take priority.");
            Assert.IsFalse(parser.Read(), "The schema record was not skipped.");
        }

        private class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Created { get; set; }
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
            SeparatedValueReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            Assert.IsTrue(parser.Read(), "The first record was skipped.");
            object[] actual = parser.GetValues();
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            CollectionAssert.AreEqual(expected, actual, "The values were not parsed as expected.");
        }

        /// <summary>
        /// If we provide a schema, the records in the file must have a value for each column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FlatFileException))]
        public void TestGetSchema_SchemaProvided_WrongNumberOfColumns_Throws()
        {
            const string text = @"123,Bob";
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new Int32Column("id"))
                  .AddColumn(new StringColumn("name"))
                  .AddColumn(new DateTimeColumn("created"));
            SeparatedValueReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            parser.Read();
        }

        /// <summary>
        /// If the first record is the schema, the records in the file must have the
        /// same number of columns.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FlatFileException))]
        public void TestGetSchema_FirstRecordSchema_WrongNumberOfColumns_Throws()
        {
            const string text = @"id,name,created
123,Bob,1/19/2013,Hello";
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            SeparatedValueReader parser = new SeparatedValueReader(new MemoryStream(Encoding.Default.GetBytes(text)), options);
            parser.Read();
        }

        /// <summary>
        /// If a record has a blank value at the end of a line, null should be
        /// returned for the last column.
        /// </summary>
        [TestMethod]
        public void TestGetValues_BlankTrailingSection_ReturnsNull()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
                SeparatedValueSchema schema = new SeparatedValueSchema();
                schema.AddColumn(new Int32Column("id"))
                    .AddColumn(new StringColumn("name"))
                    .AddColumn(new DateTimeColumn("created") { InputFormat = "M/d/yyyy", OutputFormat = "M/d/yyyy" })
                    .AddColumn(new StringColumn("trailing"));
                object[] sources = new object[] { 123, "Bob", new DateTime(2013, 1, 19), "" };
                using (SeparatedValueWriter builder = new SeparatedValueWriter(stream, schema, options))
                {
                    builder.Write(sources);
                }
                stream.Position = 0;

                SeparatedValueReader parser = new SeparatedValueReader(stream, schema, options);
                Assert.IsTrue(parser.Read(), "No records were found.");
                object[] values = parser.GetValues();
                Assert.AreEqual(schema.ColumnDefinitions.Count, values.Length, "The wrong number of values were read.");
                Assert.AreEqual(sources[0], values[0], "The first column was not parsed correctly.");
                Assert.AreEqual(sources[1], values[1], "The second column was not parsed correctly.");
                Assert.AreEqual(sources[2], values[2], "The third column was not parsed correctly.");
                Assert.AreEqual(null, values[3], "The forth column was not interpreted as null.");
                Assert.IsFalse(parser.Read(), "Too many records were found.");
            }
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
                using (SeparatedValueWriter builder = new SeparatedValueWriter(stream, schema, options))
                {
                    builder.Write(sources);
                }
                stream.Position = 0;

                SeparatedValueReader parser = new SeparatedValueReader(stream, schema, options);
                Assert.IsTrue(parser.Read(), "No records were found.");
                object[] values = parser.GetValues();
                Assert.AreEqual(schema.ColumnDefinitions.Count, values.Length, "The wrong number of values were read.");
                Assert.AreEqual(sources[0], values[0], "The first column was not parsed correctly.");
                Assert.AreEqual(sources[1], values[1], "The second column was not parsed correctly.");
                Assert.AreEqual(null, values[2], "The third column was not interpreted as null.");
                Assert.AreEqual(sources[3], values[3], "The forth column was not parsed correctly.");
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
                using (SeparatedValueWriter builder = new SeparatedValueWriter(stream, schema, options))
                {
                    builder.Write(sources);
                }
                stream.Position = 0;

                SeparatedValueReader parser = new SeparatedValueReader(stream, schema, options);
                Assert.IsTrue(parser.Read(), "No records were found.");
                object[] values = parser.GetValues();
                Assert.AreEqual(schema.ColumnDefinitions.Count, values.Length, "The wrong number of values were read.");
                Assert.AreEqual(null, values[0], "The first column was not interpreted as null.");
                Assert.AreEqual(sources[1], values[1], "The second column was not parsed correctly.");
                Assert.AreEqual(sources[2], values[2], "The third column was not parsed correctly.");
                Assert.AreEqual(sources[3], values[3], "The forth column was not parsed correctly.");
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

            var testee = new SeparatedValueReader(new MemoryStream(Encoding.GetEncoding(1252).GetBytes(text)), options);

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

            using (MemoryStream stream = new MemoryStream())
            {
                var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19) };
                var options = new SeparatedValueOptions() { IsFirstRecordSchema = true, Separator = "\t" };
                
                mapper.Write(stream, options, new Person[] { bob });

                stream.Position = 0;  // go back to the beginning of the stream

                var people = mapper.Read(stream, options);
                Assert.AreEqual(1, people.Count(), "The wrong number of people were returned.");
                var person = people.SingleOrDefault();
                Assert.AreEqual(bob.Id, person.Id, "The ID value was not persisted.");
                Assert.AreEqual(bob.Name, person.Name, "The Name value was not persisted.");
                Assert.AreEqual(bob.Created, person.Created, "The Created value was not persisted.");
            }
        }
    }
}
