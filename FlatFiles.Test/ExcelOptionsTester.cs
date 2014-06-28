using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class ExcelOptionsTester
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowNullWorksheetName()
        {
            new ExcelOptions(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowBlankWorksheetName()
        {
            new ExcelOptions(String.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowAllWhitespaceWorksheetName()
        {
            new ExcelOptions("   ");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldNotAllowNegativeStartingRow()
        {
            var options = new ExcelOptions("Sheet1");
            options.StartingRow = -1;
        }

        [TestMethod]
        public void ShouldAllowNullStartingRow()
        {
            var options = new ExcelOptions("Sheet1");
            options.StartingRow = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ShouldNotAllowNegativeEndingRow()
        {
            var options = new ExcelOptions("Sheet1");
            options.EndingRow = -1;
        }

        [TestMethod]
        public void ShouldAllowNullEndingRow()
        {
            var options = new ExcelOptions("Sheet1");
            options.EndingRow = null;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowBlankStartingColumn()
        {
            var options = new ExcelOptions("Sheet1");
            options.StartingColumn = String.Empty;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowNonCharactersStartingColumn()
        {
            var options = new ExcelOptions("Sheet1");
            options.StartingColumn = "W1";
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowWhitespaceCharactersStartingColumn()
        {
            var options = new ExcelOptions("Sheet1");
            options.StartingColumn = "   ";
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowLowercaseCharactersStartingColumn()
        {
            var options = new ExcelOptions("Sheet1");
            options.StartingColumn = "a";
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowBlankEndingColumn()
        {
            var options = new ExcelOptions("Sheet1");
            options.EndingColumn = String.Empty;
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowNonCharactersEndingColumn()
        {
            var options = new ExcelOptions("Sheet1");
            options.EndingColumn = "W1";
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowWhitespaceCharactersEndingColumn()
        {
            var options = new ExcelOptions("Sheet1");
            options.EndingColumn = "   ";
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ShouldNotAllowLowercaseCharactersEndingColumn()
        {
            var options = new ExcelOptions("Sheet1");
            options.EndingColumn = "a";
        }
    }
}
