using System.Collections;
using System.Collections.Generic;

namespace FlatFiles
{
    /// <summary>
    /// Holds the column windows that make up a schema.
    /// </summary>
    public sealed class WindowCollection : IEnumerable<Window>
    {
        private readonly List<Window> windows;

        /// <summary>
        /// Initializes a new WindowCollection.
        /// </summary>
        /// <param name="windows">The windows making up the collection.</param>
        internal WindowCollection(List<Window> windows)
        {
            this.windows = windows;
        }

        /// <summary>
        /// Gets the window at the given index.
        /// </summary>
        /// <param name="index">The index of the window to get.</param>
        /// <returns>The window at the given index.</returns>
        public Window this[int index]
        {
            get { return windows[index]; }
        }

        /// <summary>
        /// Gets the number of columns in the collection.
        /// </summary>
        public int Count
        {
            get { return windows.Count; }
        }

        /// <summary>
        /// Gets an enumerator over the column definitions in the collection.
        /// </summary>
        /// <returns>The enumerator.</returns>
        public IEnumerator<Window> GetEnumerator()
        {
            return windows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return windows.GetEnumerator();
        }
    }
}
