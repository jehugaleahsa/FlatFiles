namespace FlatFiles
{
    internal interface ISeparatorMatcher
    {
        int Size { get; }

        bool IsMatch();

        string Trim(string value);
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
                case 1: return new OneCharacterSeparatorMatcher(reader, separator[0]);
                case 2: return new TwoCharacterSeparatorMatcher(reader, separator[0], separator[1]);
                default: return new StringSeparatorMatcher(reader, separator);
            }
        }
    }

    internal sealed class DefaultSeparatorMatcher : ISeparatorMatcher
    {
        private readonly RetryReader reader;

        public DefaultSeparatorMatcher(RetryReader reader)
        {
            this.reader = reader;
        }

        public int Size => 2;

        public bool IsMatch()
        {
            if (reader.IsMatch1('\r'))
            {
                reader.IsMatch1('\n');
                return true;
            }
            return reader.IsMatch1('\n');
        }

        public string Trim(string value)
        {
            int length = value.Length;
            if (length > 0 && value[value.Length - 1] == '\n')
            {
                --length;
                if (length > 1 && value[value.Length - 2] == '\r')
                {
                    --length;
                }
                return value.Substring(0, length);
            }
            return value;
        }
    }

    internal sealed class OneCharacterSeparatorMatcher : ISeparatorMatcher
    {
        private readonly RetryReader reader;
        private readonly char first;

        public OneCharacterSeparatorMatcher(RetryReader reader, char first)
        {
            this.reader = reader;
            this.first = first;
        }

        public int Size => 1;

        public bool IsMatch()
        {
            return reader.IsMatch1(first);
        }

        public string Trim(string value)
        {
            if (value.Length > 0 || value[value.Length - 1] == first)
            {
                return value.Substring(0, value.Length - 1);
            }
            return value;
        }
    }

    internal sealed class TwoCharacterSeparatorMatcher : ISeparatorMatcher
    {
        private readonly RetryReader reader;
        private readonly char first;
        private readonly char second;

        public TwoCharacterSeparatorMatcher(RetryReader reader, char first, char second)
        {
            this.reader = reader;
            this.first = first;
            this.second = second;
        }

        public int Size => 2;

        public bool IsMatch()
        {
            return reader.IsMatch2(first, second);
        }

        public string Trim(string value)
        {
            int length = value.Length;
            if (length > 1 && value[value.Length - 2] == first && value[value.Length - 1] == second)
            {
                return value.Substring(0, value.Length - 2);
            }
            return value;
        }
    }

    internal sealed class StringSeparatorMatcher : ISeparatorMatcher
    {
        private readonly RetryReader reader;
        private readonly string separator;

        public StringSeparatorMatcher(RetryReader reader, string separator)
        {
            this.reader = reader;
            this.separator = separator;
        }

        public int Size => separator.Length;

        public bool IsMatch()
        {
            return reader.IsMatch(separator);
        }

        public string Trim(string value)
        {
            if (value.EndsWith(separator))
            {
                return value.Substring(0, value.Length - separator.Length);
            }
            return value;
        }
    }
}
