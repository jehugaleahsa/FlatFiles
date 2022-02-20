using System.Globalization;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class PreprocessorTester
    {
        [TestMethod]
        public void ShouldStripNonNumericCharacters_Preprocessor()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");

            const string input = @"=""12345.67"",=""$123""";

            var mapper = DelimitedTypeMapper.Define<Numbers>();
#pragma warning disable CS0618 // Type or member is obsolete
            mapper.Property(x => x.Value).ColumnName("value").Preprocessor(x => x.Trim('"', '=')).NumberStyles(NumberStyles.AllowDecimalPoint);
            mapper.Property(x => x.Money).ColumnName("money").Preprocessor(x => x.Trim('"', '=')).NumberStyles(NumberStyles.Currency);
#pragma warning restore CS0618 // Type or member is obsolete

            StringReader reader = new StringReader(input);
            var results = mapper.Read(reader).ToArray();

            Assert.AreEqual(1, results.Length);
            var result = results.Single();
            Assert.AreEqual(12345.67m, result.Value);
            Assert.AreEqual(123m, result.Money);
        }

        [TestMethod]
        public void ShouldStripNonNumericCharacters_OnParsing()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");

            const string input = @"=""12345.67"",=""$123""";

            var mapper = DelimitedTypeMapper.Define<Numbers>();
            mapper.Property(x => x.Value).ColumnName("value").OnParsing((ctx, x) => x.Trim('"', '=')).NumberStyles(NumberStyles.AllowDecimalPoint);
            mapper.Property(x => x.Money).ColumnName("money").OnParsing((ctx, x) => x.Trim('"', '=')).NumberStyles(NumberStyles.Currency);

            StringReader reader = new StringReader(input);
            var results = mapper.Read(reader).ToArray();

            Assert.AreEqual(1, results.Length);
            var result = results.Single();
            Assert.AreEqual(12345.67m, result.Value);
            Assert.AreEqual(123m, result.Money);
        }

        public class Numbers
        {
            public decimal Value { get; set; }

            public decimal Money { get; set; }
        }
    }
}
