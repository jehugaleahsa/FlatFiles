using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class IgnoredColumnTester
    {
        [TestMethod]
        public void TestIgnoredColumn_HandlePreAndPostProcessing()
        {
            var ignored = new IgnoredColumn()
            {
                ColumnName = "Ignored",
                NullFormatter = NullFormatter.ForValue("NULL"),
                OnParsing = (ctx, value) => 
                {
                    Assert.AreEqual("NULL", value);
                    return value;
                },
                OnParsed = (ctx, value) =>
                {
                    Assert.IsNull(value);
                    return value;
                },
                OnFormatting = (ctx, value) =>
                {
                    Assert.IsNull(value);
                    return value;
                },
                OnFormatted = (ctx, value) =>
                {
                    Assert.AreEqual("NULL", value);
                    return value;
                }
            };
            object value = ignored.Parse(null, "NULL");
            Assert.IsNull(value);
            string formatted = ignored.Format(null, value);
            Assert.AreEqual("NULL", formatted);
        }

        [TestMethod]
        public void TestIgnoredMapping_HandlePreAndPostProcessing()
        {
            ISeparatedValueTypeMapper<IgnoredOnly> mapper = SeparatedValueTypeMapper.Define(() => new IgnoredOnly());
            mapper.Ignored()
                .ColumnName("Ignored")
                .NullFormatter(NullFormatter.ForValue("NULL"))
                .OnParsing((ctx, value) =>
                {
                    Assert.AreEqual("NULL", value);
                    return value;
                })
                .OnParsed((ctx, value) =>
                {
                    Assert.IsNull(value);
                    return value;
                })
                .OnFormatting((ctx, value) =>
                {
                    Assert.IsNull(value);
                    return value;
                })
                .OnFormatted((ctx, value) =>
                {
                    Assert.AreEqual("NULL", value);
                    return value;
                });
            var ignored = mapper.GetSchema().ColumnDefinitions["Ignored"];
            object value = ignored.Parse(null, "NULL");
            Assert.IsNull(value);
            string formatted = ignored.Format(null, value);
            Assert.AreEqual("NULL", formatted);
        }

        private class IgnoredOnly
        {
            public string Ignored { get; set; }
        }
    }
}
