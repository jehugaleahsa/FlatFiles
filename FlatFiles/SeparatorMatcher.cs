namespace FlatFiles
{
    internal interface ISeparatorMatcher
    {
        int Size { get; }

        bool IsMatch();
    }

    internal static class SeparatorMatcher
    {
        public static ISeparatorMatcher GetMatcher(RetryReader reader, string separator)
        {
            if (separator == null)
            {
                return new DefaultSeparatorMatcher(reader);
            }

            switch (separator.Length)
            {
                case 1:
                    return new OneCharacterSeparatorMatcher(reader, separator[0]);
                case 2:
                    return new TwoCharacterSeparatorMatcher(reader, separator[0], separator[1]);
            }

            return new StringSeparatorMatcher(reader, separator);
        }
    }

    internal sealed class DefaultSeparatorMatcher : ISeparatorMatcher
    {
        private readonly RetryReader _reader;

        public DefaultSeparatorMatcher(RetryReader reader)
        {
            _reader = reader;
        }

        public int Size => 2;

        public bool IsMatch()
        {
            if (_reader.IsMatch1('\r'))
            {
                _reader.IsMatch1('\n');
                return true;
            }

            return _reader.IsMatch1('\n');
        }
    }

    internal sealed class OneCharacterSeparatorMatcher : ISeparatorMatcher
    {
        private readonly RetryReader _reader;
        private readonly char _first;

        public OneCharacterSeparatorMatcher(RetryReader reader, char first)
        {
            _reader = reader;
            _first = first;
        }

        public int Size => 1;

        public bool IsMatch()
        {
            return _reader.IsMatch1(_first);
        }
    }

    internal sealed class TwoCharacterSeparatorMatcher : ISeparatorMatcher
    {
        private readonly RetryReader _reader;
        private readonly char _first;
        private readonly char _second;

        public TwoCharacterSeparatorMatcher(RetryReader reader, char first, char second)
        {
            _reader = reader;
            _first = first;
            _second = second;
        }

        public int Size => 2;

        public bool IsMatch()
        {
            return _reader.IsMatch2(_first, _second);
        }
    }

    internal sealed class StringSeparatorMatcher : ISeparatorMatcher
    {
        private readonly RetryReader _reader;
        private readonly string _separator;

        public StringSeparatorMatcher(RetryReader reader, string separator)
        {
            _reader = reader;
            _separator = separator;
        }
        public int Size => _separator.Length;

        public bool IsMatch()
        {
            return _reader.IsMatch(_separator);
        }
    }
}
