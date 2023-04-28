using System.Collections.Generic;

namespace Pixelbyte
{
    /// <summary>
    /// This should reduce the garbage allocated when accessing our pool dictionaries
    /// When the default comparer is used to lookup the key, it generates garbage
    /// </summary>
    class IntEqualityComparer : IEqualityComparer<int>
    {
        public bool Equals(int x, int y) => x == y;

        public int GetHashCode(int obj) =>obj.GetHashCode();
    }
}