using System;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class ExcelReaderTester
    {
        [TestMethod]
        public void ShouldReadExcelFile()
        {
            var mapper = ExcelTypeMapper.Define<Lead>();
            mapper.Property(l => l.BusinessName).ColumnName("BusinessName");
            mapper.Property(l => l.TotalEmployees).ColumnName("TotalEmployees");
            mapper.Property(l => l.SalesVolume).ColumnName("SalesVolume");
            mapper.Property(l => l.NextAdvertisingDate).ColumnName("NextAdvertisingDate");

            string fileName = @"TestFiles\test.xlsx";

            ExcelOptions options = new ExcelOptions("Leads");
            options.IsFirstRecordSchema = false;
            options.StartingRow = 3;
            options.StartingColumn = "B";

            var leads = mapper.Read(fileName, options);
        }

        public class Lead
        {
            public string BusinessName { get; set; }

            public int TotalEmployees { get; set; }

            public decimal? SalesVolume { get; set; }

            public DateTime? NextAdvertisingDate { get; set; }
        }
    }
}
