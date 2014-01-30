using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class FixedLengthTypeWriterTester
    {
        /// <summary>
        /// We should be able to write anonymous types to a stream with full type safety.
        /// </summary>
        [TestMethod]
        public void TestTypeWriter_WriteAnonymous()
        {
            var people = from id in Enumerable.Range(0, 1)
                         select new 
                         { 
                             Id = id, 
                             Name = "Bob " + id,
                             Created = new DateTime(2013, 1, 19) 
                         };
            var mapper = FixedLengthTypeMapper.DefineWriter(people);
            mapper.Property(p => p.Id, 10).ColumnName("id");
            mapper.Property(p => p.Name, 100).ColumnName("name");
            mapper.Property(p => p.Created, 8).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            using (MemoryStream stream = new MemoryStream())
            {
                var options = new FixedLengthOptions() { FillCharacter = '@', RecordSeparator = "\n" };
                mapper.Write(stream, options);

                stream.Position = 0;  // go back to the beginning of the stream

                FixedLengthSchema schema = mapper.GetSchema();
                FlatFileReader reader = new FlatFileReader(new FixedLengthReader(stream, schema, options));
                Assert.IsTrue(reader.Read(), "The writer did not write the entities.");
                int id = reader.GetInt32(0);
                string name = reader.GetString(1);
                DateTime created = reader.GetDateTime(2);
                Assert.AreEqual(people.First().Id, id, "The ID value was not persisted.");
                Assert.AreEqual(people.First().Name, name, "The Name value was not persisted.");
                Assert.AreEqual(people.First().Created, created, "The Created value was not persisted.");
                Assert.IsFalse(reader.Read(), "The writer wrote too many records.");
            }
        }
    }
}
