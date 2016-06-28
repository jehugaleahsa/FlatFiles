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
    /// Tests the FixedLengthParserTester class.
    /// </summary>
    public class FixedLengthReaderTester
    {
        public FixedLengthReaderTester()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// If we try to pass null text to the parser, an exception should be thrown.
        /// </summary>
        [Fact]
        public void TestCtor_Options_TextNull_Throws()
        {
            TextReader reader = null;
            FixedLengthSchema schema = new FixedLengthSchema();
            FixedLengthOptions options = new FixedLengthOptions();
            Assert.Throws<ArgumentNullException>(() => new FixedLengthReader(reader, schema, options));
        }

        /// <summary>
        /// If we trying to pass a null schema, an exception should be thrown.
        /// </summary>
        [Fact]
        public void TestCtor_SchemaNull_Throws()
        {
            StringReader reader = new StringReader(String.Empty);
            FixedLengthSchema schema = null;
            Assert.Throws<ArgumentNullException>(() => new FixedLengthReader(reader, schema));
        }

        /// <summary>
        /// If we pass a single record, Read should return true once.
        /// </summary>
        [Fact]
        public void TestRead_SingleRecord_ReturnsTrueOnce()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));

            StringReader stringReader = new StringReader(text);
            FixedLengthReader parser = new FixedLengthReader(stringReader, schema);
            Assert.True(parser.Read(), "Could not read the record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            object[] actual = parser.GetValues();
            Assert.Equal(expected, actual);
            Assert.False(parser.Read(), "No more records should have been read.");
        }

        /// <summary>
        /// If we skip a bad record, it should not result in a parsing error.
        /// </summary>
        [Fact]
        public void TestRead_SkipRecord_NoParsingError()
        {
            const string text = "a b c";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("A"), 8);
            schema.AddColumn(new DateTimeColumn("B"), 23);
            schema.AddColumn(new GuidColumn("C"), 2);

            StringReader stringReader = new StringReader(text);
            FixedLengthReader parser = new FixedLengthReader(stringReader, schema);
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
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));

            StringReader stringReader = new StringReader(text);
            FixedLengthReader parser = new FixedLengthReader(stringReader, schema);
            Assert.Throws<InvalidOperationException>(() => parser.GetValues());
        }

        /// <summary>
        /// We should be able to read the same values as many times as we want.
        /// </summary>
        [Fact]
        public void TestRead_MultipleCallsToValues_ReturnsSameValues()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));

            StringReader stringReader = new StringReader(text);
            FixedLengthReader parser = new FixedLengthReader(stringReader, schema);
            bool canRead = parser.Read();
            Assert.True(canRead, "Could not read the record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
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
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));

            StringReader stringReader = new StringReader(text);
            FixedLengthReader parser = new FixedLengthReader(stringReader, schema);
            Assert.True(parser.Read(), "Could not read the record.");
            Assert.False(parser.Read(), "We should have reached the end of the file.");
            Assert.Throws<InvalidOperationException>(() => parser.GetValues());
        }

        /// <summary>
        /// If we provide a schema, it will be used to parse the values
        /// and can be retrieved.
        /// </summary>
        [Fact]
        public void TestGetSchema_SchemaProvided_ParsesValues()
        {
            const string text = @"       123                      Bob 1/19/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                .AddColumn(new StringColumn("name"), new Window(25))
                .AddColumn(new DateTimeColumn("created"), new Window(10));

            StringReader stringReader = new StringReader(text);
            IReader parser = new FixedLengthReader(stringReader, schema);
            ISchema actual = parser.GetSchema();
            Assert.Same(schema, actual);
        }

        /// <summary>
        /// The records in the file must have a value for each column.
        /// </summary>
        [Fact]
        public void TestGetSchema_SchemaProvided_WrongNumberOfColumns_Throws()
        {
            const string text = @"       123                      Bob";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                  .AddColumn(new StringColumn("name"), new Window(25))
                  .AddColumn(new DateTimeColumn("created"), new Window(10));

            StringReader stringReader = new StringReader(text);
            FixedLengthReader parser = new FixedLengthReader(stringReader, schema);
            Assert.Throws<FlatFileException>(() => parser.Read());
        }

        /// <summary>
        /// If we specify a custom record separator, it should be used
        /// to split records in the file.
        /// </summary>
        [Fact]
        public void TestGetValues_CustomRecordSeparator_SplitsFile()
        {             
            const string text = "       123                      Bob 1/19/2013BOOM       234                      Sam12/20/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                  .AddColumn(new StringColumn("name"), new Window(25))
                  .AddColumn(new DateTimeColumn("created"), new Window(10));
            FixedLengthOptions options = new FixedLengthOptions() { RecordSeparator = "BOOM" };

            StringReader stringReader = new StringReader(text);
            FixedLengthReader parser = new FixedLengthReader(stringReader, schema, options);

            Assert.True(parser.Read(), "Could not read the first record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            object[] actual = parser.GetValues();
            Assert.Equal(expected, actual);

            Assert.True(parser.Read(), "Could not read the second record.");
            expected = new object[] { 234, "Sam", new DateTime(2013, 12, 20) };
            actual = parser.GetValues();
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If we specify null or String.Empty as the record separator, the length of the record
        /// is expected to perfectly match the length of schema.
        /// </summary>
        [Fact]
        public void TestGetValues_NoRecordSeparator_SplitsFile()
        {
            const string text = "       123                      Bob 1/19/2013       234                      Sam12/20/2013";
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10))
                  .AddColumn(new StringColumn("name"), new Window(25))
                  .AddColumn(new DateTimeColumn("created"), new Window(10));
            FixedLengthOptions options = new FixedLengthOptions() { RecordSeparator = null };

            StringReader stringReader = new StringReader(text);
            FixedLengthReader parser = new FixedLengthReader(stringReader, schema, options);

            Assert.True(parser.Read(), "Could not read the first record.");
            object[] expected = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };
            object[] actual = parser.GetValues();
            Assert.Equal(expected, actual);

            Assert.True(parser.Read(), "Could not read the second record.");
            expected = new object[] { 234, "Sam", new DateTime(2013, 12, 20) };
            actual = parser.GetValues();
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If we specify a custom fill character, it should be used to buffer fields in the file.
        /// </summary>
        [Fact]
        public void TestGetValues_CustomFillCharacter_TrimsFill()
        {
            FixedLengthSchema schema = new FixedLengthSchema();
            schema.AddColumn(new Int32Column("id"), new Window(10) { Alignment = FixedAlignment.LeftAligned })
                  .AddColumn(new StringColumn("name"), new Window(25) { Alignment = FixedAlignment.LeftAligned })
                  .AddColumn(new DateTimeColumn("created") { InputFormat = "M/d/yyyy", OutputFormat = "M/d/yyyy" }, new Window(10) { Alignment = FixedAlignment.LeftAligned });
            FixedLengthOptions options = new FixedLengthOptions() { FillCharacter = '@' };
            object[] sources = new object[] { 123, "Bob", new DateTime(2013, 1, 19) };

            StringWriter stringWriter = new StringWriter();
            FixedLengthWriter builder = new FixedLengthWriter(stringWriter, schema, options);
            builder.Write(sources);

            StringReader stringReader = new StringReader(stringWriter.ToString());
            FixedLengthReader parser = new FixedLengthReader(stringReader, schema, options);

            Assert.True(parser.Read(), "Could not read the first record.");
            object[] actual = parser.GetValues();
            Assert.Equal(sources, actual);
        }

        /// <summary>
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [Fact]
        public void TestTypeMapper_Roundtrip()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19) };
            var options = new FixedLengthOptions() { FillCharacter = '@' };

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
        /// We should be able to write and read values using a type mappers.
        /// </summary>
        [Fact]
        public void TestTypeMapper_RoundTrip_SkipHeaderRow()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            var bob = new Person() { Id = 123, Name = "Bob", Created = new DateTime(2013, 1, 19) };
            var options = new FixedLengthOptions() { IsFirstRecordHeader = true, FillCharacter = '@' };

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
        public void TestTypeMapper_RoundtripWithNull()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            var bob = new Person() { Id = 123, Name = null, Created = new DateTime(2013, 1, 19) };
            var options = new FixedLengthOptions() { FillCharacter = '@' };

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
        /// We should be able to round-trip a schema that has separators in a fixed-length schema.
        /// </summary>
        [Fact]
        public void TestTypeMapper_IgnoredSeparators_RoundTrip()
        {
            var mapper = FixedLengthTypeMapper.Define<Person>();
            mapper.Property(p => p.Id, new Window(25)).ColumnName("id");
            mapper.Ignored(new Window(1) { FillCharacter = '|' });
            mapper.Property(p => p.Name, new Window(100)).ColumnName("name");
            mapper.Ignored(new Window(1) { FillCharacter = '|' });
            mapper.Property(p => p.Created, new Window(8)).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            var bob = new Person() { Id = 123, Name = "Bob Smith", Created = new DateTime(2013, 1, 19) };
            var options = new FixedLengthOptions() { FillCharacter = ' ' };

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

        private class Person
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Created { get; set; }
        }
    }
}
