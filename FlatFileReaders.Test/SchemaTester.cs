using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFileReaders.Test
{
    /// <summary>
    /// Tests the Schema class.
    /// </summary>
    [TestClass]
    public class SchemaTester
    {
        /// <summary>
        /// An exception should be thrown if we try to add a null column definition.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestAddColumn_NullDefinition_Throws()
        {
            Schema schema = new Schema();
            schema.AddColumn(null);
        }

        /// <summary>
        /// An exception should be thrown if a column with the same name was already added.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddColumn_DuplicateColumnName_Throws()
        {
            Schema schema = new Schema();
            schema.AddColumn(new StringColumn("Name"));
            schema.AddColumn(new Int32Column("name"));
        }

        /// <summary>
        /// An exception should be thrown if a null values list is passed to the ParseValues method.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestParseValues_NullValues_Throws()
        {
            Schema schema = new Schema();
            schema.ParseValues(null);
        }

        /// <summary>
        /// An exception should be thrown if the number of values does not equal the number of columns.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestParseValues_WrongNumber_Throws()
        {
            Schema schema = new Schema();
            schema.AddColumn(new StringColumn("name"));
            schema.ParseValues(new string[] { "bob", "smith" });
        }

        /// <summary>
        /// Each of the values should be parsed and returned in the respective location.
        /// </summary>
        [TestMethod]
        public void TestParseValues_ParsesValues()
        {
            Schema schema = new Schema();
            schema.AddColumn(new StringColumn("first_name"))
                  .AddColumn(new StringColumn("last_name"))
                  .AddColumn(new DateTimeColumn("birth_date") { DateTimeFormat = "yyyyMMdd" })
                  .AddColumn(new Int32Column("weight"));
            string[] values = new string[] { "bob", "smith", "20120123", "185" };
            object[] parsed = schema.ParseValues(values);
            object[] expected = new object[] { "bob", "smith", new DateTime(2012, 1, 23), 185 };
            CollectionAssert.AreEqual(expected, parsed, "The values were not parsed as expected.");
        }

        /// <summary>
        /// If there are no columns in the schema, the ColumnCollection.Count
        /// should be zero.
        /// </summary>
        [TestMethod]
        public void TestColumnDefinitions_NoColumns_CountZero()
        {
            Schema schema = new Schema();
            ColumnCollection collection = schema.ColumnDefinitions;
            Assert.AreEqual(0, collection.Count, "The column collection count was wrong.");
        }

        /// <summary>
        /// If there columns in the schema, the ColumnCollection.Count
        /// should equal the number of columns.
        /// </summary>
        [TestMethod]
        public void TestColumnDefinitions_WithColumns_CountEqualsColumnCount()
        {
            Schema schema = new Schema();
            schema.AddColumn(new Int32Column("id")).AddColumn(new StringColumn("name")).AddColumn(new DateTimeColumn("created"));
            ColumnCollection collection = schema.ColumnDefinitions;
            Assert.AreEqual(3, collection.Count, "The column collection count was wrong.");
        }

        /// <summary>
        /// I should be able to get the column definitions by index.
        /// </summary>
        [TestMethod]
        public void TestColumnDefinitions_FindByIndex()
        {
            Schema schema = new Schema();
            ColumnDefinition id = new Int32Column("id");
            ColumnDefinition name = new StringColumn("name");
            ColumnDefinition created = new DateTimeColumn("created");
            schema.AddColumn(id).AddColumn(name).AddColumn(created);
            ColumnCollection collection = schema.ColumnDefinitions;
            Assert.AreSame(id, collection[0], "The first column definition was wrong.");
            Assert.AreSame(name, collection[1], "The second column definition was wrong.");
            Assert.AreSame(created, collection[2], "The third column definition was wrong.");
        }

        /// <summary>
        /// Make sure we can get column definitions using the IEnumerable interface.
        /// </summary>
        [TestMethod]
        public void TestColumnDefinitions_GetEnumerable_Explicit()
        {
            Schema schema = new Schema();
            ColumnDefinition id = new Int32Column("id");
            ColumnDefinition name = new StringColumn("name");
            ColumnDefinition created = new DateTimeColumn("created");
            schema.AddColumn(id).AddColumn(name).AddColumn(created);
            IEnumerable collection = schema.ColumnDefinitions;
            IEnumerator enumerator = collection.GetEnumerator();
        }
    }
}
