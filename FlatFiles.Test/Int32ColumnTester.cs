using System;
using System.Globalization;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the Int32Column class.
    /// </summary>
    [TestClass]
    public class Int32ColumnTester
    {
        /// <summary>
        /// An exception should be thrown if name is blank.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCtor_NameBlank_Throws()
        {
            new Int32Column("    ");
        }

        /// <summary>
        /// If someone tries to pass a name that contains leading or trailing whitespace, it will be trimmed.
        /// The name will also be made lower case.
        /// </summary>
        [TestMethod]
        public void TestCtor_SetsName_Trimmed()
        {
            Int32Column column = new Int32Column(" Name   ");
            Assert.AreEqual("Name", column.ColumnName, "The name was not set as expected.");
        }

        /// <summary>
        /// If the value is blank and the field is not required, null will be returned.
        /// </summary>
        [TestMethod]
        public void TestParse_ValueBlank_NullReturned()
        {
            Int32Column column = new Int32Column("count");
            Int32? actual = (Int32?)column.Parse("    ");
            Int32? expected = null;
            Assert.AreEqual(expected, actual, "The value was not parsed as expected.");
        }

        /// <summary>
        /// If the FormatProvider is null , Parse will use the currrent culture.
        /// </summary>
        [TestMethod]
        public void TestParse_FormatProviderNull_UsesCurrentCulture()
        {
            Int32Column column = new Int32Column("count");
            column.FormatProvider = null;
            int actual = (int)column.Parse("  -123 ");
            int expected = -123;
            Assert.AreEqual(expected, actual, "The value was not parsed correctly.");
        }

        /// <summary>
        /// If the FormatProvider is provided, Parse will use the given provider.
        /// </summary>
        [TestMethod]
        public void TestParse_FormatProviderProvided_UsesProvider()
        {
            Int32Column column = new Int32Column("count");
            column.FormatProvider = CultureInfo.CurrentCulture;
            int actual = (int)column.Parse("  -123 ");
            int expected = -123;
            Assert.AreEqual(expected, actual, "The value was not parsed correctly.");
        }
    }
}
