using System;
using System.Globalization;
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
        public void TestCtor_NameBlank_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => new Int32Column("    "));
        }

        /// <summary>
        /// If someone tries to pass a name that contains leading or trailing whitespace, it will be trimmed.
        /// The name will also be made lower case.
        /// </summary>
        [TestMethod]
        public void TestCtor_SetsName_Trimmed()
        {
            Int32Column column = new Int32Column(" Name   ");
            Assert.AreEqual("Name", column.ColumnName);
        }

        /// <summary>
        /// If the value is blank and the field is not required, null will be returned.
        /// </summary>
        [TestMethod]
        public void TestParse_ValueBlank_NullReturned()
        {
            Int32Column column = new Int32Column("count");
            Int32? actual = (Int32?)column.Parse(null, "    ");
            Int32? expected = null;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If the FormatProvider is null , Parse will use the currrent culture.
        /// </summary>
        [TestMethod]
        public void TestParse_FormatProviderNull_UsesCurrentCulture()
        {
            Int32Column column = new Int32Column("count")
            {
                FormatProvider = null
            };
            int actual = (int)column.Parse(null, "  -123 ");
            int expected = -123;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If the FormatProvider is provided, Parse will use the given provider.
        /// </summary>
        [TestMethod]
        public void TestParse_FormatProviderProvided_UsesProvider()
        {
            Int32Column column = new Int32Column("count")
            {
                FormatProvider = CultureInfo.CurrentCulture
            };
            int actual = (int)column.Parse(null, "  -123 ");
            int expected = -123;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// An exception should be thrown if trying to parse a null when not nullable.
        /// </summary>
        [TestMethod]
        public void TestParse_NotNullable_NullValue_Throws()
        {
            Int32Column column = new Int32Column("count") { IsNullable = false };
            Assert.ThrowsException<InvalidCastException>(() => column.Parse(null, String.Empty));
        }
    }
}
