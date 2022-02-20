using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FlatFiles.Test
{
    [TestClass]
    public sealed class RawTester
    {
        [TestMethod]
        public void TestReadWrite_Comments()
        {
            StringWriter output = new StringWriter();
            DelimitedWriter writer = new DelimitedWriter(output);
            writer.Write(new[] { "a", "b", "c" });
            writer.WriteRaw("# Hello, world!!!", true);
            writer.Write(new[] { "d", "e", "f" });

            StringReader input = new StringReader(output.ToString());
            DelimitedReader reader = new DelimitedReader(input);
            reader.RecordRead += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length > 0 && e.Values[0].StartsWith("#");
            };
            Assert.IsTrue(reader.Read());
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, reader.GetValues());
            Assert.IsTrue(reader.Read());
            CollectionAssert.AreEqual(new[] { "d", "e", "f" }, reader.GetValues());
            Assert.IsFalse(reader.Read());
        }

        [TestMethod]
        public async Task TestReadWriteAsync_Comments()
        {
            StringWriter output = new StringWriter();
            DelimitedWriter writer = new DelimitedWriter(output);
            await writer.WriteAsync(new[] { "a", "b", "c" });
            await writer.WriteRawAsync("# Hello, world!!!", true);
            await writer.WriteAsync(new[] { "d", "e", "f" });

            StringReader input = new StringReader(output.ToString());
            DelimitedReader reader = new DelimitedReader(input);
            reader.RecordRead += (sender, e) =>
            {
                e.IsSkipped = e.Values.Length > 0 && e.Values[0].StartsWith("#");
            };
            Assert.IsTrue(await reader.ReadAsync());
            CollectionAssert.AreEqual(new[] { "a", "b", "c" }, reader.GetValues());
            Assert.IsTrue(await reader.ReadAsync());
            CollectionAssert.AreEqual(new[] { "d", "e", "f" }, reader.GetValues());
            Assert.IsFalse(await reader.ReadAsync());
        }
    }
}
