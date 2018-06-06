using System;
using System.Globalization;
using Xunit;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the Int32Column class.
    /// </summary>
    public class Int32ColumnTester
    {
        /// <summary>
        /// An exception should be thrown if name is blank.
        /// </summary>
        [Fact]
        public void TestCtor_NameBlank_Throws()
        {
            Assert.Throws<ArgumentException>(() => new Int32Column("    "));
        }

        /// <summary>
        /// If someone tries to pass a name that contains leading or trailing whitespace, it will be trimmed.
        /// The name will also be made lower case.
        /// </summary>
        [Fact]
        public void TestCtor_SetsName_Trimmed()
        {
            Int32Column column = new Int32Column(" Name   ");
            Assert.Equal("Name", column.ColumnName);
        }

        /// <summary>
        /// If the value is blank and the field is not required, null will be returned.
        /// </summary>
        [Fact]
        public void TestParse_ValueBlank_NullReturned()
        {
            Int32Column column = new Int32Column("count");
            Int32? actual = (Int32?)column.Parse("    ");
            Int32? expected = null;
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If the FormatProvider is null , Parse will use the currrent culture.
        /// </summary>
        [Fact]
        public void TestParse_FormatProviderNull_UsesCurrentCulture()
        {
            Int32Column column = new Int32Column("count");
            column.FormatProvider = null;
            int actual = (int)column.Parse("  -123 ");
            int expected = -123;
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If the FormatProvider is provided, Parse will use the given provider.
        /// </summary>
        [Fact]
        public void TestParse_FormatProviderProvided_UsesProvider()
        {
            Int32Column column = new Int32Column("count");
            column.FormatProvider = CultureInfo.CurrentCulture;
            int actual = (int)column.Parse("  -123 ");
            int expected = -123;
            Assert.Equal(expected, actual);
        }
    }
}
