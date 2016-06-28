using System;
using Xunit;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the StringColumn class.
    /// </summary>
    public class StringColumnTester
    {
        /// <summary>
        /// An exception should be thrown if name is blank.
        /// </summary>
        [Fact]
        public void TestCtor_NameBlank_Throws()
        {
            Assert.Throws<ArgumentException>(() => new StringColumn("    "));
        }

        /// <summary>
        /// If someone tries to pass a name that contains leading or trailing whitespace, it will be trimmed.
        /// The name will also be made lower case.
        /// </summary>
        [Fact]
        public void TestCtor_SetsName_Trimmed()
        {
            StringColumn column = new StringColumn(" Name   ");
            Assert.Equal("Name", column.ColumnName);
        }

        /// <summary>
        /// If the value is blank, it is interpreted as null.
        /// </summary>
        [Fact]
        public void TestParse_ValueBlank_ReturnsNull()
        {
            StringColumn column = new StringColumn("name");
            string actual = (string)column.Parse("     ");
            string expected = null;
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If the value is not blank, it is trimmed.
        /// </summary>
        [Fact]
        public void TestParse_ValueTrimmed()
        {
            StringColumn column = new StringColumn("name");
            string actual = (string)column.Parse("  abc 123 ");
            string expected = "abc 123";
            Assert.Equal(expected, actual);
        }
    }
}
