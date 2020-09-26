using System;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    /// <summary>
    /// Tests the TimeSpan class.
    /// </summary>
    [TestClass]
    public class TimeSpanColumnTester
    {
        public TimeSpanColumnTester()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// An exception should be thrown if name is blank.
        /// </summary>
        [TestMethod]
        public void TestCtor_NameBlank_Throws()
        {
            Assert.ThrowsException<ArgumentException>(() => new TimeSpanColumn("    "));
        }

        /// <summary>
        /// If someone tries to pass a name that contains leading or trailing whitespace, it will be trimmed.
        /// The name will also be made lower case.
        /// </summary>
        [TestMethod]
        public void TestCtor_SetsName_Trimmed()
        {
            var column = new TimeSpanColumn(" Name   ");
            Assert.AreEqual("Name", column.ColumnName);
        }

        /// <summary>
        /// If no format string is provided, a generic parse will be attempted.
        /// </summary>
        [TestMethod]
        public void TestParse_NoFormatString_ParsesGenerically()
        {
            var column = new TimeSpanColumn("created");
            var actual = (TimeSpan)column.Parse(null, "12.13:23:33");
            var expected = new TimeSpan(12, 13, 23, 33);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestParse_FormatString_ParseExact()
        {
            var column = new TimeSpanColumn("created")
            {
                InputFormat = @"d\.hh\:mm\:ss\.FFF"
            };
            var actual = (TimeSpan)column.Parse(null, "12.13:23:33.123");
            var expected = new TimeSpan(12, 13, 23, 33, 123);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// If the value is blank and the field is not required, null will be returned.
        /// </summary>
        [TestMethod]
        public void TestParse_ValueBlank_NullReturned()
        {
            var column = new TimeSpanColumn("created");
            var actual = (TimeSpan?)column.Parse(null, "    ");
            Assert.IsNull(actual);
        }

        /// <summary>
        /// If the value is blank and the field is not required, null will be returned.
        /// </summary>
        [TestMethod]
        public void TestParse_Preprocessor_ShouldBeCalledOnce()
        {
            int preprocessorCallCount = 0;

            var column = new TimeSpanColumn("created");
#pragma warning disable CS0618 // Type or member is obsolete
            column.Preprocessor = (value) => { preprocessorCallCount++; return value; };
#pragma warning restore CS0618 // Type or member is obsolete

            var actual = (TimeSpan?)column.Parse(null, "    ");
            Assert.AreEqual(1, preprocessorCallCount, "Preprocessor function should be called exactly once");
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestFromDays_FromDouble()
        {
            var daysColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromDays(daysColumn);
            
            var actual = (TimeSpan)durationColumn.Parse(null, "1");
            Assert.AreEqual(TimeSpan.FromDays(1), actual);
        }

        [TestMethod]
        public void TestFromDays_ToDouble()
        {
            var daysColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromDays(daysColumn);

            var actual = durationColumn.Format(null, TimeSpan.FromDays(1));
            Assert.AreEqual("1", actual);
        }

        [TestMethod]
        public void TestFromDays_FromNull()
        {
            var daysColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromDays(daysColumn);
            
            var actual = (TimeSpan?)durationColumn.Parse(null, String.Empty);
            Assert.IsNull(actual);
        }

        [TestMethod]
        public void TestFromDays_ToNull()
        {
            var daysColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromDays(daysColumn);

            var actual = durationColumn.Format(null, null);
            Assert.AreEqual(String.Empty, actual);
        }

        [TestMethod]
        public void TestFromHours_FromDouble()
        {
            var hoursColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromHours(hoursColumn);
            
            var actual = (TimeSpan)durationColumn.Parse(null, "24");
            Assert.AreEqual(TimeSpan.FromDays(1), actual);
        }

        [TestMethod]
        public void TestFromHours_ToDouble()
        {
            var hoursColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromHours(hoursColumn);

            var actual = durationColumn.Format(null, TimeSpan.FromDays(1));
            Assert.AreEqual("24", actual);
        }

        [TestMethod]
        public void TestFromMilliseconds_FromDouble()
        {
            var msColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromMillseconds(msColumn);

            var actual = (TimeSpan)durationColumn.Parse(null, "86400000");
            Assert.AreEqual(TimeSpan.FromDays(1), actual);
        }

        [TestMethod]
        public void TestFromMilliseconds_ToDouble()
        {
            var msColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromMillseconds(msColumn);

            var actual = durationColumn.Format(null, TimeSpan.FromDays(1));
            Assert.AreEqual("86400000", actual);
        }

        [TestMethod]
        public void TestFromMinutes_FromDouble()
        {
            var minutesColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromMinutes(minutesColumn);

            var actual = (TimeSpan)durationColumn.Parse(null, "1440");
            Assert.AreEqual(TimeSpan.FromDays(1), actual);
        }

        [TestMethod]
        public void TestFromMinutes_ToDouble()
        {
            var minutesColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromMinutes(minutesColumn);

            var actual = durationColumn.Format(null, TimeSpan.FromDays(1));
            Assert.AreEqual("1440", actual);
        }

        [TestMethod]
        public void TestFromSeconds_FromDouble()
        {
            var secondsColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromSeconds(secondsColumn);

            var actual = (TimeSpan)durationColumn.Parse(null, "86400");
            Assert.AreEqual(TimeSpan.FromDays(1), actual);
        }

        [TestMethod]
        public void TestFromSeconds_ToDouble()
        {
            var secondsColumn = new DoubleColumn("Duration");
            var durationColumn = TimeSpanColumn.FromSeconds(secondsColumn);

            var actual = durationColumn.Format(null, TimeSpan.FromDays(1));
            Assert.AreEqual("86400", actual);
        }

        [TestMethod]
        public void TestFromTicks_FromInt64()
        {
            var ticksColumn = new Int64Column("Duration");
            var durationColumn = TimeSpanColumn.FromTicks(ticksColumn);

            var actual = (TimeSpan)durationColumn.Parse(null, "864000000000");
            Assert.AreEqual(TimeSpan.FromDays(1), actual);
        }

        [TestMethod]
        public void TestFromTicks_ToInt64()
        {
            var ticksColumn = new Int64Column("Duration");
            var durationColumn = TimeSpanColumn.FromTicks(ticksColumn);

            var actual = durationColumn.Format(null, TimeSpan.FromDays(1));
            Assert.AreEqual("864000000000", actual);
        }
    }
}
