using System;
using System.IO;
using System.Linq;
using System.Text;
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

            SeparatedValueReader reader = new SeparatedValueReader(stringReader, outerSchema);
            Assert.IsTrue(reader.Read(), "A record should have been read.");
            object[] values = reader.GetValues();
            Assert.AreEqual("Tom", values[0], "The FirstName was wrong.");
            Assert.AreEqual("Hanselman", values[1], "The LastName was wrong.");
            Assert.IsInstanceOfType(values[2], typeof(object[]), "The PlayerStats was not an object[].");
            object[] playerValues = (object[])values[2];
            Assert.AreEqual(new DateTime(2016, 06, 04), playerValues[0], "The StartDate was wrong.");
            Assert.AreEqual(26, playerValues[1], "The Age was not wrong.");
            Assert.AreEqual("Walking Ice", playerValues[2], "The StageName was wrong.");
            Assert.AreEqual("Ace", values[3], "The Nickname was wrong.");

            StringWriter stringWriter = new StringWriter();
            SeparatedValueWriter writer = new SeparatedValueWriter(stringWriter, outerSchema);
            writer.Write(values);

            string output = stringWriter.GetStringBuilder().ToString();
            Assert.AreEqual(message, output, "The re-written message did not match the original message.");
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
            Assert.AreEqual(1, players.Count(), "The wrong number of players were found.");
            var player = players.Single();
                
            Assert.AreEqual("Tom", player.FirstName, "The FirstName was wrong.");
            Assert.AreEqual("Hanselman", player.LastName, "The LastName was wrong.");
            Stats statistics = player.Statistics;
            Assert.IsNotNull(statistics, "The statistics object was not created.");
            Assert.AreEqual(new DateTime(2016, 06, 04), statistics.StartDate, "The StartDate was wrong.");
            Assert.AreEqual(26, statistics.Age, "The Age was not wrong.");
            Assert.AreEqual("Walking Ice", statistics.StageName, "The StageName was wrong.");
            Assert.AreEqual("Ace", player.NickName, "The Nickname was wrong.");

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Player[] { player });
            
            string output = stringWriter.ToString();
            Assert.AreEqual(message, output, "The re-written message did not match the original message.");
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
