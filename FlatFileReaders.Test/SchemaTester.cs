using System;
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
    }
}
