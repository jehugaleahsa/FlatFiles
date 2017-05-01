using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Xunit;

namespace FlatFiles.Test
{
    public class UnusualTypesTester
    {
        [Fact]
        public void ShouldRoundTripMaxValues()
        {
            var mapper = getWeirdMapper();
            var thing = new WeirdThing()
            {
                Small = SByte.MaxValue,
                Big = UInt16.MaxValue,
                Bigger = UInt32.MaxValue,
                Huge = UInt64.MaxValue
            };
            var deserialized = roundTrip(mapper, thing);
            assertEqual(thing, deserialized);
        }

        [Fact]
        public void ShouldRoundTripMinValues()
        {
            var mapper = getWeirdMapper();
            var thing = new WeirdThing()
            {
                Small = SByte.MinValue,
                Big = UInt16.MinValue,
                Bigger = UInt32.MinValue,
                Huge = UInt64.MinValue
            };
            var deserialized = roundTrip(mapper, thing);
            assertEqual(thing, deserialized);
        }

        private static ISeparatedValueTypeMapper<WeirdThing> getWeirdMapper()
        {
            var mapper = SeparatedValueTypeMapper.Define<WeirdThing>(() => new WeirdThing());
            mapper.Property(x => x.Small);
            mapper.Property(x => x.Big);
            mapper.Property(x => x.Bigger);
            mapper.Property(x => x.Huge);
            return mapper;
        }

        private static WeirdThing roundTrip(ISeparatedValueTypeMapper<WeirdThing> mapper, WeirdThing thing)
        {
            using (StringWriter writer = new StringWriter())
            {
                mapper.Write(writer, new WeirdThing[] { thing });
                var output = writer.ToString();
                using (StringReader reader = new StringReader(output))
                {
                    var things = mapper.Read(reader).ToArray();
                    Assert.Equal(1, things.Count());
                    var deserialized = things.Single();
                    return deserialized;
                }
            }
        }

        private static void assertEqual(WeirdThing thing1, WeirdThing thing2)
        {
            Assert.Equal(thing1.Small, thing2.Small);
            Assert.Equal(thing1.Big, thing2.Big);
            Assert.Equal(thing1.Bigger, thing2.Bigger);
            Assert.Equal(thing1.Huge, thing2.Huge);
        }

        public class WeirdThing
        {
            public sbyte Small { get; set; }

            public ushort Big { get; set; }

            public uint Bigger { get; set; }

            public ulong Huge { get; set; }
        }
    }
}
