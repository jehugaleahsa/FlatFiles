using System.Linq;
using System.IO;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class FixedLengthTypeMapperSelectorTester
    {
        [TestMethod]
        public void TestSimpleSelection()
        {
            var selector = new FixedLengthTypeMapperSelector();
            var mapper = FixedLengthTypeMapper.Define<Data>();
            mapper.Property(x => x.Amount, new Window(11)
            {
                Alignment = FixedAlignment.RightAligned,
                FillCharacter = '0'
            });
            selector.When(v => v.Length == 11).Use(mapper);

            var stringReader = new StringReader("00000000100\r\n00000000200\r\n");
            var reader = selector.GetReader(stringReader);

            var data = reader.ReadAll().Cast<Data>().ToList();
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(100, data[0].Amount);
            Assert.AreEqual(200, data[1].Amount);
        }

        public class Data
        {
            public decimal Amount { get; set; }
        }
    }
}
