using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Viki.LoadRunner.Tools.Extensions
{
    public static class CsvStream
    {
        public static Func<object, string> ValueFormatter { get; set; } = o => o.ToString(); 

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
            return SerializeToCsv(input, (i) => headerOrder, includeHeaders);
        }
        public static IEnumerable<string> SerializeToCsv<T>(this IEnumerable<T> input, Func<string[], string[]> headerOrder, bool includeHeaders = true)
        {
            IEnumerable<string> result = Enumerable.Empty<string>();
            IEnumerator<T> enumerator = input.GetEnumerator();
            if (enumerator.MoveNext() && FirstNotNull(enumerator))
            {
                object current = enumerator.Current;
                string[] headers = headerOrder(ExtractHeaders(current).ToArray());
                if (headers.Length == 0)
                {
                    enumerator.Dispose();
                    return result;
                }

                if (includeHeaders)
                {
                    result = result.Concat(new[] { String.Join(",", headers.Select(CsvValue)) });
                }

                if (current is ICustomTypeDescriptor) // NewtonSoft json
                {
                    result = result.Concat(SerializeCustomTypeDescriptor(enumerator, headers));
                }
                else if (current is IDictionary<string, object>) // Expando
                {
                    result = result.Concat(SerializeExpando(enumerator, headers));
                }
                else if (IsAnonymous(current)) // Anonymous
                {
                    result = result.Concat(SerializeAnonymousType(enumerator, headers));
                }
                else // StrongType 
                {
                    result = result.Concat(SerializeStrongType(enumerator, headers));
                }
            }
            return result;
            
        }

        private static IEnumerable<string> SerializeCustomTypeDescriptor<T>(IEnumerator<T> iterator, string[] headers)
        {
            do
            {
                ICustomTypeDescriptor row = (ICustomTypeDescriptor)iterator.Current;
                if (row != null)
                {
                    PropertyDescriptorCollection properties = row.GetProperties();
                    yield return String.Join(",", headers.Select(h => CsvValue(properties[h]?.GetValue(row))));
                }
            } while (iterator.MoveNext());

            iterator.Dispose();
        }

        private static IEnumerable<string> SerializeExpando<T>(IEnumerator<T> iterator, string[] headers)
        {
            do
            {
                IDictionary<string, object> row = (IDictionary<string, object>)iterator.Current;
                if (row != null)
                    yield return String.Join(",", headers.Select(h => CsvValue(row.GetOrNull(h))));
            } while (iterator.MoveNext());

            iterator.Dispose();
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

            iterator.Dispose();
        }

        private static IEnumerable<string> SerializeStrongType<T>(IEnumerator<T> iterator, string[] headers)
        {
            Func<object, object>[] valueSelectors = BuildValueSelectors(headers, iterator.Current).ToArray();
            
            do
            {
                T current = iterator.Current;
                if (current != null)
                {
                    yield return String.Join(",", valueSelectors.Select(selector => CsvValue(selector(current))));
                }
                    
            } while (iterator.MoveNext());

            iterator.Dispose();
        }

        private static IEnumerable<Func<object, object>> BuildValueSelectors<T>(string[] headers, T current)
        {
            PropertyInfo[] properties = current.GetType().GetProperties().Where(p => p.CanRead).ToArray();
            FieldInfo[] fields = current.GetType().GetFields().Where(f => f.IsPublic).ToArray();

            foreach (string header in headers)
            {

                PropertyInfo property = properties.FirstOrDefault(p => p.Name == header);
                if (property != null)
                {
                    yield return (obj) => property.GetValue(obj);
                }
                else
                {
                    FieldInfo field = fields.FirstOrDefault(f => f.Name == header);
                    if (field != null)
                    {
                        yield return (obj) => field.GetValue(obj);
                    }
                }
            }
        }

        private static IEnumerable<string> ExtractHeaders<T>(T current)
        {
            if (current is ICustomTypeDescriptor typeDescriptor)
            {
                return SelectCustomTypeDescriptorHeaders(typeDescriptor);
            }
            else if (current is IDictionary<string, object> expando)
            {
                return expando.Keys;
            }
            else
            {
                IEnumerable<PropertyInfo> properties = current.GetType().GetProperties().Where(p => p.CanRead);
                IEnumerable<FieldInfo> fields = current.GetType().GetFields().Where(f => f.IsPublic);

                return properties.Select(p => p.Name)
                    .Concat(fields.Select(p => p.Name));
            }
        }
        private static IEnumerable<string> SelectCustomTypeDescriptorHeaders(ICustomTypeDescriptor typeDescriptor)
        {
            PropertyDescriptorCollection properties = typeDescriptor.GetProperties();
            for (int i = 0; i < properties.Count; i++)
            {
                yield return properties[i].Name;
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
            if (value is Array arrayValue)
                value = String.Join(", ", arrayValue);
            value = value ?? String.Empty;
            string valueString = ValueFormatter(value);
            if (valueString.IndexOfAny(CsvSpecialSymbols) == -1)
            {
                return valueString;
            }
            value = valueString.Replace("\"", "\"\"");
            return string.Concat("\"", value, "\"");
        }
    }
}