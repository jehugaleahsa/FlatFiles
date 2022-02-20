namespace FlatFiles
{
    internal sealed class DefaultRecordSeparatorMatcher : IRecordSeparatorMatcher
    {
        private readonly RetryReader reader;

        public DefaultRecordSeparatorMatcher(RetryReader reader)
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
            if (length >= 1 && value[length - 1] == '\n')
            {
                --length;
                if (length >= 1 && value[length - 1] == '\r')
                {
                    --length;
                }
                return value.Substring(0, length);
            }
            return value;
        }
    }
}
