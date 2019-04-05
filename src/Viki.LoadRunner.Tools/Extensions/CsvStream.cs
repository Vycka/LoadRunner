using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Viki.LoadRunner.Tools.Extensions
{
    public static class CsvStream
    {
        private static readonly char[] CsvSpecialSymbols = { ',', '\"', '\r', '\n' };

        public static void SerializeToCsv<T>(this IEnumerable<T> input, string outFile, bool includeHeaders = true)
        {
            using (StreamWriter writer = File.CreateText(outFile))
            {
                foreach (string line in input.SerializeToCsv(includeHeaders))
                {
                    writer.WriteLine(line);
                }
            }
        }

        public static IEnumerable<string> SerializeToCsv<T>(this IEnumerable<T> input, bool includeHeaders = true)
        {
            using (IEnumerator<T> iterator = input.GetEnumerator())
            {
                if (iterator.MoveNext() && iterator.Current != null)
                {
                    if (iterator.Current is IDictionary<string, object>)
                    {
                        foreach (string line in SerializeExpando(iterator, includeHeaders))
                        {
                            yield return line;
                        }
                    }
                    else
                    {
                        foreach (string line in SerializeAnonymous(iterator, includeHeaders))
                        {
                            yield return line;
                        }
                    }
                }
            }
        }

        private static IEnumerable<string> SerializeExpando<T>(IEnumerator<T> iterator, bool includeHeaders)
        {
            if (iterator.Current == null)
                yield break;

            bool headersAdded = !includeHeaders;

            string[] headers = ((IDictionary<string, object>)iterator.Current).Select(kv => kv.Key).ToArray();

            do
            {
                if (!headersAdded)
                {
                    headersAdded = true;
                    yield return String.Join(",", headers.Select(CsvValue));
                }

                IDictionary<string, object> row = (IDictionary<string, object>)iterator.Current;

                yield return String.Join(",", headers.Select(h => CsvValue(row.GetOrNull(h))));
            } while (iterator.MoveNext());
        }

        private static IEnumerable<string> SerializeAnonymous<T>(IEnumerator<T> iterator, bool includeHeaders)
        {
            if (iterator.Current == null)
                yield break;

            bool headersAdded = !includeHeaders;

            FieldInfo[] fields = iterator.Current.GetType().GetFields().Where(f => f.IsPublic).ToArray();
            string[] headers = fields.Select(p => p.Name).ToArray();

            do
            {
                if (!headersAdded)
                {
                    headersAdded = true;
                    yield return String.Join(",", headers.Select(CsvValue));
                }

                T current = iterator.Current;

                yield return String.Join(",", fields.Select(f => CsvValue(f.GetValue(current))));
            } while (iterator.MoveNext());
        }

        private static string CsvValue(object value)
        {
            value = value ?? String.Empty;
            string valueString = value.ToString();

            if (valueString.IndexOfAny(CsvSpecialSymbols) == -1)
            {
                return valueString;
            }

            value = valueString.Replace("\"", "\"\"");

            return string.Concat("\"", value, "\"");
        }
    }
}