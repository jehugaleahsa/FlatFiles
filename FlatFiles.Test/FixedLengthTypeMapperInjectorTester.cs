using System;
using System.Collections.Generic;
using System.IO;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class FixedLengthTypeMapperInjectorTester
    {
        [TestMethod]
        public void TestSimpleInjection()
        {
            var injector = new FixedLengthTypeMapperInjector();
            var mapper = FixedLengthTypeMapper.Define<Data>();
            mapper
                .CustomMapping(new Int64Column("amount"), new Window(11)
                {
                    Alignment = FixedAlignment.RightAligned,
                    FillCharacter = '0'
                })
                .WithWriter((p, v) =>
                {
                    return (long)Math.Floor(Math.Abs(v.Amount) * 100M);
                });
            injector.When<Data>().Use(mapper);

            var stringWriter = new StringWriter();
            var writer = injector.GetWriter(stringWriter);

            var data = new List<Data>()
            {
                new Data() { Amount = 1M },
                new Data() { Amount = 2M }
            };

            writer.WriteAll(data);

            var output = stringWriter.ToString();
            Assert.AreEqual("00000000100\r\n00000000200\r\n", output);
        }

        public class Data
        {
            public decimal Amount { get; set; }
        }
    }
}
