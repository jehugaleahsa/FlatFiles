using System;
using System.IO;
using System.Linq;
using FlatFiles.TypeMapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public class SeparatedValueTypeWriterTester
    {
        /// <summary>
        /// We should be able to write anonymous types to a stream with full type safety.
        /// </summary>
        [TestMethod]
        public void TestTypeWriter_AnonymousType()
        {
            var people = from id in Enumerable.Range(0, 1)
                         select new 
                         { 
                             Id = id, 
                             Name = "Bob " + id,
                             Created = new DateTime(2013, 1, 19) 
                         };
            var mapper = SeparatedValueTypeMapper.DefineWriter(people);
            mapper.Property(p => p.Id).ColumnName("id");
            mapper.Property(p => p.Name).ColumnName("name");
            mapper.Property(p => p.Created).ColumnName("created").InputFormat("yyyyMMdd").OutputFormat("yyyyMMdd");

            using (MemoryStream stream = new MemoryStream())
            {
                var options = new SeparatedValueOptions() { IsFirstRecordSchema = true, Separator = "\t" };

                mapper.Write(stream, options);

                stream.Position = 0;  // go back to the beginning of the stream

                SeparatedValueSchema schema = mapper.GetSchema();
                FlatFileReader reader = new FlatFileReader(new SeparatedValueReader(stream, schema, options));
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
