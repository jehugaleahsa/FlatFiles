using System;
using System.Globalization;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class PreprocessorTester
    {
        [Fact]
        public void ShouldStripNonNumericCharacters()
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US");

            const string input = @"=""12345.67"",=""$123""";

            var mapper = SeparatedValueTypeMapper.Define<Numbers>();
            mapper.Property(x => x.Value).ColumnName("value").Preprocessor(x => x.Trim('"', '=')).NumberStyles(NumberStyles.AllowDecimalPoint);
            mapper.Property(x => x.Money).ColumnName("money").Preprocessor(x => x.Trim('"', '=')).NumberStyles(NumberStyles.Currency);

            StringReader reader = new StringReader(input);
            var results = mapper.Read(reader).ToArray();

            Assert.Equal(1, results.Count());
            var result = results.Single();
            Assert.Equal(12345.67m, result.Value);
            Assert.Equal(123m, result.Money);
        }

        public class Numbers
        {
            public decimal Value { get; set; }

            public decimal Money { get; set; }
        }
    }
}
