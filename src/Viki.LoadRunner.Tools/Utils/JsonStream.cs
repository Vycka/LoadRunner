using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Viki.LoadRunner.Tools.Utils
{
    //http://stackoverflow.com/questions/9026508/incremental-json-parsing-in-c-sharp
    public static class JsonStream
    {
        //Serialize sequence of objects as JSON array in to a specified file
        public static void SerializeSequenceToJson<T>(this IEnumerable<T> sequence, string fileName)
        {
            using (var fileStream = File.CreateText(fileName))
                SerializeSequenceToJson(sequence, fileStream);
        }

        //Deserialize specified file in to IEnumerable assuming it has array of JSON objects
        public static IEnumerable<T> DeserializeSequenceFromJson<T>(string fileName)
        {
            using (var fileStream = File.OpenText(fileName))
                foreach (var responseJson in DeserializeSequenceFromJson<T>(fileStream))
                    yield return responseJson;
        }

        //Utility methods to operate on streams instead of file
        public static void SerializeSequenceToJson<T>(this IEnumerable<T> sequence, TextWriter writeStream, Action<T, long> progress = null)
        {
            using (var writer = new JsonTextWriter(writeStream))
            {
                var serializer = new JsonSerializer();
                serializer.NullValueHandling = NullValueHandling.Ignore;
                serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

                writer.WriteStartArray();
                long index = 0;
                foreach (var item in sequence)
                {
                    if (progress != null)
                        progress(item, index++);

                    serializer.Serialize(writer, item);
                }
                writer.WriteEnd();
            }
        }
        public static IEnumerable<T> DeserializeSequenceFromJson<T>(TextReader readerStream)
        {
            using (var reader = new JsonTextReader(readerStream))
            {
                var serializer = new JsonSerializer();
                if (!reader.Read() || reader.TokenType != JsonToken.StartArray)
                    throw new Exception("Expected start of array in the deserialized json string");

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray) break;
                    var item = serializer.Deserialize<T>(reader);
                    yield return item;
                }
            }
        }
    }
}