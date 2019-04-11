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
        private static string[] DefaultHeaderOrder(string[] input) => input;

        public static void SerializeToCsv<T>(this IEnumerable<T> input, string outFile, string[] headerOrder, bool includeHeaders = true)
        {
            SerializeToCsv(input, outFile, (i) => headerOrder, includeHeaders);
        }

        public static void SerializeToCsv<T>(this IEnumerable<T> input, string outFile, bool includeHeaders = true)
        {
            SerializeToCsv(input, outFile, DefaultHeaderOrder, includeHeaders);
        }

        public static void SerializeToCsv<T>(this IEnumerable<T> input, string outFile, Func<string[], string[]> headerOrder, bool includeHeaders = true)
        {
            using (StreamWriter writer = File.CreateText(outFile))
            {
                foreach (string line in input.SerializeToCsv(headerOrder, includeHeaders))
                {
                    writer.WriteLine(line);
                }
            }
        }

        public static IEnumerable<string> SerializeToCsv<T>(this IEnumerable<T> input, bool includeHeaders = true)
        {
            return SerializeToCsv(input, DefaultHeaderOrder, includeHeaders);
        }

        public static IEnumerable<string> SerializeToCsv<T>(this IEnumerable<T> input, string[] headerOrder, bool includeHeaders = true)
        {
            return SerializeToCsv(input, (i) => headerOrder,  includeHeaders);
        }

        public static IEnumerable<string> SerializeToCsv<T>(this IEnumerable<T> input, Func<string[], string[]> headerOrder, bool includeHeaders = true)
        {
            IEnumerable<string> result = Enumerable.Empty<string>();

            using (IEnumerator<T> enumerator = input.GetEnumerator())
            {
                if (enumerator.MoveNext() && FirstNotNull(enumerator))
                {
                    string[] headers = headerOrder(ExtractHeaders(enumerator.Current).ToArray());

                    if (includeHeaders)
                    {
                        result = result.Concat(new [] { String.Join(",", headers.Select(CsvValue)) });
                    }

                    object current = enumerator.Current;

                    if (current is IDictionary<string, object>) // Expando/Dynamic
                    {
                        result = result.Concat(SerializeDynamicType(enumerator, headers));
                    }
                    else if (IsAnonymous(current)) // Anonymous
                    {
                        result = result.Concat(SerializeAnonymousType(enumerator, headers));
                    }
                    else // Hopefully only strong typed
                    {
                        result = result.Concat(SerializeStrongType(enumerator, headers));
                    }
                }

                return result;
            }
        }

        private static IEnumerable<string> SerializeDynamicType<T>(IEnumerator<T> iterator, string[] headers)
        {
            do
            {
                IDictionary<string, object> row = (IDictionary<string, object>)iterator.Current;

                if (row != null)
                    yield return String.Join(",", headers.Select(h => CsvValue(row.GetOrNull(h))));
            } while (iterator.MoveNext());
        }

        private static IEnumerable<string> SerializeAnonymousType<T>(IEnumerator<T> iterator, string[] headers)
        {
            do
            {
                T current = iterator.Current;

                if (current != null)
                {
                    Type type = current.GetType();
                    yield return String.Join(",", headers.Select(h => CsvValue(type.GetProperty(h)?.GetValue(current))));
                }
            } while (iterator.MoveNext());
        }

        private static IEnumerable<string> SerializeStrongType<T>(IEnumerator<T> iterator, string[] headers)
        {
            FieldInfo[] allFields = iterator.Current.GetType().GetFields().Where(f => f.IsPublic).ToArray();
            FieldInfo[] fields = headers.Select(header => allFields.First(f => f.Name == header)).ToArray();

            do
            {
                T current = iterator.Current;

                if (current != null)
                    yield return String.Join(",", fields.Select(f => CsvValue(f.GetValue(current))));

            } while (iterator.MoveNext());
        }

        private static IEnumerable<string> ExtractHeaders<T>(T current)
        {
            if (current is IDictionary<string, object> expando)
            {
                return expando.Keys;
            }
            else
            {
                if (IsAnonymous(current))
                {
                    PropertyInfo[] properties = current.GetType().GetProperties();
                    return properties.Where(p => p.CanRead).Select(p => p.Name);
                }
                else
                {
                    FieldInfo[] fields = current.GetType().GetFields().Where(f => f.IsPublic).ToArray();
                    return fields.Select(p => p.Name);
                }
            }
        }

        private static bool IsAnonymous(object current)
        {
            Type type = current.GetType();
            return type.Name.StartsWith("<>") && type.Name.Contains("AnonymousType") && type.Namespace == null;
        }

        private static bool FirstNotNull<T>(IEnumerator<T> enumerator)
        {
            while (enumerator.Current == null && enumerator.MoveNext())
            {
            }

            return enumerator.Current != null;
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