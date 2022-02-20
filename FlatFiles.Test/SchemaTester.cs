using System;
using System.Collections;
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
        public void TestAddColumn_NullDefinition_Throws()
        {
            DelimitedSchema schema = new DelimitedSchema();
            Assert.ThrowsException<ArgumentNullException>(() => schema.AddColumn(null));
        }

        /// <summary>
        /// An exception should be thrown if a column with the same name was already added.
        /// </summary>
        [TestMethod]
        public void TestAddColumn_DuplicateColumnName_Throws()
        {
            DelimitedSchema schema = new DelimitedSchema();
            schema.AddColumn(new StringColumn("Name"));
            Assert.ThrowsException<ArgumentException>(() => schema.AddColumn(new Int32Column("name")));
        }

        /// <summary>
        /// Each of the values should be parsed and returned in the respective location.
        /// </summary>
        [TestMethod]
        public void TestParseValues_ParsesValues()
        {
            DelimitedSchema schema = new DelimitedSchema();
            schema.AddColumn(new StringColumn("first_name"))
                  .AddColumn(new StringColumn("last_name"))
                  .AddColumn(new DateTimeColumn("birth_date") { InputFormat = "yyyyMMdd" })
                  .AddColumn(new Int32Column("weight"));
            string[] values = new string[] { "bob", "smith", "20120123", "185" };
            var executionContext = new DelimitedExecutionContext(schema, new DelimitedOptions());
            var recordContext = new DelimitedRecordContext(executionContext);
            object[] parsed = schema.ParseValues(recordContext, values);
            object[] expected = new object[] { "bob", "smith", new DateTime(2012, 1, 23), 185 };
            CollectionAssert.AreEqual(expected, parsed);
        }

        /// <summary>
        /// If there are no columns in the schema, the ColumnCollection.Count
        /// should be zero.
        /// </summary>
        [TestMethod]
        public void TestColumnDefinitions_NoColumns_CountZero()
        {
            DelimitedSchema schema = new DelimitedSchema();
            ColumnCollection collection = schema.ColumnDefinitions;
            Assert.AreEqual(0, collection.Count);
        }

        /// <summary>
        /// If there columns in the schema, the ColumnCollection.Count
        /// should equal the number of columns.
        /// </summary>
        [TestMethod]
        public void TestColumnDefinitions_WithColumns_CountEqualsColumnCount()
        {
            DelimitedSchema schema = new DelimitedSchema();
            schema.AddColumn(new Int32Column("id")).AddColumn(new StringColumn("name")).AddColumn(new DateTimeColumn("created"));
            ColumnCollection collection = schema.ColumnDefinitions;
            Assert.AreEqual(3, collection.Count);
        }

        /// <summary>
        /// I should be able to get the column definitions by index.
        /// </summary>
        [TestMethod]
        public void TestColumnDefinitions_FindByIndex()
        {
            DelimitedSchema schema = new DelimitedSchema();
            IColumnDefinition id = new Int32Column("id");
            IColumnDefinition name = new StringColumn("name");
            IColumnDefinition created = new DateTimeColumn("created");
            schema.AddColumn(id).AddColumn(name).AddColumn(created);
            ColumnCollection collection = schema.ColumnDefinitions;
            Assert.AreSame(id, collection[0]);
            Assert.AreSame(name, collection[1]);
            Assert.AreSame(created, collection[2]);
        }

        /// <summary>
        /// Make sure we can get column definitions using the IEnumerable interface.
        /// </summary>
        [TestMethod]
        public void TestColumnDefinitions_GetEnumerable_Explicit()
        {
            DelimitedSchema schema = new DelimitedSchema();
            IColumnDefinition id = new Int32Column("id");
            IColumnDefinition name = new StringColumn("name");
            IColumnDefinition created = new DateTimeColumn("created");
            schema.AddColumn(id).AddColumn(name).AddColumn(created);
            IEnumerable collection = schema.ColumnDefinitions;
            IEnumerator enumerator = collection.GetEnumerator();
        }
    }
}
