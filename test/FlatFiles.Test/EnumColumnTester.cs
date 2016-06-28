using System;
using Xunit;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the EnumColumn class.
    /// </summary>
    public class EnumColumnTester
    {
        public enum MyEnum
        {
            First = 1,
            Second = 2
        }

        /// <summary>
        /// An exception should be thrown if TEnum is not an enumeration.
        /// </summary>
        [Fact]
        public void TestCtor_NonEnum_Throws()
        {
            Assert.Throws<ArgumentException>(() => new EnumColumn<DateTime>("    "));
        }

        /// <summary>
        /// An exception should be thrown if name is blank.
        /// </summary>
        [Fact]
        public void TestCtor_NameBlank_Throws()
        {
            Assert.Throws<ArgumentException>(() => new EnumColumn<MyEnum>("    "));
        }

        /// <summary>
        /// If someone tries to pass a name that contains leading or trailing whitespace, it will be trimmed.
        /// The name will also be made lower case.
        /// </summary>
        [Fact]
        public void TestCtor_SetsName_Trimmed()
        {
            EnumColumn<MyEnum> column = new EnumColumn<MyEnum>(" Name   ");
            Assert.Equal("Name", column.ColumnName);
        }

        /// <summary>
        /// If the value is blank and the field is not required, null will be returned.
        /// </summary>
        [Fact]
        public void TestParse_ValueBlank_NullReturned()
        {
            EnumColumn<MyEnum> column = new EnumColumn<MyEnum>("count");
            MyEnum? actual = (MyEnum?)column.Parse("    ");
            MyEnum? expected = null;
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If the value is a valid integer value, the equivalent enum value should be returned.
        /// </summary>
        [Fact]
        public void TestParse_Int32Value_EnumReturned()
        {
            EnumColumn<MyEnum> column = new EnumColumn<MyEnum>("count");
            MyEnum actual = (MyEnum)column.Parse("1");
            MyEnum expected = MyEnum.First;
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// If the value is a valid string value, the equivalent enum value should be returned.
        /// </summary>
        [Fact]
        public void TestParse_StringValue_EnumReturned()
        {
            EnumColumn<MyEnum> column = new EnumColumn<MyEnum>("count");
            MyEnum actual = (MyEnum)column.Parse("First");
            MyEnum expected = MyEnum.First;
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// By default, enums should be written as integers.
        /// </summary>
        [Fact]
        public void TestFormat_IntegerStringReturned()
        {
            EnumColumn<MyEnum> column = new EnumColumn<MyEnum>("count");
            string actual = column.Format(MyEnum.First);
            string expected = "1";
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// The given formatter should be used.
        /// </summary>
        [Fact]
        public void TestFormat_OverrideFormatter_UsesCustomFormatter()
        {
            EnumColumn<MyEnum> column = new EnumColumn<MyEnum>("count");
            column.Formatter = e => e.ToString();
            string actual = column.Format(MyEnum.First);
            string expected = "First";
            Assert.Equal(expected, actual);
        }
    }
}
