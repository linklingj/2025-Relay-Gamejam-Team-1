using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SeolMJ.Pool
{
    public static class StringBuilderPool
    {
        static readonly Stack<StringBuilder> pool = new();

        public static StringBuilder Get()
        {
            if (pool.TryPop(out var result))
            {
                return result;
            }
            return new StringBuilder();
        }

        public static void Release(StringBuilder value)
        {
            value.Clear();
            pool.Push(value);
        }
    }
}
