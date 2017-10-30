using System;
using System.Threading.Tasks;

namespace FlatFiles
{
    internal interface ISeparatorMatcher
    {
        bool IsMatch();

        Task<bool> IsMatchAsync();
    }

    internal static class SeparatorMatcher
    {
        public static ISeparatorMatcher GetMatcher(RetryReader reader, string separator)
        {
            if (separator == null)
            {
                return new DefaultSeparatorMatcher(reader);
            }
            else if (separator.Length == 1)
            {
                return new OneCharacterSeparatorMatcher(reader, separator[0]);
            }
            else if (separator.Length == 2)
            {
                return new TwoCharacterSeparatorMatcher(reader, separator[0], separator[1]);
            }
            else
            {
                return new StringSeparatorMatcher(reader, separator);
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

        public bool IsMatch()
        {
            if (reader.IsMatch1('\r'))
            {
                reader.IsMatch1('\n');
                return true;
            }
            else if (reader.IsMatch1('\n'))
            {
                return true;
            }
            return false;
        }

        public async Task<bool> IsMatchAsync()
        {
            if (await reader.IsMatch1Async('\r'))
            {
                await reader.IsMatch1Async('\n');
                return true;
            }
            else if (await reader.IsMatch1Async('\n'))
            {
                return true;
            }
            return false;
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

        public bool IsMatch()
        {
            return reader.IsMatch1(first);
        }

        public async Task<bool> IsMatchAsync()
        {
            return await reader.IsMatch1Async(first);
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

        public bool IsMatch()
        {
            return reader.IsMatch2(first, second);
        }

        public async Task<bool> IsMatchAsync()
        {
            return await reader.IsMatch2Async(first, second);
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

        public bool IsMatch()
        {
            return reader.IsMatch(separator);
        }

        public async Task<bool> IsMatchAsync()
        {
            return await reader.IsMatchAsync(separator);
        }
    }
}
