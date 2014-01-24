namespace FlatFiles.Test
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests the Mapper class.
    /// </summary>
    [TestClass]
    public class MapperTester
    {
        /// <summary>
        /// Setup for tests.
        /// </summary>
        [TestInitialize]
        public void TestSetup()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        }

        /// <summary>
        /// We can map a CSV file to a data object.
        /// </summary>
        [TestMethod]
        public void ShouldCreateCustomerFromCsv()
        {
            SeparatedValueSchema schema = new SeparatedValueSchema().AddColumn<string>("name").AddColumn<DateTime>("modified").AddColumn<int>("visits");
            Mapper<Customer> mapper = new Mapper<Customer>().Map(c => c.Name).To("name").Map(c => c.LastModified).To("modified").Map(c => c.Visits).To("visits");

            const string data = @"name,modified,visits
bob,12/31/2012,108";

            Stream stream = new MemoryStream(Encoding.Default.GetBytes(data));
            SeparatedValueOptions options = new SeparatedValueOptions() { IsFirstRecordSchema = true };
            SeparatedValueReader parser = new SeparatedValueReader(stream, schema, options);
            FlatFileReader reader = new FlatFileReader(parser);

            IEnumerable<Customer> customers = mapper.Extract(reader);
            Assert.AreEqual(1, customers.Count(), "The wrong number of records were mapped.");
            Assert.AreEqual("bob", customers.First().Name, "The customer name was not parsed correctly.");
            Assert.AreEqual(new DateTime(2012, 12, 31), customers.First().LastModified, "The customer modified date was not parsed correctly.");
            Assert.AreEqual(108, customers.First().Visits, "The customer visits was not parsed correctly.");
        }

        public sealed class Customer
        {
            public string Name { get; set; }

            public DateTime LastModified { get; set; }

            public int Visits { get; set; }
        }
    }
}
