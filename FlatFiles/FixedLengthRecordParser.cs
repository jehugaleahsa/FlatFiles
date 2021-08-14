using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordParser
    {
        private readonly IRecordReader recordReader;

        public FixedLengthRecordParser(TextReader reader, FixedLengthSchema? schema, FixedLengthOptions options)
        {
            // When no record separator is specified, we must rely on the total width of the windows
            // to figure out how much to read. If a separator is provided, we just read up to that
            // separator. We can then pass that record to the schema selector to determine the schema
            // afterwards.
            if (options.HasRecordSeparator)
            {
                recordReader = new SeparatorRecordReader(reader, options.RecordSeparator);
            }
            else if (schema == null)
            {
                throw new FlatFileException(Resources.RecordSeparatorRequired);
            }
            else
            {
                recordReader = new FixedLengthRecordReader(reader, schema.TotalWidth);
            }
        }

        public bool IsEndOfStream()
        {
            return recordReader.IsEndOfStream();
        }

        public ValueTask<bool> IsEndOfStreamAsync()
        {
            return recordReader.IsEndOfStreamAsync();
        }

        public string ReadRecord()
        {
            return recordReader.ReadRecord();
        }

        public Task<string> ReadRecordAsync()
        {
            return recordReader.ReadRecordAsync();
        }

        private interface IRecordReader
        {
            bool IsEndOfStream();

            ValueTask<bool> IsEndOfStreamAsync();

            string ReadRecord();

            Task<string> ReadRecordAsync();
        }

        private sealed class SeparatorRecordReader : IRecordReader
        {
            private readonly RetryReader reader;
            private readonly ISeparatorMatcher matcher;
            private readonly StringBuilder builder;

            public SeparatorRecordReader(TextReader reader, string? separator)
            {
                this.reader = new RetryReader(reader);
                matcher = SeparatorMatcher.GetMatcher(this.reader, separator);
                builder = new StringBuilder();
            }

            public bool IsEndOfStream()
            {
                if (reader.ShouldLoadBuffer(1))
                {
                    reader.LoadBuffer();
                }
                return reader.IsEndOfStream();
            }

            public async ValueTask<bool> IsEndOfStreamAsync()
            {
                if (reader.ShouldLoadBuffer(1))
                {
                    await reader.LoadBufferAsync().ConfigureAwait(false);
                }
                return reader.IsEndOfStream();
            }

            public string ReadRecord()
            {
                if (reader.ShouldLoadBuffer(matcher.Size))
                {
                    reader.LoadBuffer();
                }
                while (!matcher.IsMatch() && reader.Read())
                {
                    builder.Append(reader.Current);
                    if (reader.ShouldLoadBuffer(matcher.Size))
                    {
                        reader.LoadBuffer();
                    }
                }
                string record = builder.ToString();
                builder.Clear();
                return record;
            }

            public async Task<string> ReadRecordAsync()
            {
                if (reader.ShouldLoadBuffer(matcher.Size))
                {
                    await reader.LoadBufferAsync().ConfigureAwait(false);
                }
                while (!matcher.IsMatch() && reader.Read())
                {
                    builder.Append(reader.Current);
                    if (reader.ShouldLoadBuffer(matcher.Size))
                    {
                        await reader.LoadBufferAsync().ConfigureAwait(false);
                    }
                }
                string record = builder.ToString();
                builder.Clear();
                return record;
            }
        }

        private sealed class FixedLengthRecordReader : IRecordReader
        {
            private readonly TextReader reader;
            private readonly char[] buffer;
            private int length;
            private bool isEndOfStream;

            public FixedLengthRecordReader(TextReader reader, int totalWidth)
            {
                this.reader = reader;
                buffer = new char[totalWidth];
            }

            public bool IsEndOfStream()
            {
                if (isEndOfStream)
                {
                    return true;
                }
                length = reader.ReadBlock(buffer, 0, buffer.Length);
                if (length == 0)
                {
                    isEndOfStream = true;
                    return true;
                }
                return false;
            }

            public async ValueTask<bool> IsEndOfStreamAsync()
            {
                if (isEndOfStream)
                {
                    return true;
                }
                length = await reader.ReadBlockAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                if (length == 0)
                {
                    isEndOfStream = true;
                    return true;
                }
                return false;
            }

            public string ReadRecord()
            {
                return new string(buffer, 0, length);
            }

            public Task<string> ReadRecordAsync()
            {
                return Task.FromResult(new string(buffer, 0, length));
            }
        }
    }
}
