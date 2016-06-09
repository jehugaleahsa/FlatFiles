using System;
using System.Collections;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
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
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(null);
        }

        /// <summary>
        /// An exception should be thrown if a column with the same name was already added.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestAddColumn_DuplicateColumnName_Throws()
        {
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("Name"));
            schema.AddColumn(new Int32Column("name"));
        }

        /// <summary>
        /// Each of the values should be parsed and returned in the respective location.
        /// </summary>
        [TestMethod]
        public void TestParseValues_ParsesValues()
        {
            SeparatedValueSchema schema = new SeparatedValueSchema();
            schema.AddColumn(new StringColumn("first_name"))
                  .AddColumn(new StringColumn("last_name"))
                  .AddColumn(new DateTimeColumn("birth_date") { InputFormat = "yyyyMMdd" })
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
            SeparatedValueSchema schema = new SeparatedValueSchema();
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
            SeparatedValueSchema schema = new SeparatedValueSchema();
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
            SeparatedValueSchema schema = new SeparatedValueSchema();
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
            SeparatedValueSchema schema = new SeparatedValueSchema();
            ColumnDefinition id = new Int32Column("id");
            ColumnDefinition name = new StringColumn("name");
            ColumnDefinition created = new DateTimeColumn("created");
            schema.AddColumn(id).AddColumn(name).AddColumn(created);
            IEnumerable collection = schema.ColumnDefinitions;
            IEnumerator enumerator = collection.GetEnumerator();
        }
    }
}
