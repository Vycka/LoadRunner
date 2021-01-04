using System;

namespace Viki.LoadRunner.Engine.Utils
{
    public static class StringExtensions
    {
        public static string SubstringSafe(this string input, int startIndex, int length)
        {
            if (input == null)
            {
                return null;
            }

            if (input.Length - 1 < startIndex)
            {
                return String.Empty;
            }

            int safeLength = input.Length - startIndex;
            if (input.Length - startIndex < length)
            {
                length = safeLength;
            }

            return input.Substring(startIndex, length);
        }
    }
}