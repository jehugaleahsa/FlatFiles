using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class ComplexColumnTester
    {
        [TestMethod]
        public void ShouldRoundTrip()
        {
            const string message = @"Tom,Hanselman,2016-06-0426         Walking Ice,Ace
";
            StringReader stringReader = new StringReader(message);
            SeparatedValueSchema outerSchema = new SeparatedValueSchema();
            outerSchema.AddColumn(new StringColumn("FirstName"));
            outerSchema.AddColumn(new StringColumn("LastName"));

            FixedLengthSchema innerSchema = new FixedLengthSchema();
            innerSchema.AddColumn(new DateTimeColumn("StartDate") { InputFormat = "yyyy-MM-dd", OutputFormat = "yyyy-MM-dd" }, 10);
            innerSchema.AddColumn(new Int32Column("Age"), 2);
            innerSchema.AddColumn(new StringColumn("StageName"), new Window(20) { Alignment = FixedAlignment.RightAligned });
            outerSchema.AddColumn(new FixedLengthComplexColumn("PlayerStats", innerSchema));
            outerSchema.AddColumn(new StringColumn("Nickname"));

            SeparatedValueReader reader = new SeparatedValueReader(stringReader, outerSchema, new SeparatedValueOptions { RecordSeparator = "\n" });
            Assert.IsTrue(reader.Read(), "A record should have been read.");
            object[] values = reader.GetValues();
            Assert.AreEqual("Tom", values[0]);
            Assert.AreEqual("Hanselman", values[1]);
            Assert.IsInstanceOfType(values[2], typeof(object[]));
            object[] playerValues = (object[])values[2];
            Assert.AreEqual(new DateTime(2016, 06, 04), playerValues[0]);
            Assert.AreEqual(26, playerValues[1]);
            Assert.AreEqual("Walking Ice", playerValues[2]);
            Assert.AreEqual("Ace", values[3]);

            StringWriter stringWriter = new StringWriter();
            SeparatedValueWriter writer = new SeparatedValueWriter(stringWriter, outerSchema, new SeparatedValueOptions { RecordSeparator = "\n" });
            writer.Write(values);

            string output = stringWriter.GetStringBuilder().ToString();
            Assert.AreEqual(message, output);
        }

        [TestMethod]
        public void ShouldRoundTrip_TypeMappers()
        {
            const string message = @"Tom,Hanselman,2016-06-0426         Walking Ice,Ace
";
            StringReader stringReader = new StringReader(message);

            var nestedMapper = FixedLengthTypeMapper.Define<Stats>();
            nestedMapper.Property(s => s.StartDate, 10).InputFormat("yyyy-MM-dd").OutputFormat("yyyy-MM-dd");
            nestedMapper.Property(s => s.Age, 2);
            nestedMapper.Property(s => s.StageName, new Window(20) { Alignment = FixedAlignment.RightAligned });

            var mapper = SeparatedValueTypeMapper.Define<Player>();
            mapper.Property(p => p.FirstName);
            mapper.Property(p => p.LastName);
            mapper.ComplexProperty(p => p.Statistics, nestedMapper);
            mapper.Property(p => p.NickName);

            var players = mapper.Read(stringReader).ToArray();
            Assert.AreEqual(1, players.Length);
            var player = players.Single();
                
            Assert.AreEqual("Tom", player.FirstName);
            Assert.AreEqual("Hanselman", player.LastName);
            Stats statistics = player.Statistics;
            Assert.IsNotNull(statistics);
            Assert.AreEqual(new DateTime(2016, 06, 04), statistics.StartDate);
            Assert.AreEqual(26, statistics.Age);
            Assert.AreEqual("Walking Ice", statistics.StageName);
            Assert.AreEqual("Ace", player.NickName);

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Player[] { player }, new SeparatedValueOptions { RecordSeparator = "\n" });
            
            string output = stringWriter.ToString();
            Assert.AreEqual(message, output);
        }

        public class Player
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public Stats Statistics { get; set; }
            
            public string NickName { get; set; }
        }

        public class Stats
        {
            public DateTime StartDate { get; set; }

            public int Age { get; set; }

            public string StageName { get; set; }
        }
    }
}
