using System;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    using System.Globalization;
    using System.Threading;
    using FlatFiles.TypeMapping;

    /// <summary>
    /// Tests the FixedLengthParserTester class.
    /// </summary>
    [TestClass]
    public class FixedLengthReaderTester
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
            FixedLengthSchema schema = new FixedLengthSchema();
            new FixedLengthReader(text, schema);
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_TextNull_Throws()
        {
            Stream stream = null;
            FixedLengthSchema schema = new FixedLengthSchema();
            FixedLengthOptions options = new FixedLengthOptions();
            new FixedLengthReader(stream, schema, options);
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_SchemaNull_Throws()
        {
            string text = String.Empty;
            FixedLengthSchema schema = null;
            new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_Options_SchemaNull_Throws()
        {
            string text = String.Empty;
            FixedLengthSchema schema = null;
            FixedLengthOptions options = new FixedLengthOptions();
            new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
        }

        /// <summary>
        /// If we try to pass null options to the parser, an exception should be thrown.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCtor_OptionsNull_Throws()
        {
            string text = String.Empty;
            FixedLengthSchema schema = new FixedLengthSchema();
            FixedLengthOptions options = null;
            new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);
        }

        /// <summary>
        /// If we pass a single record, Read should return true once.
        /// </summary>
        [TestMethod]
        public void TestRead_SingleRecord_ReturnsTrueOnce()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));
            FixedLengthReader parser = new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            Assert.IsTrue(parser.Read(), "Could not read the record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
            Assert.IsFalse(parser.Read(), "No more records should have been read.");
        }

        /// <summary>
        /// If we skip a bad record, it should not result in a parsing error.
        /// </summary>
        [TestMethod]
        public void TestRead_SkipRecord_NoParsingError()
        {
            const string text = "a b c";
            var data = new MemoryStream(Encoding.Default.GetBytes(text));
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("A"), 8);
            schema.AddColumn(new DateTimeColumn("B"), 23);
            schema.AddColumn(new GuidColumn("C"), 2);
            FixedLengthReader parser = new FixedLengthReader(data, schema);
            bool canRead = parser.Skip();
            Assert.IsTrue(canRead, "Could not skip the record.");
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
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));
            FixedLengthReader parser = new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            parser.GetValues();
        }

        /// <summary>
        /// We should be able to read the same values as many times as we want.
        /// </summary>
        [TestMethod]
        public void TestRead_MultipleCallsToValues_ReturnsSameValues()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));
            FixedLengthReader parser = new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            bool canRead = parser.Read();
            Assert.IsTrue(canRead, "Could not read the record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
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
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));
            FixedLengthReader parser = new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            Assert.IsTrue(parser.Read(), "Could not read the record.");
            Assert.IsFalse(parser.Read(), "We should have reached the end of the file.");
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
            var text = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1252), Encoding.UTF8.GetBytes(@"       123                   Müller 1/17/2014"));
            var schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));
            var options = new FixedLengthOptions() { Encoding = Encoding.GetEncoding(1252) };

            var testee = new FixedLengthReader(new MemoryStream(text), schema, options);
            
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
            var text = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1251), Encoding.UTF8.GetBytes(@"       123                  Лучиано 1/17/2014"));
            var schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));
            var options = new FixedLengthOptions() { Encoding = Encoding.GetEncoding(1251) };

            var testee = new FixedLengthReader(new MemoryStream(text), schema, options);

            //---- Act ---------------------------------------------------------
            var result = testee.Read();

            //---- Assert ------------------------------------------------------
            Assert.IsTrue(result, "Could not read the record.");
            object[] expected = { 123, "Лучиано", new DateTime(2014, 1, 17) };
            object[] actual = testee.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The wrong values were parsed.");
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values
        /// and can be retrieved.
        /// </summary>
        [TestMethod]
        public void TestGetSchema_SchemaProvided_ParsesValues()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));
            IReader parser = new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            ISchema actual = parser.GetSchema();
            Assert.AreSame(schema, actual, "The underlying schema was not returned.");
        }

        /// <summary>
        /// The records in the file must have a value for each column.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(FlatFileException))]
        public void TestGetSchema_SchemaProvided_WrongNumberOfColumns_Throws()
        {
            const string text = @"       123                      Bob";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                  .AddColumn(new StringColumn("name"), new Window(25))
                  .AddColumn(new DateTimeColumn("created"), new Window(10));
            FixedLengthReader parser = new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema);
            parser.Read();
        }

        /// <summary>
        /// If we specify a custom record separator, it should be used
        /// to split records in the file.
        /// </summary>
        [TestMethod]
        public void TestGetValues_CustomRecordSeparator_SplitsFile()
        {             
            const string text = "       123                      Bob 1/19/2013BOOM       234                      Sam12/20/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                  .AddColumn(new StringColumn("name"), new Window(25))
                  .AddColumn(new DateTimeColumn("created"), new Window(10));
            FixedLengthOptions options = new FixedLengthOptions() { RecordSeparator = "BOOM" };
            FixedLengthReader parser = new FixedLengthReader(new MemoryStream(Encoding.Default.GetBytes(text)), schema, options);

            Assert.IsTrue(parser.Read(), "Could not read the first record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            object[] actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The values for the first record were wrong.");

            Assert.IsTrue(parser.Read(), "Could not read the second record.");
            expected = new object[] { 234, "Sam", new DateTime(2013, 12, 20) };
            actual = parser.GetValues();
            CollectionAssert.AreEqual(expected, actual, "The values for the second record were wrong.");
        }

        /// <summary>
        /// If we specify a custom fill character, it should be used to buffer fields in the file.
        /// </summary>
        [TestMethod]
        public void TestGetValues_CustomFillCharacter_TrimsFill()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10) { Alignment = FixedAlignment.LeftAligned })
                  .AddColumn(new StringColumn("name"), new Window(25) { Alignment = FixedAlignment.LeftAligned })
                  .AddColumn(new DateTimeColumn("created") { InputFormat = "M/d/yyyy", OutputFormat = "M/d/yyyy" }, new Window(10) { Alignment = FixedAlignment.LeftAligned });
            FixedLengthOptions options = new FixedLengthOptions() { FillCharacter = '@' };
            object[] sources = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            using (MemoryStream stream = new MemoryStream())
            {
                using (FixedLengthWriter builder = new FixedLengthWriter(stream, schema, options))
                {
                    builder.Write(sources);
                }
                stream.Position = 0;

                FixedLengthReader parser = new FixedLengthReader(stream, schema, options);

                Assert.IsTrue(parser.Read(), "Could not read the first record.");
                object[] actual = parser.GetValues();
                CollectionAssert.AreEqual(sources, actual, "The values for the first record were wrong.");
            }
        }

        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [TestMethod]
        public void TestTypeMapper_Roundtrip()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            using (MemoryStream stream = new MemoryStream())
            {
                var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19) };
                var options = new FixedLengthOptions() { FillCharacter = '@' };

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

        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [TestMethod]
        public void TestTypeMapper_RoundTrip_SkipHeaderRow()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            using (MemoryStream stream = new MemoryStream())
            {
                var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19) };
                var options = new FixedLengthOptions() { IsFirstRecordHeader = true, FillCharacter = '@' };

                var writer = new StreamWriter(stream);
                writer.WriteLine("{0,25}{1,100}{2,8}", "id", "name", "created");
                writer.Flush();

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

        /// <summary>
        /// We should be able to write and read values using a type mapper with a null value.
        /// </summary>
        [TestMethod]
        public void TestTypeMapper_RoundtripWithNull()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            using (MemoryStream stream = new MemoryStream())
            {
                var bob = new Person() { Id = 123, Name = null, Created = new DateTime(2013, 1, 19) };
                var options = new FixedLengthOptions() { FillCharacter = '@' };

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

        private class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Created { get; set; }
        }
    }
}
