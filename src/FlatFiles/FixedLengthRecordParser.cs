using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordParser
    {
        private readonly IRecordReader recordReader;

        public FixedLengthRecordParser(TextReader reader, FixedLengthSchema schema, FixedLengthOptions options)
        {
            if (options.HasRecordSeparator)
            {
                this.recordReader = new SeparatorRecordReader(reader, options.RecordSeparator);
            }
            else
            {
                this.recordReader = new FixedLengthRecordReader(reader, schema.TotalWidth);
            }
        }

        public bool IsEndOfStream()
        {
            return recordReader.IsEndOfStream();
        }

        public async ValueTask<bool> IsEndOfStreamAsync()
        {
            return await recordReader.IsEndOfStreamAsync();
        }

        public string ReadRecord()
        {
            return recordReader.ReadRecord();
        }

        public async Task<string> ReadRecordAsync()
        {
            return await recordReader.ReadRecordAsync();
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

            public SeparatorRecordReader(TextReader reader, string separator)
            {
                this.reader = new RetryReader(reader);
                this.matcher = SeparatorMatcher.GetMatcher(this.reader, separator);
            }

            public bool IsEndOfStream()
            {
                return reader.IsEndOfStream();
            }

            public async ValueTask<bool> IsEndOfStreamAsync()
            {
                return await reader.IsEndOfStreamAsync();
            }

            public string ReadRecord()
            {
                StringBuilder builder = new StringBuilder();
                while (!matcher.IsMatch() && reader.Read())
                {
                    builder.Append(reader.Current);
                }
                return builder.ToString();
            }

            public async Task<string> ReadRecordAsync()
            {
                StringBuilder builder = new StringBuilder();
                while (!await matcher.IsMatchAsync() && await reader.ReadAsync())
                {
                    builder.Append(reader.Current);
                }
                return builder.ToString();
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
                this.buffer = new char[totalWidth];
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
                length = await reader.ReadBlockAsync(buffer, 0, buffer.Length);
                if (length == 0)
                {
                    isEndOfStream = true;
                    return true;
                }
                return false;
            }

            public string ReadRecord()
            {
                return new String(buffer, 0, length);
            }

            public Task<string> ReadRecordAsync()
            {
                return Task.FromResult(new String(buffer, 0, length));
            }
        }
    }
}
