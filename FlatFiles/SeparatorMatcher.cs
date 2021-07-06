namespace FlatFiles
{
    internal static class SeparatorMatcher
    {
        public static ISeparatorMatcher GetMatcher(RetryReader reader, string separator)
        {
            if (separator == null)
            {
                return new DefaultSeparatorMatcher(reader);
            }
            return separator.Length switch
            {
                1 => new OneCharacterSeparatorMatcher(reader, separator[0]),
                2 => new TwoCharacterSeparatorMatcher(reader, separator[0], separator[1]),
                _ => new StringSeparatorMatcher(reader, separator),
            };
        }
    }
}
