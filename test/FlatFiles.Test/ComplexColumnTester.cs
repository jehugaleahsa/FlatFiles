using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class ComplexColumnTester
    {
        [Fact]
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
            Assert.True(reader.Read(), "A record should have been read.");
            object[] values = reader.GetValues();
            Assert.Equal("Tom", values[0]);
            Assert.Equal("Hanselman", values[1]);
            Assert.IsType<object[]>(values[2]);
            object[] playerValues = (object[])values[2];
            Assert.Equal(new DateTime(2016, 06, 04), playerValues[0]);
            Assert.Equal(26, playerValues[1]);
            Assert.Equal("Walking Ice", playerValues[2]);
            Assert.Equal("Ace", values[3]);

            StringWriter stringWriter = new StringWriter();
            SeparatedValueWriter writer = new SeparatedValueWriter(stringWriter, outerSchema);
            writer.Write(values);

            string output = stringWriter.GetStringBuilder().ToString();
            Assert.Equal(message, output);
        }

        [Fact]
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
            Assert.Single(players);
            var player = players.Single();
                
            Assert.Equal("Tom", player.FirstName);
            Assert.Equal("Hanselman", player.LastName);
            Stats statistics = player.Statistics;
            Assert.NotNull(statistics);
            Assert.Equal(new DateTime(2016, 06, 04), statistics.StartDate);
            Assert.Equal(26, statistics.Age);
            Assert.Equal("Walking Ice", statistics.StageName);
            Assert.Equal("Ace", player.NickName);

            StringWriter stringWriter = new StringWriter();
            mapper.Write(stringWriter, new Player[] { player });
            
            string output = stringWriter.ToString();
            Assert.Equal(message, output);
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
