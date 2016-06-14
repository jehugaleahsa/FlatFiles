using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the StringColumn class.
    /// </summary>
    [TestClass]
    public class StringColumnTester
    {
        /// <summary>
        /// An exception should be thrown if name is blank.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCtor_NameBlank_Throws()
        {
            new StringColumn("    ");
        }

        /// <summary>
        /// If someone tries to pass a name that contains leading or trailing whitespace, it will be trimmed.
        /// The name will also be made lower case.
        /// </summary>
        [TestMethod]
        public void TestCtor_SetsName_Trimmed()
        {
            StringColumn column = new StringColumn(" Name   ");
            Assert.AreEqual("Name", column.ColumnName, "The name was not set as expected.");
        }

        /// <summary>
        /// If the value is blank, it is interpreted as null.
        /// </summary>
        [TestMethod]
        public void TestParse_ValueBlank_ReturnsNull()
        {
            StringColumn column = new StringColumn("name");
            string actual = (string)column.Parse("     ");
            string expected = null;
            Assert.AreEqual(expected, actual, "The value was not parsed as null.");
        }

        /// <summary>
        /// If the value is not blank, it is trimmed.
        /// </summary>
        [TestMethod]
        public void TestParse_ValueTrimmed()
        {
            StringColumn column = new StringColumn("name");
            string actual = (string)column.Parse("  abc 123 ");
            string expected = "abc 123";
            Assert.AreEqual(expected, actual, "The value was not trimmed.");
        }
    }
}
