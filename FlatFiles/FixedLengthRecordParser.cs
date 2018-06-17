using System.IO;
using System.Text;
using System.Threading.Tasks;
using FlatFiles.Properties;

namespace FlatFiles
{
    internal sealed class FixedLengthRecordParser
    {
        private readonly IRecordReader _recordReader;

        public FixedLengthRecordParser(TextReader reader, FixedLengthSchema schema, FixedLengthOptions options)
        {
            if (options.HasRecordSeparator)
            {
                _recordReader = new SeparatorRecordReader(reader, options.RecordSeparator);
            }
            else if (schema == null)
            {
                throw new FlatFileException(Resources.RecordSeparatorRequired);
            }
            else
            {
                _recordReader = new FixedLengthRecordReader(reader, schema.TotalWidth);
            }
        }

        public bool IsEndOfStream()
        {
            return _recordReader.IsEndOfStream();
        }

        public async ValueTask<bool> IsEndOfStreamAsync()
        {
            return await _recordReader.IsEndOfStreamAsync().ConfigureAwait(false);
        }

        public string ReadRecord()
        {
            return _recordReader.ReadRecord();
        }

        public async Task<string> ReadRecordAsync()
        {
            return await _recordReader.ReadRecordAsync().ConfigureAwait(false);
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
            private readonly RetryReader _reader;
            private readonly ISeparatorMatcher _matcher;
            private readonly StringBuilder _builder;

            public SeparatorRecordReader(TextReader reader, string separator)
            {
                _reader = new RetryReader(reader);
                _matcher = SeparatorMatcher.GetMatcher(_reader, separator);
                _builder = new StringBuilder();
            }

            public bool IsEndOfStream()
            {
                if (_reader.ShouldLoadBuffer(1))
                {
                    _reader.LoadBuffer();
                }
                return _reader.IsEndOfStream();
            }

            public async ValueTask<bool> IsEndOfStreamAsync()
            {
                if (_reader.ShouldLoadBuffer(1))
                {
                    await _reader.LoadBufferAsync().ConfigureAwait(false);
                }
                return _reader.IsEndOfStream();
            }

            public string ReadRecord()
            {
                if (_reader.ShouldLoadBuffer(_matcher.Size))
                {
                    _reader.LoadBuffer();
                }
                while (!_matcher.IsMatch() && _reader.Read())
                {
                    _builder.Append(_reader.Current);
                    if (_reader.ShouldLoadBuffer(_matcher.Size))
                    {
                        _reader.LoadBuffer();
                    }
                }
                string record = _builder.ToString();
                _builder.Clear();
                return record;
            }

            public async Task<string> ReadRecordAsync()
            {
                if (_reader.ShouldLoadBuffer(_matcher.Size))
                {
                    await _reader.LoadBufferAsync().ConfigureAwait(false);
                }
                while (!_matcher.IsMatch() && _reader.Read())
                {
                    _builder.Append(_reader.Current);
                    if (_reader.ShouldLoadBuffer(_matcher.Size))
                    {
                        await _reader.LoadBufferAsync().ConfigureAwait(false);
                    }
                }
                string record = _builder.ToString();
                _builder.Clear();
                return record;
            }
        }

        private sealed class FixedLengthRecordReader : IRecordReader
        {
            private readonly TextReader _reader;
            private readonly char[] _buffer;
            private int _length;
            private bool _isEndOfStream;

            public FixedLengthRecordReader(TextReader reader, int totalWidth)
            {
                _reader = reader;
                _buffer = new char[totalWidth];
            }

            public bool IsEndOfStream()
            {
                if (_isEndOfStream)
                {
                    return true;
                }
                _length = _reader.ReadBlock(_buffer, 0, _buffer.Length);
                if (_length == 0)
                {
                    _isEndOfStream = true;
                    return true;
                }
                return false;
            }

            public async ValueTask<bool> IsEndOfStreamAsync()
            {
                if (_isEndOfStream)
                {
                    return true;
                }
                _length = await _reader.ReadBlockAsync(_buffer, 0, _buffer.Length).ConfigureAwait(false);
                if (_length == 0)
                {
                    _isEndOfStream = true;
                    return true;
                }
                return false;
            }

            public string ReadRecord()
            {
                return new string(_buffer, 0, _length);
            }

            public Task<string> ReadRecordAsync()
            {
                return Task.FromResult(new string(_buffer, 0, _length));
            }
        }
    }
}
