using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FlatFiles
{
    internal sealed class RetryReader
    {
        private readonly TextReader reader;
        private readonly Stack<char> retry;
        private char current;

        public RetryReader(TextReader reader)
        {
            this.reader = reader;
            this.retry = new Stack<char>();
        }

        public bool EndOfStream
        {
            get { return retry.Count == 0 && reader.Peek() == -1; }
        }

        public char Current
        {
            get { return current; }
        }

        public bool Read()
        {
            if (retry.Count > 0)
            {
                current = retry.Pop();
                return true;
            }
            int next = reader.Read();
            if (next == -1)
            {
                return false;
            }
            current = (char)next;
            return true;
        }

        public bool IsMatch(char value)
        {
            if (retry.Count > 0)
            {
                if (retry.Peek() == value)
                {
                    current = retry.Pop();
                    return true;
                }
                return false;
            }
            int next = reader.Peek();
            if (next != -1)
            {
                if ((char)next == value)
                {
                    current = (char)reader.Read();
                    return true;
                }
            }
            return false;
        }

        public bool IsMatch(string value)
        {
            // Optimized for two character separators.
            int position = 0;
            if (!IsMatch(value[position]))
            {
                return false;
            }
            ++position;
            if (position == value.Length)
            {
                return true;
            }
            if (!IsMatch(value[position]))
            {
                Undo(value[0]);
                return false;
            }
            ++position;
            if (position == value.Length)
            {
                return true;
            }
            List<char> tail = new List<char>(value.Length);
            tail.Add(value[0]);
            tail.Add(value[1]);
            while (IsMatch(value[position]))
            {
                tail.Add(current);
                ++position;
                if (position == value.Length)
                {
                    return true;
                }
            }
            Undo(tail);
            return false;
        }

        public void Undo(char item)
        {
            retry.Push(item);
        }

        public void Undo(List<char> items)
        {
            int position = items.Count;
            while (position != 0)
            {
                --position;
                retry.Push(items[position]);
            }
        }
    }
}
