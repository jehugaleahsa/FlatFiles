namespace FlatFiles
{
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
